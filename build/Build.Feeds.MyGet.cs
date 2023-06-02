using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
	const string RMDYMyGetName = "RMDY-MyGet";

	[Secret, Parameter]
	readonly string MyGetApiKey;

	Target PublishToMyGet => _ => _
		.DependsOn(Pack)
		.Requires(() => !string.IsNullOrEmpty(MyGetApiKey))
		.Executes(() =>
		{
			IEnumerable<AbsolutePath> artifactPackages = ArtifactsDirectory.GlobFiles("*.nupkg");

			DotNetNuGetPush(s => s
				.SetSource(RMDYMyGetName)
				.SetApiKey(MyGetApiKey)
				.EnableSkipDuplicate()
				.CombineWith(artifactPackages, (_, v) => _
					.SetTargetPath(v)));
		});
}