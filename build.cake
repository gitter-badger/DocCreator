///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var pushPackage = Argument<bool>("uploadpackage", false); 

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

Func<IFileSystemInfo, bool> excludePackageProject = info => !info.Path.FullPath.Contains("TemplatePackage.csproj");
var solutions = GetFiles("./**/*.sln"); 
var projects = GetFiles("./**/*.csproj", excludePackageProject);
var projectPaths = projects.Select(p => p.GetDirectory());
var outputPath = "bin\\" + configuration + "\\";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in projectPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
	DeleteFiles(GetFiles("./**/TemplatePackage/*.nupkg*"));
});

Task("Restore")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);
        NuGetRestore(solution);
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    // Build all solutions.
    foreach(var project in projects)
    {
        Information("Building {0}", project);
        MSBuild(project, settings => 
            settings.SetPlatformTarget(PlatformTarget.MSIL)
                .WithProperty("TreatWarningsAsErrors","true")
				.WithProperty("OutputPath", outputPath)
                .WithTarget("Build")
                .SetConfiguration(configuration));
    }
});

Task("BuildTemplatePackage")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() => {
		var releaseMode = pushPackage ? "Release" : "Debug";
		Information("Building TemplatePackage project in {0} mode", releaseMode);
		var projects = GetFiles("./**/TemplatePackage.csproj");
		foreach (var projectFile in projects) {
			MSBuild(projectFile, settings =>
				settings.SetConfiguration(releaseMode)
					.WithTarget("Build"));
		}
	});
	
Task("Publish")
	.IsDependentOn("Build")
	.IsDependentOn("BuildTemplatePackage")
	.Does(() => {
		Information("Merging DocCreator.exe");
		CreateDirectory("./build");
		var assemblyList = GetFiles("./src/DocCreator/bin/" + configuration + "/**/*.dll");
		Information("Executing ILMerge to merge {0} assemblies", assemblyList.Count);
		ILRepack(
			"./build/DocCreator.exe",
			"./src/DocCreator/bin/" + configuration + "/DocCreator.exe",
			assemblyList);
		
	});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);