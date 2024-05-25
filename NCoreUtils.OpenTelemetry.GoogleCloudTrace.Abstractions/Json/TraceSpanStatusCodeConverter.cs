using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.OpenTelemetry.Json;

public sealed class TraceSpanStatusCodeConverter : JsonConverter<TraceSpanStatusCode>
{
    public override TraceSpanStatusCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => (TraceSpanStatusCode)reader.GetInt32();

    public override void Write(Utf8JsonWriter writer, TraceSpanStatusCode value, JsonSerializerOptions options)
        => writer.WriteNumberValue((int)value);
}