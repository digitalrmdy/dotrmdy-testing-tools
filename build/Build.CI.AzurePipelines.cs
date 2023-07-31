using Nuke.Common.CI.AzurePipelines;

[AzurePipelines(
    suffix: "PR",
    AzurePipelinesImage.UbuntuLatest,
    AutoGenerate = false,
    FetchDepth = 0,
    TriggerBatch = true,
    InvokedTargets = new[] { nameof(Pack) },
    CacheKeyFiles = new string[0],
    CachePaths = new string[0])]
[AzurePipelines(
    suffix: "Publish",
    AzurePipelinesImage.UbuntuLatest,
    EnableAccessToken = true,
    AutoGenerate = false,
    FetchDepth = 0,
    TriggerBatch = true,
    TriggerTagsInclude = new[] { "'*.*.*'" },
    InvokedTargets = new[] { nameof(Publish) },
    CacheKeyFiles = new string[0],
    CachePaths = new string[0])]
partial class Build
{
}