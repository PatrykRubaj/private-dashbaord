import { mkdtemp, writeFile } from "node:fs/promises";
import { execFileSync } from "node:child_process";
import { tmpdir } from "node:os";
import { join } from "node:path";
import { Type } from "typebox";
import { Text } from "@earendil-works/pi-tui";
import {
  DEFAULT_MAX_BYTES,
  DEFAULT_MAX_LINES,
  formatSize,
  truncateTail,
  withFileMutationQueue,
  type ExtensionAPI,
  type TruncationResult,
} from "@earendil-works/pi-coding-agent";

const SOLUTION = "Api.sln";
const DEFAULT_TIMEOUT_MS = 5 * 60_000;

const BuildParams = Type.Object({
  project: Type.Optional(
    Type.String({
      description:
        "Project or solution path to build (default: Api.sln in cwd). e.g. Core.Tests/Core.Tests.csproj",
    }),
  ),
  configuration: Type.Optional(
    Type.String({ description: "Build configuration (default: Debug)" }),
  ),
  noRestore: Type.Optional(
    Type.Boolean({ description: "Skip implicit dotnet restore (default: false)" }),
  ),
});

const TestParams = Type.Object({
  project: Type.Optional(
    Type.String({
      description:
        "Test project path to run (default: Api.sln). e.g. Core.Tests/Core.Tests.csproj",
    }),
  ),
  filter: Type.Optional(
    Type.String({
      description:
        "Fully qualified filter expression passed via --filter. e.g. \"FullyQualifiedName~AirGradient\"",
    }),
  ),
  configuration: Type.Optional(
    Type.String({ description: "Build configuration (default: Debug)" }),
  ),
  noBuild: Type.Optional(
    Type.Boolean({ description: "Do not build before testing (default: false)" }),
  ),
  verbosity: Type.Optional(
    Type.String({
      description: "Test logger verbosity: quiet|minimal|normal|detailed|diagnostic (default: normal)",
    }),
  ),
});

interface DotnetDetails {
  command: string;
  exitCode: number;
  durationMs: number;
  truncation?: TruncationResult;
  fullOutputPath?: string;
}

interface ExecResult {
  output: string;
  exitCode: number;
  timedOut: boolean;
}

function runDotnet(args: string[], cwd: string, timeoutMs: number): ExecResult {
  try {
    const output = execFileSync("dotnet", args, {
      cwd,
      encoding: "utf-8",
      maxBuffer: 100 * 1024 * 1024,
      timeout: timeoutMs,
    });
    return { output, exitCode: 0, timedOut: false };
  } catch (err: any) {
    if (err.code === "ETIMEDOUT") {
      return {
        output: typeof err.stdout === "string" ? err.stdout : "",
        exitCode: err.status ?? 124,
        timedOut: true,
      };
    }
    const stdout = typeof err.stdout === "string" ? err.stdout : "";
    const stderr = typeof err.stderr === "string" ? err.stderr : "";
    const combined = [stdout, stderr].filter(Boolean).join("\n");
    return {
      output: combined || err.message,
      exitCode: typeof err.status === "number" ? err.status : 1,
      timedOut: false,
    };
  }
}

async function formatOutput(
  raw: string,
  command: string,
  exitCode: number,
  durationMs: number,
  timedOut: boolean,
): Promise<{ text: string; details: DotnetDetails }> {
  const truncation = truncateTail(raw, {
    maxLines: DEFAULT_MAX_LINES,
    maxBytes: DEFAULT_MAX_BYTES,
  });

  const details: DotnetDetails = { command, exitCode, durationMs };
  let text = truncation.content;

  const banner = timedOut ? `[timed out after ${durationMs}ms]\n` : "";

  if (truncation.truncated) {
    const tempDir = await mkdtemp(join(tmpdir(), "pi-dotnet-"));
    const tempFile = join(tempDir, "output.txt");
    await withFileMutationQueue(tempFile, async () => {
      await writeFile(tempFile, raw, "utf8");
    });

    details.truncation = truncation;
    details.fullOutputPath = tempFile;

    text +=
      `\n\n[Output truncated: showing last ${truncation.outputLines} of ${truncation.totalLines} lines` +
      ` (${formatSize(truncation.outputBytes)} of ${formatSize(truncation.totalBytes)}).` +
      ` Full output saved to: ${tempFile}]`;
  }

  return { text: banner + text, details };
}

export default function (pi: ExtensionAPI) {
  pi.registerTool({
    name: "dotnet_build",
    label: "dotnet build",
    description: `Run 'dotnet build' on Api.sln (or a specific project). Output is truncated to ${DEFAULT_MAX_LINES} lines / ${formatSize(DEFAULT_MAX_BYTES)}; full output is saved to a temp file when truncated.`,
    promptSnippet: "Build the .NET solution via dotnet build",
    promptGuidelines: [
      "Use dotnet_build to verify code compiles after making changes. By default build the whole solution (Api.sln); only scope to the single changed project when you are certain no other project is affected. Pass noRestore=true only if a restore just ran, since skipping restore on a stale obj/bin can hide real errors.",
    ],
    parameters: BuildParams,

    async execute(_id, params, signal, _onUpdate, ctx) {
      const args = ["build", params.project ?? SOLUTION];
      if (params.configuration) args.push("-c", params.configuration);
      if (params.noRestore) args.push("--no-restore");

      const command = `dotnet ${args.join(" ")}`;
      const start = Date.now();
      const { output, exitCode, timedOut } = runDotnet(
        args,
        ctx.cwd,
        DEFAULT_TIMEOUT_MS,
      );
      if (signal?.aborted) throw new Error("aborted");

      const { text, details } = await formatOutput(
        output,
        command,
        exitCode,
        Date.now() - start,
        timedOut,
      );
      return {
        content: [{ type: "text", text }],
        details,
        isError: exitCode !== 0,
      };
    },

    renderCall(args, theme) {
      let text = theme.fg("toolTitle", theme.bold("dotnet build "));
      text += theme.fg("accent", args.project ?? SOLUTION);
      if (args.configuration) {
        text += theme.fg("dim", ` -c ${args.configuration}`);
      }
      if (args.noRestore) {
        text += theme.fg("dim", " --no-restore");
      }
      return new Text(text, 0, 0);
    },

    renderResult(result, { isPartial }, theme) {
      if (isPartial) {
        return new Text(theme.fg("warning", "Building..."), 0, 0);
      }
      const details = result.details as DotnetDetails | undefined;
      if (!details) {
        return new Text(theme.fg("dim", "done"), 0, 0);
      }
      const ok = details.exitCode === 0;
      const status = ok
        ? theme.fg("success", "Build succeeded")
        : theme.fg("error", `Build failed (exit ${details.exitCode})`);
      const meta = theme.fg(
        "muted",
        ` \u00b7 ${details.durationMs}ms${details.truncation?.truncated ? " \u00b7 truncated" : ""}`,
      );
      return new Text(status + meta, 0, 0);
    },
  });

  pi.registerTool({
    name: "dotnet_test",
    label: "dotnet test",
    description: `Run 'dotnet test' on Api.sln (or a specific project). Output is truncated to ${DEFAULT_MAX_LINES} lines / ${formatSize(DEFAULT_MAX_BYTES)}; full output is saved to a temp file when truncated.`,
    promptSnippet: "Run the xUnit test suite via dotnet test",
    promptGuidelines: [
      "Use dotnet_test to verify changes. Pass --filter via the filter parameter when the user names a specific test or feature (e.g. \"FullyQualifiedName~AirGradient\"). Prefer the Core.Tests project path when only unit tests are needed.",
    ],
    parameters: TestParams,

    async execute(_id, params, signal, _onUpdate, ctx) {
      const args = ["test", params.project ?? SOLUTION];
      if (params.configuration) args.push("-c", params.configuration);
      if (params.filter) args.push("--filter", params.filter);
      if (params.noBuild) args.push("--no-build");
      if (params.verbosity) args.push("--verbosity", params.verbosity);

      const command = `dotnet ${args.join(" ")}`;
      const start = Date.now();
      const { output, exitCode, timedOut } = runDotnet(
        args,
        ctx.cwd,
        DEFAULT_TIMEOUT_MS,
      );
      if (signal?.aborted) throw new Error("aborted");

      const { text, details } = await formatOutput(
        output,
        command,
        exitCode,
        Date.now() - start,
        timedOut,
      );
      return {
        content: [{ type: "text", text }],
        details,
        isError: exitCode !== 0,
      };
    },

    renderCall(args, theme) {
      let text = theme.fg("toolTitle", theme.bold("dotnet test "));
      text += theme.fg("accent", args.project ?? SOLUTION);
      if (args.filter) {
        text += theme.fg("dim", ` --filter "${args.filter}"`);
      }
      if (args.noBuild) {
        text += theme.fg("dim", " --no-build");
      }
      if (args.verbosity) {
        text += theme.fg("dim", ` -v ${args.verbosity}`);
      }
      return new Text(text, 0, 0);
    },

    renderResult(result, { isPartial }, theme) {
      if (isPartial) {
        return new Text(theme.fg("warning", "Running tests..."), 0, 0);
      }
      const details = result.details as DotnetDetails | undefined;
      if (!details) {
        return new Text(theme.fg("dim", "done"), 0, 0);
      }
      const ok = details.exitCode === 0;
      const status = ok
        ? theme.fg("success", "Tests passed")
        : theme.fg("error", `Tests failed (exit ${details.exitCode})`);
      const meta = theme.fg(
        "muted",
        ` \u00b7 ${details.durationMs}ms${details.truncation?.truncated ? " \u00b7 truncated" : ""}`,
      );
      return new Text(status + meta, 0, 0);
    },
  });
}
