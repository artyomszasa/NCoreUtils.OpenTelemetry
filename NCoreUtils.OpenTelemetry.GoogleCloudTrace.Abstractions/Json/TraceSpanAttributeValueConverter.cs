using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NCoreUtils.OpenTelemetry.Json;

public sealed class TraceSpanAttributeValueConverter : JsonConverter<TraceSpanAttributeValue>
{
    public override TraceSpanAttributeValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }
        reader.Expect(JsonTokenType.StartObject);
        reader.ReadOrThrow();
        TraceSpanAttributeValue result;
        if (reader.ValueTextEquals("stringValue"u8))
        {
            reader.ReadOrThrow();
            var typeInfo = options.GetTypeInfo(typeof(TruncatableString)) as JsonTypeInfo<TruncatableString>
                ?? throw new InvalidOperationException("No JSON type info found for TruncatableString.");
            result = new(JsonSerializer.Deserialize(ref reader, typeInfo));
            reader.ReadOrThrow();
        }
        else if (reader.ValueTextEquals("intValue"u8))
        {
            reader.ReadOrThrow();
            result = new(reader.GetInt64());
            reader.ReadOrThrow();
        }
        else if (reader.ValueTextEquals("boolValue"u8))
        {
            reader.ReadOrThrow();
            result = new(reader.GetBoolean());
            reader.ReadOrThrow();
        }
        else
        {
            throw new JsonException($"Invalid property: \"{reader.GetString()}\".");
        }
        reader.Expect(JsonTokenType.EndObject);
        return result;
    }

    public override void Write(Utf8JsonWriter writer, TraceSpanAttributeValue value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case { IsString: true, StringValue: var v }:
                var typeInfo = options.GetTypeInfo(typeof(TruncatableString)) as JsonTypeInfo<TruncatableString>
                    ?? throw new InvalidOperationException("No JSON type info found for TruncatableString.");
                writer.WriteStartObject();
                writer.WritePropertyName("stringValue"u8);
                JsonSerializer.Serialize(writer, v, typeInfo);
                writer.WriteEndObject();
                break;
            case { IsInteger: true, IntValue: var v }:
                writer.WriteStartObject();
                writer.WriteNumber("intValue"u8, v);
                writer.WriteEndObject();
                break;
            case { IsBoolean: true, BoolValue: var v }:
                writer.WriteStartObject();
                writer.WriteBoolean("boolValue"u8, v);
                writer.WriteEndObject();
                break;
            default:
                writer.WriteNullValue();
                break;
        }
    }
}