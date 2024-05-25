using System.Text.Json;

namespace NCoreUtils.OpenTelemetry.Json;

internal static class Utf8JsonReaderExtensions
{
    public static void Expect(this in Utf8JsonReader reader, JsonTokenType expected)
    {
        if (reader.TokenType != expected)
        {
            throw new JsonException($"Expected {expected} found {reader.TokenType}.");
        }
    }

    public static void ReadOrThrow(this ref Utf8JsonReader reader)
    {
        if (!reader.Read())
        {
            throw new JsonException("Unexpected end og the JSON stream.");
        }
    }
}