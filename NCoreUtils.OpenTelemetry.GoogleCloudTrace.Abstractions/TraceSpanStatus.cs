using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.OpenTelemetry;

[method: JsonConstructor]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct TraceSpanStatus(
    TraceSpanStatusCode code,
    string? message
    /* FIXME: details */)
{
    [JsonPropertyName("code")]
    public TraceSpanStatusCode Code { get; } = code;

    [JsonPropertyName("message")]
    public string? Message { get; } = message;
}
