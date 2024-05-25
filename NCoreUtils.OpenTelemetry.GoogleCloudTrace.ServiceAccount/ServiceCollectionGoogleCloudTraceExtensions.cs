using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreUtils.Google;
using NCoreUtils.OpenTelemetry;

namespace NCoreUtils;

public static class ServiceCollectionGoogleCloudTraceExtensions
{
    public const string DefaultGoogleCloudTracesEndpoint = "https://cloudtrace.googleapis.com";

    public static IServiceCollection AddGoogleCloudTraceExporter(
        this IServiceCollection services,
        ServiceAccountCredentialData credentials,
        string projectId,
        string? endpoint = default,
        bool configureHttpClient = true)
    {
        if (configureHttpClient)
        {
            services.TryAddTransient<InjectGoogleAccessTokenHandler>();
            services.AddHttpClient(GoogleCloudTraceV2Client.HttpClientConfigurationName)
                .AddHttpMessageHandler<InjectGoogleAccessTokenHandler>();
        }
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddGoogleCloudTraceExporter(projectId));

        return services
            .AddGoogleCloudServiceAccount(credentials)
            .AddGoogleCloudTraceV2Client(endpoint ?? DefaultGoogleCloudTracesEndpoint, GoogleCloudTraceV2Client.HttpClientConfigurationName);
    }

    public static IServiceCollection AddGoogleCloudTraceExporter(
        this IServiceCollection services,
        string projectId,
        string? endpoint = default,
        bool configureHttpClient = true)
        => services.AddGoogleCloudTraceExporter(
            projectId: projectId,
            credentials: ServiceAccountCredentialData.ReadDefaultAsync(CancellationToken.None).GetAwaiter().GetResult(),
            endpoint: endpoint,
            configureHttpClient: configureHttpClient
        );
}

