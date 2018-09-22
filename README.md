## .NET Release

Publish GitHub release

[![Build Status](https://travis-ci.org/wk-j/dotnet-release.svg?branch=master)](https://travis-ci.org/wk-j/dotnet-release)
[![Build Status](https://dev.azure.com/wk-j/dotnet-release/_apis/build/status/wk-j.dotnet-release)](https://dev.azure.com/wk-j/dotnet-release/_build/latest?definitionId=11)
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