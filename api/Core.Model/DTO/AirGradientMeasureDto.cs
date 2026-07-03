namespace Core.Model.DTO;

public class AirGradientMeasureDto
{
    public int Wifi { get; set; }
    public int Rco2 { get; set; }
    public decimal Pm01 { get; set; }
    public decimal Pm02 { get; set; }
    public decimal? Pm02Compensated { get; set; }
    public decimal Pm10 { get; set; }
    public decimal Atmp { get; set; }
    public decimal? AtmpCompensated { get; set; }
    public decimal Rhum { get; set; }
    public decimal? RhumCompensated { get; set; }
    public int TvocIndex { get; set; }
    public int NoxIndex { get; set; }
}
