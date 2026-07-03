namespace Core.Model;

public class AirQualityLog
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public Sensor Sensor { get; set; } = null!;
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
