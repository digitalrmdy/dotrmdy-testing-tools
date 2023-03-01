using Nuke.Common.CI.AzurePipelines;

[AzurePipelines(
    suffix: "PR",
    AzurePipelinesImage.UbuntuLatest,
    AutoGenerate = false,
    FetchDepth = 0,
    TriggerBatch = true,
    PullRequestsBranchesInclude = new[] { "main" },
    InvokedTargets = new[] { nameof(Pack) },
    CacheKeyFiles = new string[0],
    CachePaths = new string[0])]
partial class Build
{
}