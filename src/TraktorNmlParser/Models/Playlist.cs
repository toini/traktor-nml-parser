using System.Xml.Linq;

namespace TraktorNmlParser.Models;

public class Playlist
{
    public static Playlist FromXml(XElement node, Dictionary<string, Track> trackMap)
    {
        var name = (string?)node.Attribute("NAME")!;
        var playlistNode = node.Element("PLAYLIST")!;
        var entries = (int)playlistNode.Attribute("ENTRIES")!;

        var keys = playlistNode.Elements("ENTRY")
            .Select(e => e.Element("PRIMARYKEY")?.Attribute("KEY")?.Value)
            .Where(k => !string.IsNullOrEmpty(k))
            .Select(k => k!.Replace("Macintosh HD/", "").Replace(":", "").TrimStart('/'))
            .ToList();

        var matchedTracks = keys
            .Where(trackMap.ContainsKey)
            .Select(k => trackMap[k])
            .ToList();

        return new Playlist { Name = name, Entries = entries, Tracks = matchedTracks };
    }

    public string? Name { get; set; }
    public int Entries { get; set; }
    public List<Track> Tracks { get; set; } = [];
}