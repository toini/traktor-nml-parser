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
$token = "your_github_pat_here"
$version = "0.0.6"

git tag "v$version"
git push origin "v$version"

dotnet clean
dotnet pack -c Release /p:Version=$version

$packagePath = "src/TraktorNmlParser/bin/Release/TraktorNmlParser.$version.nupkg"
dotnet nuget push $packagePath --source "github" --api-key $token
```

## Usage

To calculate timings and total cumulative duration:

```csharp
static string credentialsPath = @"C:\Users\toni\tmp\google-mixtapes\gdrive-toni-268419-2a19e4629ffc.json";

const string LocalPath = "Macintosh HD/:Users/:toni/:Google Drive/:Musiikki/:Traktor";
const string ExternalPath = "Musiikki/Traktor";

private DriveService _driveService;

async Task Main(string[] args)
{
	GoogleCredential credential;
	using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
	{
		credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile });
	}

	// Initialize the Google Drive API service
	_driveService = new DriveService(new BaseClientService.Initializer()
	{
		HttpClientInitializer = credential,
		ApplicationName = "GoogleDriveDownloadUrls",
	});
	
	const string fileName = "COLLECTION.nml";
	var collectionFileId = await Util.CacheAsync(async () => await GetCollectionFileId(fileName), fileName);
	using var contentStream = GetFileContent(collectionFileId);

	var parser = new NmlParser();
	var collection = await parser.LoadAsync(contentStream);
	
	const string playlistName = "Z17-2024 Psytech.rec";
	const int targetBpm = 133;
	
	//var playlistName = "2025-04-19 Golgatan farssi";
	var tracks = collection.Playlists.FirstOrDefault(p => p.Name == playlistName).Tracks;

	var timings = TracklistTransitionTimer.CalculateTransitionTiming(tracks, targetBpm).Dump();

	var plays = timings
		.Aggregate(
			new
			{
				List = new List<Row>(),
				MinTotalOrig = TimeSpan.Zero,
				MinTotalScaled = TimeSpan.Zero,
				TotalDurationOrig = TimeSpan.Zero,
				TotalDurationScaled = TimeSpan.Zero
			},
			(acc, next) =>
			{
				var minCumulativeOrig = acc.MinTotalOrig + next.MinDuration.Original;
				var minCumulativeScaled= acc.MinTotalScaled + next.MinDuration.Scaled;
				var cumulativeOrig = acc.TotalDurationOrig + next.TotalDuration.Original;
				var cumulativeScaled = acc.TotalDurationScaled + next.TotalDuration.Scaled;

				acc.List.Add(new Row
				{
					Number = acc.List.Count + 1,
					Title = next.Track.Title!,
					Artist = next.Track.Artist!,
					Bpm = Math.Round((decimal)next.Track.Bpm.Value),
					Start = next.In.Time,
					Stop = next.Out.Time,
					MinDurationOrig = next.MinDuration.Original.ToString(@"mm\:ss"),
					MinDurationScaled = next.MinDuration.Scaled.ToString(@"mm\:ss"),
					LoopDuration = next.LoopDuration.Original.ToString(@"mm\:ss"),
					MinCumulativeOrig = minCumulativeOrig.ToString(@"h\:mm\:ss"),
					TotalCumulativeOrig = cumulativeOrig.ToString(@"h\:mm\:ss"),
					MinCumulativeScaled = minCumulativeScaled.ToString(@"h\:mm\:ss"),
					TotalCumulativeScaled = cumulativeScaled.ToString(@"h\:mm\:ss")
				});

				return new
				{
					acc.List,
					MinTotalOrig = minCumulativeOrig,
					MinTotalScaled = minCumulativeScaled,
					TotalDurationOrig = cumulativeOrig,
					TotalDurationScaled = cumulativeScaled
				};
			})
		.List;

	plays.Dump();
}

public class Row
{
	public int Number { get; set; }
	public string Title { get; set; }
	public string Artist { get; set; }
	public decimal Bpm { get; set; }
	public string Start { get; set; }
	public string Stop { get; set; }
	public string MinDurationOrig { get; set; }
	public string MinDurationScaled { get; set; }	
	public string LoopDuration { get; set; }
	public string MinCumulativeOrig { get; set; }
	public string MinCumulativeScaled { get; set; }
	public string TotalCumulativeOrig { get; set; }
	public string TotalCumulativeScaled { get; set; }
}

// Fetches file id from Cloud in Musiikki/Native Instruments/collection.nml
private async Task<string> GetCollectionFileId(string fileName)
{
	// Search for the Musiikki folder
	var musiikkiRequest = _driveService.Files.List();
	musiikkiRequest.Q = "name = 'Musiikki' and mimeType='application/vnd.google-apps.folder'";
	var musiikkiResult = await musiikkiRequest.ExecuteAsync();
	var musiikkiId = musiikkiResult.Files.FirstOrDefault()?.Id;

	// Search for the Native Instruments folder inside Musiikki
	var nativeInstrumentsRequest = _driveService.Files.List();
	nativeInstrumentsRequest.Q = $"name = 'Native Instruments' and '{musiikkiId}' in parents and mimeType='application/vnd.google-apps.folder'";
	var nativeInstrumentsResult = await nativeInstrumentsRequest.ExecuteAsync();
	var nativeInstrumentsId = nativeInstrumentsResult.Files.FirstOrDefault()?.Id;

	var fileRequest = _driveService.Files.List();
	fileRequest.Q = $"name = '{fileName}' and '{nativeInstrumentsId}' in parents";
	var fileResult = await fileRequest.ExecuteAsync();
	return fileResult.Files.FirstOrDefault()?.Id ?? throw new ApplicationException("Collection file not found in Musiikki/Native Instruments/collection.nml");
}

private Stream GetFileContent(string fileId)
{
	var stream = new MemoryStream();
	_driveService.Files.Get(fileId).Download(stream);
	stream.Position = 0; // rewind before returning
	return stream;
}

```