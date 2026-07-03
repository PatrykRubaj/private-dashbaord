using System.Net;
using System.Text;
using Core.Model;
using Core.Services;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Core.Tests;

public class AirGradientClientTests
{
    private static Sensor Sensor(int id = 1, string ip = "10.1.0.191") => new()
    {
        Id = id,
        Name = "bedroom",
        IpAddress = ip,
        IsActive = true,
        AccentColor = "#6366f1",
    };

    private static AirGradientClient CreateClient(HttpResponseMessage response)
    {
        var handler = new StubHandler(_ => response);
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://stub/") };
        return new AirGradientClient(http, NoopLogger<AirGradientClient>.Instance);
    }

    [Fact]
    public async Task FetchCurrentAsync_prefers_compensated_values_when_present()
    {
        var json = """
        {
            "wifi": -46,
            "rco2": 447,
            "pm01": 3,
            "pm02": 7,
            "pm02Compensated": 9,
            "pm10": 8,
            "atmp": 25.87,
            "atmpCompensated": 24.47,
            "rhum": 43,
            "rhumCompensated": 49,
            "tvocIndex": 100,
            "noxIndex": 1
        }
        """;
        var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        });

        var log = await client.FetchCurrentAsync(Sensor());

        Assert.NotNull(log);
        Assert.Equal(9m, log!.Pm02);
        Assert.Equal(24.47m, log.Atmp);
        Assert.Equal(49m, log.Rhum);
        Assert.Equal(447, log.Rco2);
        Assert.Equal(3m, log.Pm01);
        Assert.Equal(8m, log.Pm10);
        Assert.Equal(100, log.TvocIndex);
        Assert.Equal(1, log.NoxIndex);
        Assert.Equal(-46, log.Wifi);
        Assert.Equal(1, log.SensorId);
    }

    [Fact]
    public async Task FetchCurrentAsync_falls_back_to_plain_values_without_compensated()
    {
        var json = """
        {
            "wifi": -46,
            "rco2": 447,
            "pm01": 3,
            "pm02": 7,
            "pm10": 8,
            "atmp": 25.87,
            "rhum": 43,
            "tvocIndex": 100,
            "noxIndex": 1
        }
        """;
        var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        });

        var log = await client.FetchCurrentAsync(Sensor());

        Assert.NotNull(log);
        Assert.Equal(7m, log!.Pm02);
        Assert.Equal(25.87m, log.Atmp);
        Assert.Equal(43m, log.Rhum);
    }

    [Fact]
    public async Task FetchCurrentAsync_returns_null_on_http_failure()
    {
        var client = CreateClient(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var log = await client.FetchCurrentAsync(Sensor());

        Assert.Null(log);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) => _respond = respond;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_respond(request));
        }
    }

    private sealed class NoopLogger<T> : ILogger<T>
    {
        public static readonly NoopLogger<T> Instance = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
