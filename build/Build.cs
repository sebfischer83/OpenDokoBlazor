#nullable enable
using System.IO;
using System.Linq;
using Colorful;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using Octokit;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.GitHub.GitHubTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter()]
    readonly string? GitHubToken;

    [Solution] readonly Solution? Solution;
    [GitRepository] readonly GitRepository? GitRepository;
    [GitVersion] readonly GitVersion? GitVersion;
    [CI] readonly GitHubActions? GitHubActions;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    AbsolutePath PublishDirectory => ArtifactsDirectory / "publish";

    AbsolutePath TestResultDirectory => ArtifactsDirectory / "tests";
    AbsolutePath CoverageDirectory => ArtifactsDirectory / "coverage";

    string ChangelogFile => RootDirectory / "CHANGELOG.md";
    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release";
    const string HotfixBranchPrefix = "hotfix";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                .SetFileVersion(GitVersion?.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion?.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution?.GetProject("OpenDokoBlazor.Shared.Tests"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .ResetVerbosity()
                .EnableUseSourceLink()
                .SetResultsDirectory(TestResultDirectory)
                .SetLogger("trx;LogFileName=OpenDokoBlazor.Shared.Tests.trx")
                .EnableCollectCoverage()
                .SetCoverletOutput(TestResultDirectory / "OpenDokoBlazor.Shared.Tests.xml")
                .SetCoverletOutputFormat(CoverletOutputFormat.cobertura));

            ReportGenerator(_ => _
                .SetReports(TestResultDirectory / "*.xml")
                .SetReportTypes(ReportTypes.HtmlInline, ReportTypes.CsvSummary)
                .SetTargetDirectory(CoverageDirectory)
                .SetFramework("netcoreapp2.1"));
            
            
        });

    Target Publish => _ => _
        .DependsOn(Compile, Test)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(Solution?.GetProject("OpenDokoBlazor.Server"))
                .SetConfiguration(Configuration.Release)
                .SetAssemblyVersion(GitVersion?.AssemblySemVer)
                .SetFileVersion(GitVersion?.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion?.InformationalVersion)
                .SetSelfContained(false)
                .SetPublishSingleFile(false)
                .SetOutput(PublishDirectory)
                .EnableNoRestore());
        });

    Target Release => _ => _
        .DependsOn(Test, Compile)
        .OnlyWhenDynamic(() => GitHubActions != null)
        .Executes(async () =>
        {
            FinalizeChangelog(ChangelogFile, GitVersion?.MajorMinorPatch, GitRepository);
            Git($"add {ChangelogFile}");
            Git($"commit -m \"Finalize {Path.GetFileName(ChangelogFile)} for {GitVersion?.MajorMinorPatch}\"");
            string releaseNotes = GetNuGetReleaseNotes(ChangelogFile, GitRepository);
            
            var client = new GitHubClient(new ProductHeaderValue("OpenDokoBlazor"));
            var tokenAuth = new Credentials(GitHubToken);
            client.Credentials = tokenAuth;
            var newRelease = new NewRelease(GitVersion?.MajorMinorPatch);
            newRelease.Name = GitVersion?.MajorMinorPatch;
            newRelease.Body = releaseNotes;
            newRelease.Draft = false;
            newRelease.Prerelease = false;

            string zipFileName = ArtifactsDirectory / $"{GitVersion?.MajorMinorPatch}.zip";
            string coverageFileName = ArtifactsDirectory / "coverage.zip";
            CompressionTasks.CompressZip(PublishDirectory, zipFileName);
            CompressionTasks.CompressZip(CoverageDirectory, coverageFileName);
            var result = await client.Repository.Release.Create("sebfischer83", "OpenDokoBlazor", newRelease);
            await using var archiveContents = File.OpenRead(zipFileName);
            var assetUpload = new ReleaseAssetUpload()
            {
                FileName = $"{GitVersion?.MajorMinorPatch}.zip",
                ContentType = "application/zip",
                RawData = archiveContents
            };
            await using var covarageContent = File.OpenRead(zipFileName);
            var coverageAssetUpload = new ReleaseAssetUpload()
            {
                FileName = "coverage.zip",
                ContentType = "application/zip",
                RawData = covarageContent
            };
            await client.Repository.Release.UploadAsset(result, coverageAssetUpload);
        });

    public static bool GitHasCleanWorkingCopy()
    {
        return GitHasCleanWorkingCopy(null);
    }

    public static bool GitHasCleanWorkingCopy(string? workingDirectory)
    {
        return !Git("status --short", workingDirectory, logOutput: false).Any();
    }
}
