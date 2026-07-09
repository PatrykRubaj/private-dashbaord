import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import astroPlugin from 'eslint-plugin-astro';
import reactPlugin from 'eslint-plugin-react';
import reactHooksPlugin from 'eslint-plugin-react-hooks';
import prettierConfig from 'eslint-config-prettier';
import { defineConfig } from 'astro/config';

// Restrict the type-checked ruleset to real TS/JS files so it never collides
// with the Astro parser, which can't supply parserServices on its own.
const typedFiles = ['**/*.{ts,tsx,mts,cts,js,mjs,cjs,jsx,tsx}'];
const typedConfigs = tseslint.configs.strictTypeChecked.map((config) =>
  config.rules && !config.files ? { ...config, files: typedFiles } : config,
);

export default defineConfig([
  { ignores: ['dist/**', '.astro/**', '*.mjs', '*.config.*', '.vscode/**'] },
  eslint.configs.recommended,
  // Type-checked TS/JS rules, scoped to non-Astro source files.
  ...typedConfigs,
  {
    files: typedFiles,
    languageOptions: {
      parserOptions: {
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
  },
  {
    files: ['**/*.{jsx,tsx}'],
    plugins: { react: reactPlugin, 'react-hooks': reactHooksPlugin },
    settings: { react: { version: 'detect' } },
    rules: {
      ...reactPlugin.configs.recommended.rules,
      ...reactHooksPlugin.configs.recommended.rules,
      'react/react-in-jsx-scope': 'off',
    },
  },
  {
    files: ['src/env.d.ts'],
    rules: { '@typescript-eslint/triple-slash-reference': 'off' },
  },
  // Astro plugin LAST so its parser/processor win for *.astro files.
  ...astroPlugin.configs.recommended,
  prettierConfig,
]);
