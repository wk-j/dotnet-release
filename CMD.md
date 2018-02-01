## Instructions

```bash
dotnet run --project src/DotNetRelease.Console --owner wk-j --repository dotnet-release-temp --tag 0.0.12 --token $GITHUB_TOKEN --name "Test release" --body "- This is body" --asset "temp/Response.zip"
```