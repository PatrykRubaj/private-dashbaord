using System.Net.Http.Json;
using Core.Model;
using Core.Model.DTO;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public interface IAirGradientClient
{
    Task<AirQualityLog?> FetchCurrentAsync(Sensor sensor, CancellationToken cancellationToken = default);
}

public class AirGradientClient : IAirGradientClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AirGradientClient> _logger;

    public AirGradientClient(HttpClient httpClient, ILogger<AirGradientClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AirQualityLog?> FetchCurrentAsync(Sensor sensor, CancellationToken cancellationToken = default)
    {
        var uriBuilder = new UriBuilder($"http://{sensor.IpAddress}/measures/current");

        AirGradientMeasureDto? measure;
        try
        {
            measure = await _httpClient.GetFromJsonAsync<AirGradientMeasureDto>(uriBuilder.Uri, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch measures from sensor {SensorId} ({IpAddress}).", sensor.Id, sensor.IpAddress);
            return null;
        }

        if (measure is null)
        {
            return null;
        }

        return new AirQualityLog
        {
            SensorId = sensor.Id,
            Timestamp = DateTimeOffset.UtcNow,
            Pm01 = measure.Pm01,
            Pm02 = measure.Pm02Compensated ?? measure.Pm02,
            Pm10 = measure.Pm10,
            Rco2 = measure.Rco2,
            Atmp = measure.AtmpCompensated ?? measure.Atmp,
            Rhum = measure.RhumCompensated ?? measure.Rhum,
            TvocIndex = measure.TvocIndex,
            NoxIndex = measure.NoxIndex,
            Wifi = measure.Wifi,
        };
    }
}
