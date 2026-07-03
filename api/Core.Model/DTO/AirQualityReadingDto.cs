namespace Core.Model.DTO;

public class AirQualityReadingDto
{
    public int SensorId { get; set; }
    public required string SensorName { get; set; }
    public required string SensorIpAddress { get; set; }
    public required string AccentColor { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public decimal Pm01 { get; set; }
    public decimal Pm02 { get; set; }
    public decimal Pm10 { get; set; }
    public int Rco2 { get; set; }
    public decimal Atmp { get; set; }
    public decimal Rhum { get; set; }
    public int TvocIndex { get; set; }
    public int NoxIndex { get; set; }
    public int Wifi { get; set; }
}
