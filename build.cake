var name = "DotNetRelease.Console";

var project = $"src/{name}/{name}.fsproj";

Task("Publish")
    .Does(() => {
        DotNetCorePublish(project, new DotNetCorePublishSettings {
            OutputDirectory = "publish/DotNetRelease.Console"
        });
    });

Task("Zip")
    .IsDependentOn("Publish")
    .Does(() => {
        Zip($"publish/DotNetRelease.Console", "publish/DotNetRelease.Console.0.1.0.zip");
    });


Task("Build").Does(() => {
    MSBuild(project, settings => {
        settings.WithTarget("Build");
    });
});

var target = Argument("target", "default");
RunTarget(target);