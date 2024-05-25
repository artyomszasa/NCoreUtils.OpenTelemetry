namespace NCoreUtils.OpenTelemetry;

public interface IGoogleCloudTraceV2
{
    public Task BatchWriteAsync(string projectId, IReadOnlyList<TraceSpan> spans, CancellationToken cancellationToken = default);
}