using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.JsonConverters;

public class FlexibleDecimalConverter : JsonConverter<decimal>
{
    private const NumberStyles ParseStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDecimal(),
            JsonTokenType.String => Parse(reader.GetString()!),
            _ => throw new JsonException(
                $"Unexpected token {reader.TokenType} when parsing decimal.")
        };
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value);

    private static decimal Parse(string text)
    {
        try
        {
            var trimmed = text.Trim();
            var lastDot = trimmed.LastIndexOf('.');
            var lastComma = trimmed.LastIndexOf(',');
            var separatorIndex = Math.Max(lastDot, lastComma);

            if (separatorIndex < 0)
            {
                return decimal.Parse(trimmed, ParseStyles, CultureInfo.InvariantCulture);
            }

            var head = trimmed[..separatorIndex].Replace(".", "").Replace(",", "");
            var tail = trimmed[(separatorIndex + 1)..];

            return decimal.Parse($"{head}.{tail}", ParseStyles, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            throw new JsonException($"Cannot parse '{text}' as a decimal.");
        }
    }
}
