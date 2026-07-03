using System.Text.Json;
using Core.JsonConverters;

namespace Core.Tests;

public class FlexibleDecimalConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new FlexibleDecimalConverter() }
    };

    public static IEnumerable<object[]> ReadNumberCases =>
    [
        ["{\"V\":12.53}", 12.53m],
        ["{\"V\":0}", 0m],
        ["{\"V\":-0.5}", -0.5m],
        ["{\"V\":100}", 100m],
    ];

    [Theory]
    [MemberData(nameof(ReadNumberCases))]
    public void Read_accepts_json_number(string json, decimal expected)
    {
        var result = JsonSerializer.Deserialize<Holder>(json, Options);

        Assert.Equal(expected, result!.V);
    }

    public static IEnumerable<object[]> ReadStringDotCases =>
    [
        ["{\"V\":\"12.53\"}", 12.53m],
        ["{\"V\":\"0.0\"}", 0m],
        ["{\"V\":\"7.5\"}", 7.5m],
        ["{\"V\":\"1234567.89\"}", 1234567.89m],
        ["{\"V\":\"-0.5\"}", -0.5m],
        ["{\"V\":\"12\"}", 12m],
        ["{\"V\":\"1.300\"}", 1.3m],
    ];

    [Theory]
    [MemberData(nameof(ReadStringDotCases))]
    public void Read_accepts_quoted_string_with_dot_decimal(string json, decimal expected)
    {
        var result = JsonSerializer.Deserialize<Holder>(json, Options);

        Assert.Equal(expected, result!.V);
    }

    public static IEnumerable<object[]> ReadStringCommaCases =>
    [
        ["{\"V\":\"0,0\"}", 0m],
        ["{\"V\":\"7,5\"}", 7.5m],
        ["{\"V\":\"12,53\"}", 12.53m],
        ["{\"V\":\"1234567,89\"}", 1234567.89m],
        ["{\"V\":\"-0,5\"}", -0.5m],
        ["{\"V\":\"3,4500\"}", 3.45m],
        ["{\"V\":\"1,300\"}", 1.3m],
    ];

    [Theory]
    [MemberData(nameof(ReadStringCommaCases))]
    public void Read_accepts_quoted_string_with_comma_decimal(string json, decimal expected)
    {
        var result = JsonSerializer.Deserialize<Holder>(json, Options);

        Assert.Equal(expected, result!.V);
    }

    public static IEnumerable<object[]> ReadThousandsCases =>
    [
        ["{\"V\":\"1.234.567,89\"}", 1234567.89m],
        ["{\"V\":\"1,234,567.89\"}", 1234567.89m],
    ];

    [Theory]
    [MemberData(nameof(ReadThousandsCases))]
    public void Read_treats_rightmost_separator_as_decimal(string json, decimal expected)
    {
        var result = JsonSerializer.Deserialize<Holder>(json, Options);

        Assert.Equal(expected, result!.V);
    }

    public static IEnumerable<object[]> WriteCases =>
    [
        [12.53m, "12.53"],
        [0m, "0"],
        [-0.5m, "-0.5"],
        [1234567.89m, "1234567.89"],
    ];

    [Theory]
    [MemberData(nameof(WriteCases))]
    public void Write_emits_json_number(decimal value, string expectedNumber)
    {
        var json = JsonSerializer.Serialize(new Holder { V = value }, Options);

        Assert.Equal($"{{\"V\":{expectedNumber}}}", json);
    }

    [Theory]
    [InlineData("{\"V\":true}")]
    [InlineData("{\"V\":\"not-a-number\"}")]
    public void Read_rejects_unsupported_tokens(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Holder>(json, Options));
    }

    private class Holder
    {
        public decimal V { get; set; }
    }
}
