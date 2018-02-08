#r "nuget:Octokit, 0.27.0"

using Octokit;

public static class GitHub {
    public static void CreateReleaseDraft(string githubReleaseFolder) {
        var accessToken = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var client = new GitHubClient(new ProductHeaderValue("dotnet-script"));
        string latestTag = "1.0.0";
        var newRelease = new NewRelease(latestTag);
        newRelease.Name = latestTag;
        newRelease.Body = "Hello, world!";
        newRelease.Draft = true;
        newRelease.Prerelease = latestTag.Contains("-");

        var tokenAuth = new Credentials(accessToken);
        client.Credentials = tokenAuth;

        var createdRelease = client.Repository.Release.Create("wk-j", "temp", newRelease).Result;

        var assets = Directory.GetFiles(githubReleaseFolder, "*.zip");
        foreach (var asset in assets) {
            var archiveContents = File.OpenRead(asset);
            var assetUpload = new ReleaseAssetUpload() {
                FileName = Path.GetFileName(asset),
                ContentType = "application/zip",
                RawData = archiveContents
            };
            client.Repository.Release.UploadAsset(createdRelease, assetUpload).Wait();
        }
    }
}

GitHub.CreateReleaseDraft("publish");