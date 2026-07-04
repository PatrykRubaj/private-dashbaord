using Core.Mapping;
using Core.Model;
using Core.Model.DTO;

namespace Core.Tests;

public class AirQualityReadingMapperTests
{
    [Fact]
    public void ToDto_copies_log_and_sensor_fields()
    {
        var sensor = new Sensor
        {
            Id = 2,
            Name = "office",
            IpAddress = "10.1.0.117",
            IsActive = true,
            AccentColor = "#22c55e",
        };
        var timestamp = DateTimeOffset.UtcNow;
        var log = new AirQualityLog
        {
            Id = 99,
            SensorId = sensor.Id,
            Timestamp = timestamp,
            Pm01 = 3m,
            Pm02 = 9m,
            Pm10 = 8m,
            Rco2 = 447m,
            Atmp = 24.47m,
            Rhum = 49m,
            TvocIndex = 100m,
            NoxIndex = 1m,
            Wifi = -46,
        };

        var dto = AirQualityReadingMapper.ToDto(log, sensor);

        Assert.Equal(sensor.Id, dto.SensorId);
        Assert.Equal(sensor.Name, dto.SensorName);
        Assert.Equal(sensor.IpAddress, dto.SensorIpAddress);
        Assert.Equal(sensor.AccentColor, dto.AccentColor);
        Assert.Equal(timestamp, dto.Timestamp);
        Assert.Equal(3m, dto.Pm01);
        Assert.Equal(9m, dto.Pm02);
        Assert.Equal(8m, dto.Pm10);
        Assert.Equal(447m, dto.Rco2);
        Assert.Equal(24.47m, dto.Atmp);
        Assert.Equal(49m, dto.Rhum);
        Assert.Equal(100m, dto.TvocIndex);
        Assert.Equal(1m, dto.NoxIndex);
        Assert.Equal(-46, dto.Wifi);
    }
}
