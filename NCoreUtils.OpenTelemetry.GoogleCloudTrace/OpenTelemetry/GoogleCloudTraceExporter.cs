using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NCoreUtils.Collections;
using OpenTelemetry;

namespace NCoreUtils.OpenTelemetry;

public class GoogleCloudTraceExporter(
    string projectId,
    ILogger<GoogleCloudTraceExporter> logger,
    IGoogleCloudTraceV2 googleCloudTrace)
    : BaseExporter<Activity>
{
    private ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    private string ProjectId { get; } = projectId switch
    {
        null or "" => throw new ArgumentException("Invalid project id.", nameof(projectId)),
        var v => v
    };

    private IGoogleCloudTraceV2 GoogleCloudTrace { get; } = googleCloudTrace ?? throw new ArgumentNullException(nameof(googleCloudTrace));

    private static void FetchAndConvertAttributes(Activity activity, ref Dictionary<string, TraceSpanAttributeValue>? values)
    {
        foreach (var kv in activity.Tags)
        {
            switch (kv.Key)
            {
                // case "url.path":
                //     if (kv.Value is not null)
                //     {
                //         (values ??= []).Add("/http/url", kv.Value);
                //     }
                //     break;
                // case "http.request.method":
                //     if (kv.Value is not null)
                //     {
                //         (values ??= []).Add("/http/method", kv.Value);
                //     }
                //     break;
                default:
                    (values ??= []).Add(kv.Key, kv.Value ?? string.Empty);
                    break;
            }
        }
    }

    public override ExportResult Export(in Batch<Activity> batch)
    {
        using var scope = SuppressInstrumentationScope.Begin();
        try
        {
            using var spans = new ArrayPoolList<TraceSpan>((int)batch.Count);
            foreach (var activity in batch)
            {
                Dictionary<string, TraceSpanAttributeValue>? attributes = default;
                FetchAndConvertAttributes(activity, ref attributes);
                var spanId = activity.SpanId.ToHexString();
                var span = new TraceSpan(
                    name: $"projects/{projectId}/traces/{activity.TraceId.ToHexString()}/spans/{spanId}",
                    spanId: spanId,
                    parentSpanId: activity.ParentSpanId.Equals(default)
                        ? default
                        : activity.ParentSpanId.ToHexString(),
                    displayName: activity.DisplayName,
                    startTime: new(activity.StartTimeUtc, TimeSpan.Zero),
                    endTime: new(activity.StartTimeUtc + activity.Duration, TimeSpan.Zero),
                    attributes: attributes is null
                        ? default
                        : new TraceSpanAttributes(attributes),
                    status: new TraceSpanStatus(activity.Status switch
                    {
                        ActivityStatusCode.Ok => TraceSpanStatusCode.OK,
                        _ => TraceSpanStatusCode.UNKNOWN
                    }, activity.StatusDescription),
                    spanKind: activity.Kind switch
                    {
                        ActivityKind.Internal => TraceSpanKind.INTERNAL,
                        ActivityKind.Server => TraceSpanKind.SERVER,
                        ActivityKind.Client => TraceSpanKind.CLIENT,
                        ActivityKind.Producer => TraceSpanKind.PRODUCER,
                        ActivityKind.Consumer => TraceSpanKind.CONSUMER,
                        _ => TraceSpanKind.SPAN_KIND_UNSPECIFIED
                    }
                );
                spans.Add(span);
            }
            // TODO: find a better way
            GoogleCloudTrace.BatchWriteAsync(ProjectId, spans, CancellationToken.None).GetAwaiter().GetResult();
            return ExportResult.Success;
        }
        catch (Exception exn)
        {
            Logger.LogError(exn, "Failed to batch write traces.");
            return ExportResult.Failure;
        }
    }
}