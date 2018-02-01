#! "netcoreapp2.0"
#r "nuget:NetStandard.Library,2.0"
#r "nuget:Octokit,0.27.0"

using Octokit;
using System.IO;
using System.Linq;

var token = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
var client = new GitHubClient(new ProductHeaderValue("my-cool-app"));
var tokenAuth = new Credentials(token);
client.Credentials = tokenAuth;
var archiveContents = File.OpenRead("temp/Response.zip");
var assetUpload = new ReleaseAssetUpload() {
    FileName = "my-cool-project-2.0.zip",
    ContentType = "application/zip",
    RawData = archiveContents
};

var release = (await client.Repository.Release.GetAll("wk-j", "dotnet-release-temp")).First();

Console.WriteLine(release.Name);
var asset = await client.Repository.Release.UploadAsset(release, assetUpload);
