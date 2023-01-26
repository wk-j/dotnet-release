## .NET Release

Publish GitHub release

[![Actions](https://github.com/wk-j/dotnet-release/workflows/NuGet/badge.svg)](https://github.com/wk-j/dotnet-release/actions)
[![NuGet](https://img.shields.io/nuget/v/wk.DotNetRelease.svg)](https://www.nuget.org/packages/wk.DotNetRelease)

## Installation

```
dotnet tool install -g wk.DotNetRelease
```

## Usage

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