using System.Text.Json.Serialization;
using NCoreUtils.OpenTelemetry.Json;

namespace NCoreUtils.OpenTelemetry;

[method: JsonConstructor]
public sealed class TraceSpan(
    string name,
    string spanId,
    string? parentSpanId,
    TruncatableString displayName,
    DateTimeOffset startTime,
    DateTimeOffset endTime,
    TraceSpanAttributes? attributes = default,
    // FIXME: stackTrace
    // FIXME: timeEvents
    // FIXME: links
    TraceSpanStatus? status = default,
    bool? sameProcessAsParentSpan = default,
    int? childSpanCount = default,
    TraceSpanKind spanKind = default)
{
    [JsonPropertyName("name")]
    public string Name { get; } = name;

    [JsonPropertyName("spanId")]
    public string SpanId { get; } = spanId;

    [JsonPropertyName("parentSpanId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentSpanId { get; } = parentSpanId;

    [JsonPropertyName("displayName")]
    public TruncatableString DisplayName { get; } = displayName;

    [JsonPropertyName("startTime")]
    [JsonConverter(typeof(GoogleTraceSpanDateTimeOffsetConverter))]
    public DateTimeOffset StartTime { get; } = startTime;

    [JsonPropertyName("endTime")]
    [JsonConverter(typeof(GoogleTraceSpanDateTimeOffsetConverter))]
    public DateTimeOffset EndTime { get; } = endTime;

    [JsonPropertyName("attributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TraceSpanAttributes? Attributes { get; } = attributes;

    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TraceSpanStatus? Status { get; } = status;

    [JsonPropertyName("sameProcessAsParentSpan")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SameProcessAsParentSpan { get; } = sameProcessAsParentSpan;

    [JsonPropertyName("childSpanCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ChildSpanCount { get; } = childSpanCount;

    [JsonPropertyName("spanKind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TraceSpanKind SpanKind { get; } = spanKind;

    public TraceSpan(
        string name,
        string spanId,
        string? parentSpanId,
        string displayName,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        TraceSpanAttributes? attributes = default,
        TraceSpanStatus? status = default,
        bool? sameProcessAsParentSpan = default,
        int? childSpanCount = default,
        TraceSpanKind spanKind = default)
        : this(
            name,
            spanId,
            parentSpanId,
            TruncatableString.Create(displayName, 128),
            startTime,
            endTime,
            attributes,
            status,
            sameProcessAsParentSpan,
            childSpanCount,
            spanKind)
    { }

    public TraceSpan(
        string name,
        string spanId,
        string displayName,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        TraceSpanAttributes? attributes = default,
        TraceSpanStatus? status = default,
        bool? sameProcessAsParentSpan = default,
        int? childSpanCount = default,
        TraceSpanKind spanKind = default)
        : this(
            name,
            spanId,
            default,
            displayName,
            startTime,
            endTime,
            attributes,
            status,
            sameProcessAsParentSpan,
            childSpanCount,
            spanKind)
    { }
}