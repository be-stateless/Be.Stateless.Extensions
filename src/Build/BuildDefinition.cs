#region Copyright & License

// Copyright © 2012 - 2025 François Chabot
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.IO;
using System.Linq;
using System.Net.Mime;
using JetBrains.Annotations;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Octokit;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Serilog.Log;

namespace Build;

[GitHubActions(
	"ContinuousDelivery",
	GitHubActionsImage.UbuntuLatest,
	FetchDepth = 0,
	OnPushBranches = ["master", "feature/*"],
	InvokedTargets = [nameof(CI)],
	PublishArtifacts = true,
	EnableGitHubToken = true,
	ImportSecrets = [nameof(ReleaseFeedApiKey)],
	WritePermissions = [GitHubActionsPermissions.Contents, GitHubActionsPermissions.Packages])]
[DotNetVerbosityMapping]
file class BuildDefinition : NukeBuild
{
	public static int Main() => Execute<BuildDefinition>(static x => x.Build);

	[NotNull]
	Target Build => td => td.DependsOn(MutationTest);

	[NotNull]
	Target CI => td => td.DependsOn(Push, CreateGitHubRelease)
		.OnlyWhenStatic(() => IsServerBuild);

	[NotNull]
	Target Clean => td => td.Executes(() => {
		ArtifactsDirectory.CreateOrCleanDirectory();
		DotNetClean(s => s.EnableNoLogo()
			.SetConfiguration(Configuration)
			.SetProject(Solution)
			.SetVerbosity(DotNetVerbosity.minimal));
	});

	[NotNull]
	Target Restore => td => td.DependsOn(Clean)
		.Executes(() => {
			DotNetRestore(s => s.SetConfigFile(RootDirectory / "NuGet.config")
				.SetProjectFile(Solution));
			DotNetToolRestore(s => s.SetToolManifest(RootDirectory / ".config" / "dotnet-tools.json"));
		});

	[NotNull]
	Target Compile => td => td.DependsOn(Restore)
		.Executes(() => {
			DotNetBuild(s => s.EnableNoLogo()
				.EnableNoRestore()
				.SetConfiguration(Configuration)
				.SetProjectFile(Solution)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.SetVersion(GitVersion.SemVer));
		});

	[NotNull]
	Target UnitTest => td => td.Unlisted()
		.DependsOn(Compile)
		.Produces(TestCoverageReportsDirectory / "*.html")
		.Executes(() => {
			DotNetTest(s => s.EnableNoLogo()
				.EnableNoBuild()
				.EnableNoRestore()
				.SetConfiguration(Configuration)
				.SetProjectFile(Solution)
				.SetResultsDirectory(TestResultsDirectory)
				.SetSettingsFile(RootDirectory / "coverlet.runsettings"));
			ReportGeneratorTasks.ReportGenerator(s => s.SetTargetDirectory(TestCoverageReportsDirectory)
				.AddReports(TestResultsDirectory / "**/coverage.cobertura.xml")
				.AddReportTypes(ReportTypes.lcov, ReportTypes.HtmlInline_AzurePipelines_Dark)
				.AddFileFilters("-*.g.cs"));
			string link = TestCoverageReportsDirectory / "index.html";
			Information($"Code coverage report: \e]8;;file://{link}\e\\{link}\e]8;;\e\\");
		});

	[NotNull]
	Target MutationTest => td => td.Unlisted()
		.DependsOn(UnitTest)
		.Produces(TestMutationReportsDirectory / "mutation-report.html")
		.Executes(() => {
			DotNet(workingDirectory: RootDirectory, arguments: $"stryker --output {ArtifactsDirectory} --solution ./{Solution.FileName}");
			string link = TestMutationReportsDirectory / "mutation-report.html";
			Information($"Mutation test report: \e]8;;file://{link}\e\\{link}\e]8;;\e\\");
		});

	[NotNull]
	Target Pack => td => td.DependsOn(Compile)
		.Before(UnitTest) // prevent test instrumentation contaminating packages
		.Produces(NuGetPackagesDirectory / "*.nupkg")
		.Executes(() => {
			DotNetPack(s => s.EnableNoLogo()
				.EnableNoBuild()
				.EnableNoRestore()
				.EnableContinuousIntegrationBuild()
				.SetConfiguration(Configuration)
				.SetProject(Solution)
				.SetNoDependencies(v: true)
				.SetOutputDirectory(NuGetPackagesDirectory)
				.SetVersion(GitVersion.SemVer));
		});

	[NotNull]
	Target PreviewFeedSetup => td => td.Unlisted()
		.Description("Set PushApiUrl/PushApiKey for preview NuGet package feed when on feature branch.")
		.OnlyWhenStatic(() => GitRepository.IsOnFeatureBranch())
		.Requires(() => PreviewFeedUrl)
		.Executes(() => {
			NuGetFeedApiKey = GitHubActions.Instance.Token;
			NuGetFeedUrl = PreviewFeedUrl;
		});

	[NotNull]
	Target ReleaseFeedSetup => td => td.Unlisted()
		.Description("Set PushApiUrl/PushApiKey for release NuGet package feed when on feature branch.")
		.OnlyWhenStatic(() => GitRepository.IsOnMasterBranch())
		.Requires(() => Configuration.Equals(Configuration.Release))
		.Requires(() => ReleaseFeedApiKey)
		.Requires(() => ReleaseFeedUrl)
		.Executes(() => {
			NuGetFeedApiKey = ReleaseFeedApiKey;
			NuGetFeedUrl = ReleaseFeedUrl;
		});

	[NotNull]
	Target Push => td => td.DependsOn(MutationTest, Pack, PreviewFeedSetup, ReleaseFeedSetup)
		.OnlyWhenStatic(() => GitRepository.IsOnMasterBranch() || GitRepository.IsOnFeatureBranch())
		.Consumes(Pack)
		.Executes(() => {
			NuGetFeedApiKey.NotNullOrEmpty();
			NuGetFeedUrl.NotNullOrEmpty();
			NuGetPackagesDirectory.GlobFiles("*.nupkg")
				.ForEach(filepath => {
					Information($"Pushing NuGet package {filepath}");
					DotNetNuGetPush(s => s.SetSkipDuplicate(GitRepository.IsOnFeatureBranch())
						.SetApiKey(NuGetFeedApiKey)
						.SetSource(NuGetFeedUrl)
						.SetTargetPath(filepath));
				});
		});

	[NotNull]
	Target CreateGitHubRelease => td => td.DependsOn(Push)
		.OnlyWhenStatic(() => GitRepository.IsOnMasterBranch() || GitRepository.IsOnFeatureBranch())
		.Executes(async () => {
			GitHubTasks.GitHubClient.Credentials = new Credentials(GitHubActions.Instance.Token);
			var release = await GitHubTasks.GitHubClient.Repository.Release.Create(
				GitRepository.GetGitHubOwner(),
				GitRepository.GetGitHubName(),
				new NewRelease($"v{GitVersion.SemVer}") {
					Name = GitVersion.SemVer,
					Prerelease = GitRepository.IsOnFeatureBranch(),
					Draft = false
				});
			NuGetPackagesDirectory.GlobFiles("*.nupkg", "*.?nupkg")
				.Select(async filepath => {
					await using var assetStream = File.OpenRead(filepath);
					var asset = new ReleaseAssetUpload {
						FileName = filepath.Name,
						ContentType = MediaTypeNames.Application.Octet,
						RawData = assetStream
					};
					await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, asset);
				})
				.WaitAll();
		});

	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

	AbsolutePath NuGetPackagesDirectory => ArtifactsDirectory / "nuget-packages";

	AbsolutePath TestCoverageReportsDirectory => ArtifactsDirectory / "test-coverage-reports";

	AbsolutePath TestMutationReportsDirectory => ArtifactsDirectory / "reports";

	AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";

	[Parameter("Configuration to build: 'Debug' or 'Release'.")]
	readonly Configuration Configuration = IsLocalBuild
		? Configuration.Debug
		: Configuration.Release;

	[Required]
	[GitRepository]
	readonly GitRepository GitRepository;

	[Required]
	[GitVersion]
	readonly GitVersion GitVersion = null!;

	[Parameter("NuGet packages' preview feed URL.")]
	readonly string PreviewFeedUrl = "https://nuget.pkg.github.com/be-stateless/index.json";

	[Parameter("NuGet packages' release feed API Key.")]
	[Secret]
	readonly string ReleaseFeedApiKey;

	[Parameter("NuGet packages' release feed URL.")]
	readonly string ReleaseFeedUrl = "https://api.nuget.org/v3/index.json";

	[Secret]
	string NuGetFeedApiKey;

	string NuGetFeedUrl;

	[Required]
	[Solution]
	[NotNull]
	readonly Solution Solution = null!;
}
