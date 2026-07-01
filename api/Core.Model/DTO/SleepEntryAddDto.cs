using System.Globalization;

namespace Core.Model.DTO;

public class SleepEntryAddDto
{
    private const string DateTimeFormat = "d/M/yy, HH:mm";
    private const string OffsetFormat = "zzz";

    public required string DateString { get; set; }

    public required string StartString { get; set; }

    public required string UntilString { get; set; }

    public required string TimeZoneOffset { get; set; }

    public required decimal SleepHours { get; set; }

    public decimal? RechargePercent { get; set; }
    public decimal? CreditPercent { get; set; }
    public decimal? DebtPercent { get; set; }
    public decimal? SleepPercent { get; set; }

    public decimal? RemHours { get; set; }
    public decimal? RemPercent { get; set; }
    public decimal? DeepHours { get; set; }
    public decimal? DeepPercent { get; set; }

    public int? Bpm { get; set; }
    public decimal? BpmPercent { get; set; }
    public decimal? SleepRating { get; set; }

    public DateOnly Date => ParseDate(DateString);

    public DateTimeOffset Start => ParseDateTimeOffset(StartString);

    public DateTimeOffset Until => ParseDateTimeOffset(UntilString);

    private TimeSpan Offset => DateTimeOffset
        .ParseExact(TimeZoneOffset, OffsetFormat, CultureInfo.InvariantCulture)
        .Offset;

    private DateOnly ParseDate(string text)
    {
        var local = DateTime.ParseExact(
            StripWeekday(text),
            DateTimeFormat,
            CultureInfo.InvariantCulture);

        return DateOnly.FromDateTime(local);
    }

    private DateTimeOffset ParseDateTimeOffset(string text)
    {
        var local = DateTime.ParseExact(
            StripWeekday(text),
            DateTimeFormat,
            CultureInfo.InvariantCulture);

        return new DateTimeOffset(local, Offset);
    }

    private static string StripWeekday(string text)
    {
        var space = text.IndexOf(' ');
        return space < 0 ? text : text[(space + 1)..];
    }
}
