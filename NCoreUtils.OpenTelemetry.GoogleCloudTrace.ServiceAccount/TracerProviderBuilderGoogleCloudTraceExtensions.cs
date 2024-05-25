using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCoreUtils.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace NCoreUtils;

public static class TracerProviderBuilderGoogleCloudTraceExtensions
{
    public static TracerProviderBuilder AddGoogleCloudTraceExporter(this TracerProviderBuilder builder, string projectId)
    {
        builder.AddProcessor(serviceProvider => new BatchActivityExportProcessor(new GoogleCloudTraceExporter(
            projectId: projectId,
            logger: serviceProvider.GetRequiredService<ILogger<GoogleCloudTraceExporter>>(),
            googleCloudTrace: serviceProvider.GetRequiredService<IGoogleCloudTraceV2>()
        )));
        return builder;
    }
}