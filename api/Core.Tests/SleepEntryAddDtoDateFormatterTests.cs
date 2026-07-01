using System.Globalization;
using Core.Model.DTO;

namespace Core.Tests;

public class SleepEntryAddDtoDateFormatterTests
{
    public static IEnumerable<object[]> DateOnlyCases =>
    [
        ["Wed 1/7/26, 23:55", new DateOnly(2026, 7, 1)],
        ["Tue 30/6/26, 23:55", new DateOnly(2026, 6, 30)],
        ["Wed 1/7/26, 00:05", new DateOnly(2026, 7, 1)],
        ["Mon 31/12/26, 23:59", new DateOnly(2026, 12, 31)],
        ["Fri 1/1/26, 12:00", new DateOnly(2026, 1, 1)],
    ];

    [Theory]
    [MemberData(nameof(DateOnlyCases))]
    public void ParseDate_returns_date_only_from_text(string text, DateOnly expected)
    {
        var dto = CreateDto();

        var result = dto.ParseDate(text);

        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> DateTimeOffsetCases =>
    [
        ["Wed 1/7/26, 23:55", "+0200", new DateTimeOffset(2026, 7, 1, 23, 55, 0, TimeSpan.FromHours(2))],
        ["Tue 30/6/26, 23:55", "+0200", new DateTimeOffset(2026, 6, 30, 23, 55, 0, TimeSpan.FromHours(2))],
        ["Wed 1/7/26, 23:55", "+0000", new DateTimeOffset(2026, 7, 1, 23, 55, 0, TimeSpan.Zero)],
        ["Wed 1/7/26, 23:55", "-0500", new DateTimeOffset(2026, 7, 1, 23, 55, 0, TimeSpan.FromHours(-5))],
        ["Mon 31/12/26, 00:30", "+0530", new DateTimeOffset(2026, 12, 31, 0, 30, 0, TimeSpan.FromMinutes(5 * 60 + 30))],
    ];

    [Theory]
    [MemberData(nameof(DateTimeOffsetCases))]
    public void ParseDateTimeOffset_returns_offset_aware_moment(string text, string offset, DateTimeOffset expected)
    {
        var dto = CreateDto(offset);

        var result = dto.ParseDateTimeOffset(text);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Wed 1/7/26, 23:55", "+0200", "2026-07-01T21:55:00.0000000Z")]
    [InlineData("Tue 30/6/26, 23:55", "+0200", "2026-06-30T21:55:00.0000000Z")]
    [InlineData("Wed 1/7/26, 23:55", "+0000", "2026-07-01T23:55:00.0000000Z")]
    [InlineData("Wed 1/7/26, 23:55", "-0500", "2026-07-02T04:55:00.0000000Z")]
    public void ParseDateTimeOffset_normalizes_utc_correctly(string text, string offset, string expectedUtc)
    {
        var dto = CreateDto(offset);

        var result = dto.ParseDateTimeOffset(text);

        Assert.Equal(DateTimeOffset.Parse(expectedUtc, CultureInfo.InvariantCulture), result.UtcDateTime);
    }

    [Theory]
    [InlineData("Wed 1/7/26, 23:55")]
    public void ParseDate_strips_weekday_prefix(string text)
    {
        var dto = CreateDto();

        var result = dto.ParseDate(text);

        Assert.Equal(new DateOnly(2026, 7, 1), result);
    }

    private static SleepEntryAddDto CreateDto(string offset = "+0000") =>
        new()
        {
            DateString = "1/7/26, 00:00",
            StartString = "1/7/26, 00:00",
            UntilString = "1/7/26, 00:00",
            TimeZoneOffset = offset,
            SleepHours = 0
        };
}
