namespace Core.Model;

public class Sensor
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string IpAddress { get; set; }
    public bool IsActive { get; set; }
    public required string AccentColor { get; set; }
}
