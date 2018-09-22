## Instructions

```bash

dotnet run --project src/DotNetRelease.Console --owner wk-j --repository dotnet-release --tag v0.0.9 --token $GITHUB_TOKEN --name "Test release" --body "- This is body" --asset ".publish/DotNetRelease.Console.1.0.0.nupkg"
```

```bash
wk-dotnet-release \
    --owner wk-j \
    --repository dotnet-release \
    --tag v0.1.9 \
    --token $GITHUB_TOKEN \
    --name "Release name" \
    --body "This is body" \
    --asset ".publish/wk.DotNetRelease.0.1.0.nupkg"
```