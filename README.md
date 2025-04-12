# TraktorNmlParser

A .NET library for parsing Traktor `.nml` (XML) collection files.
Provides a strongly-typed model and easy APIs for reading Traktor metadata in your .NET applications.

## Features

- Parses Traktor `.nml` files to C# objects
- LINQ-friendly data access
- Supports common Traktor metadata like tracks, playlists, cues, and more

## Install

You can consume this package via GitHub Packages:

```bash
dotnet add package TraktorNmlParser --source "https://nuget.pkg.github.com/toini/index.json"
```

## Publishing

To publish a new version to GitHub Packages:

```powershell
$version = "0.0.1"
$token = "your_github_pat_here"

git tag "v$version"
git push origin "v$version"

dotnet clean
dotnet pack -c Release /p:Version=$version

$packagePath = "src/TraktorNmlParser/bin/Release/TraktorNmlParser.$version.nupkg"
dotnet nuget push $packagePath --source "github" --api-key $token
```
