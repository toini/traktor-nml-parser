using Shouldly;

namespace TraktorNmlParser.Tests;

public class TraktorParserSpec
{
    [Fact]
    public async Task LoadTracksAsync_Parses_Valid_Stream()
    {
        using var stream = File.OpenRead("test-data/collection.nml");
        var parser = new NmlParser();

        var collection = await parser.LoadAsync(stream);

        collection.Tracks.ShouldNotBeEmpty();
        collection.Playlists.ShouldNotBeEmpty();
    }
}
