namespace NCoreUtils.OpenTelemetry;

public enum TraceSpanKind
{
    SPAN_KIND_UNSPECIFIED = -1,
    INTERNAL = 0,
    SERVER,
    CLIENT,
    PRODUCER,
    CONSUMER
}
