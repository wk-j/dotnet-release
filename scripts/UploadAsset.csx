#! "netcoreapp2.1"
#r "nuget:Octokit,0.32.0"

using Octokit;
using System.IO;
using System.Linq;

var token = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
var client = new GitHubClient(new ProductHeaderValue("my-cool-app"));
var tokenAuth = new Credentials(token);
client.Credentials = tokenAuth;
var archiveContents = File.OpenRead(".publish/DotNetRelease.Console.1.0.0.nupkg");
var assetUpload = new ReleaseAssetUpload() {
    FileName = "my-cool-project-2.0.nupkg",
    ContentType = "application/octet-stream",
    RawData = archiveContents
};

var release = (await client.Repository.Release.GetAll("wk-j", "dotnet-release")).First();

Console.WriteLine(release.Name);
var asset = await client.Repository.Release.UploadAsset(release, assetUpload);
