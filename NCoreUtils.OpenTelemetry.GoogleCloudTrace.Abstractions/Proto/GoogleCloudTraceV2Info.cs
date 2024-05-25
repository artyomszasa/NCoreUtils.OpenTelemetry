using NCoreUtils.Proto;

namespace NCoreUtils.OpenTelemetry.Proto;

[ProtoInfo(typeof(IGoogleCloudTraceV2), Path = "v2")]
[ProtoMethodInfo(nameof(IGoogleCloudTraceV2.BatchWriteAsync), Input = InputType.Custom, Path = "projects")]
public partial class GoogleCloudTraceV2Info { }