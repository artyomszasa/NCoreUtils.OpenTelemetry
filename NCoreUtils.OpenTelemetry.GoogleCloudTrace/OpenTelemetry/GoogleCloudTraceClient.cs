using System.Text.Json;
using System.Text.Json.Serialization;
using NCoreUtils.Google;
using NCoreUtils.OpenTelemetry.Proto;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;

namespace NCoreUtils.OpenTelemetry;

public class BatchWriteRequest(IReadOnlyList<TraceSpan> spans)
{
    [JsonPropertyName("spans")]
    public IReadOnlyList<TraceSpan> Spans { get; } = spans;
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootGoogleCloudTraceV2Info))]
[JsonSerializable(typeof(BatchWriteRequest))]
[JsonSerializable(typeof(GoogleErrorResponse))]
internal partial class GoogleCloudTraceV2SerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(GoogleCloudTraceV2Info), typeof(GoogleCloudTraceV2SerializerContext))]
public partial class GoogleCloudTraceV2Client
{
    public const string HttpClientConfigurationName = nameof(GoogleCloudTraceV2Client);

    private HttpRequestMessage CreateBatchWriteRequest(string projectId, IReadOnlyList<TraceSpan> spans)
    {
        var pathBase = GetCachedMethodPath(Methods.BatchWrite);
        var path = $"{pathBase}/{projectId}/traces:batchWrite?alt=json";
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(new BatchWriteRequest(spans), GoogleCloudTraceV2SerializerContext.Default.BatchWriteRequest, default)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/trace.append");
        return request;
    }

    protected override async ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = response.Content is null
                ? null
                : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(responseContent))
            {
                throw new GoogleCloudException($"Server responded with status code {response.StatusCode} (no body).");
            }
            GoogleErrorResponse? gresponse;
            try
            {
                gresponse = JsonSerializer.Deserialize(responseContent, GoogleCloudTraceV2SerializerContext.Default.GoogleErrorResponse);
            }
            catch
            {
                throw new GoogleCloudException($"Server responded with status code {response.StatusCode} and unrecognized body: {responseContent}.");
            }
            throw new GoogleCloudException(gresponse?.Error);
        }
    }
}