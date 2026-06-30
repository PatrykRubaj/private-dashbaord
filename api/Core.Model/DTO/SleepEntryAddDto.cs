namespace Core.Model.DTO;

public class SleepEntryAddDto
{
    public required DateOnly Date { get; set; }

    public required DateTimeOffset Start { get; set; }

    public required DateTimeOffset Until { get; set; }

    public required decimal SleepHours { get; set; }

    public decimal? RechargePercent { get; set; }
    public decimal? CreditPercent { get; set; }
    public decimal? DebtPercent { get; set; }
    public decimal? BalancePercent { get; set; }
    public decimal? SleepPercent { get; set; }

    public decimal? RemHours { get; set; }
    public decimal? RemPercent { get; set; }
    public decimal? DeepHours { get; set; }
    public decimal? DeepPercent { get; set; }

    public int? Bpm { get; set; }
    public decimal? BpmPercent { get; set; }
    public decimal? SleepRating { get; set; }
}
