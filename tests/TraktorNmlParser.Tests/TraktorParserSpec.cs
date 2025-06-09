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

        collection.Tracks.Count().ShouldBe(1517);
        collection.Folders.Count().ShouldBe(10);
    }

    [Fact]
    public void LoadTracksAsync_Parses_Valid_String()
    {
        var content = File.ReadAllText("test-data/collection.nml");
        var parser = new NmlParser();

        var collection = parser.Load(content);

        collection.Tracks.Count().ShouldBe(1517);
        collection.Folders.Count().ShouldBe(10);
    }
}
