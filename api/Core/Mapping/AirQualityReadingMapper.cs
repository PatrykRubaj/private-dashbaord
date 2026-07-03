using Core.Model;
using Core.Model.DTO;

namespace Core.Mapping;

public static class AirQualityReadingMapper
{
    public static AirQualityReadingDto ToDto(AirQualityLog log, Sensor sensor) => new()
    {
        SensorId = sensor.Id,
        SensorName = sensor.Name,
        SensorIpAddress = sensor.IpAddress,
        AccentColor = sensor.AccentColor,
        Timestamp = log.Timestamp,
        Pm01 = log.Pm01,
        Pm02 = log.Pm02,
        Pm10 = log.Pm10,
        Rco2 = log.Rco2,
        Atmp = log.Atmp,
        Rhum = log.Rhum,
        TvocIndex = log.TvocIndex,
        NoxIndex = log.NoxIndex,
        Wifi = log.Wifi,
    };
}
