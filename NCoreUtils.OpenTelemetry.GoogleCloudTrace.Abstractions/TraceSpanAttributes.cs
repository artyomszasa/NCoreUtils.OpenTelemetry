using System.Text.Json.Serialization;

namespace NCoreUtils.OpenTelemetry;

[method: JsonConstructor]
public readonly struct TraceSpanAttributes(
    IReadOnlyDictionary<string, TraceSpanAttributeValue> attributeMap,
    int droppedAttributesCount = 0)
{
    [JsonPropertyName("attributeMap")]
    public IReadOnlyDictionary<string, TraceSpanAttributeValue> AttributeMap { get; } = attributeMap;

    [JsonPropertyName("droppedAttributesCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int DroppedAttributesCount { get; } = droppedAttributesCount;
}