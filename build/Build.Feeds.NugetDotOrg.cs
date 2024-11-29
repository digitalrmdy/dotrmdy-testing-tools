using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
	[Secret, Parameter] string NUGETAPIKEY;
	[Secret, Parameter] string NUGET_SOURCE = "https://api.nuget.org/v3/index.json";
	
	Target PublishToNugetOrg => _ => _
		.DependsOn(Pack)
		.Executes(() =>
		{
			IEnumerable<AbsolutePath> artifactPackages = ArtifactsDirectory.GlobFiles("*.nupkg");
			DotNetNuGetPush(s => s
				.SetSource(NUGET_SOURCE)
				.SetApiKey(NUGETAPIKEY) 
				.EnableSkipDuplicate()
				.CombineWith(artifactPackages, (_, v) => _
					.SetTargetPath(v)));		});
}