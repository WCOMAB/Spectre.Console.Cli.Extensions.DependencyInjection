#module nuget:?package=Cake.BuildSystems.Module&version=7.1.0
#load "build/helpers.cake"
#load "build/version.cake"
#tool "nuget:https://api.nuget.org/v3/index.json?package=nuget.commandline&version=5.11.6"


///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var projects = GetProjects(File("./src/Spectre.Console.Cli.Extensions.DependencyInjection.sln"), configuration);
var artifacts = "./dist/";
var frameworks = new List<string> { "netstandard2.0", "net8.0", "net9.0" };
var packageVersion = string.Empty;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	CreateDirectory(artifacts);
	packageVersion = BuildVersion(fallbackVersion);
	if (FileExists("./build/.dotnet/dotnet.exe")) {
		Information("Using local install of `dotnet` SDK!");
		Context.Tools.RegisterFile("./build/.dotnet/dotnet.exe");
	}
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in projects.AllProjectPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/**/bin/" + configuration);
		CleanDirectories(path + "/**/obj/" + configuration);
	}
	Information("Cleaning common files...");
	CleanDirectory(artifacts);
});

Task("Restore")
	.Does(() =>
{
	// Restore all NuGet packages.
	Information("Restoring solution...");
	DotNetRestore(projects.SolutionPath);
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	foreach (var project in projects.SourceProjectPaths) {
		Information($"Building {project.GetDirectoryName()} for {configuration}");
		var settings = new DotNetBuildSettings {
			Configuration = configuration,
			ArgumentCustomization = args => args.Append("/p:NoWarn=NU1701"),
		};
		DotNetBuild(project.FullPath, settings);
	}

});

Task("NuGet")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Building NuGet package");
    CreateDirectory(artifacts + "package/");
    var packSettings = new DotNetPackSettings {
        Configuration = configuration,
        NoBuild = true,
        OutputDirectory = $"{artifacts}package",
        ArgumentCustomization = args => args
            .Append($"/p:Version=\"{packageVersion}\"")
            .Append("/p:NoWarn=\"NU1701 NU1602\"")
    };
    foreach(var project in projects.SourceProjectPaths) {
        Information($"Packing {project.GetDirectoryName()}...");
        DotNetPack(project.FullPath, packSettings);
    }
});

Task("Post-Build")
	.IsDependentOn("Build")
	.Does(() =>
{
	CreateDirectory(artifacts + "build");
	foreach (var project in projects.SourceProjects) {
		CreateDirectory(artifacts + "build/" + project.Name);
		foreach (var framework in frameworks) {
			var frameworkDir = $"{artifacts}build/{project.Name}/{framework}";
			CreateDirectory(frameworkDir);
			var files = GetFiles($"{project.Path.GetDirectory()}/bin/{configuration}/{framework}/*.*");
			CopyFiles(files, frameworkDir);
		}
	}
});

Task("Publish-NuGet-Package")
.IsDependentOn("NuGet")
.WithCriteria(() => HasEnvironmentVariable("NUGET_TOKEN"))
.WithCriteria(() => HasEnvironmentVariable("GITHUB_REF"))
.WithCriteria(() => EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v") || EnvironmentVariable("GITHUB_REF") == "refs/heads/main")
.Does(() => {
    var nugetToken = EnvironmentVariable("NUGET_TOKEN");
    var pkgFiles = GetFiles($"{artifacts}package/*.nupkg");
	Information($"Pushing {pkgFiles.Count} package files!");
    NuGetPush(pkgFiles, new NuGetPushSettings {
      Source = "https://api.nuget.org/v3/index.json",
      ApiKey = nugetToken
    });
});

Task("Release")
.IsDependentOn("Default")
.IsDependentOn("Publish-NuGet-Package");

Task("Default")
.IsDependentOn("Post-Build")
.IsDependentOn("NuGet");

RunTarget(target);