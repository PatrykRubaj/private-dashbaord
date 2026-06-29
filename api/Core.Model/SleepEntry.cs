namespace Core.Model;

public class SleepEntry
{
    public int Id { get; set; }
    public required DateOnly Date { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime Until { get; set; }
    public required double SleepHours { get; set; }
    public double? RechargePercent { get; set; }
    public double? CreditPercent { get; set; }
    public double? DebtPercent { get; set; }
    public double? BalancePercent { get; set; }
    public double? SleepPercent { get; set; }
    public double? RemHours { get; set; }
    public double? RemPercent { get; set; }
    public double? DeepHours { get; set; }
    public double? DeepPercent { get; set; }
    public double? Bpm { get; set; }
    public double? BpmPercent { get; set; }
    public double? SleepRating { get; set; }
    public required string OwnerId { get; set; }
}
