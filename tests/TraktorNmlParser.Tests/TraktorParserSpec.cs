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
        collection.Folders.ShouldNotBeEmpty();
    }

    [Fact]
    public void LoadTracksAsync_Parses_Valid_String()
    {
        var content = File.ReadAllText("test-data/collection.nml");
        var parser = new NmlParser();

        var collection = parser.Load(content);

        collection.Tracks.ShouldNotBeEmpty();
        collection.Folders.ShouldNotBeEmpty();
    }
}
