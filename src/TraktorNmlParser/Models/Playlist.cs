using System.Xml.Linq;

namespace TraktorNmlParser.Models;

public class Playlist
{
    public static Playlist FromXml(XElement node, Dictionary<string, Track> trackMap)
    {
        var name = (string?)node.Attribute("NAME")!;

        var keys = node.Descendants("ENTRY")
            .Select(e => e.Element("PRIMARYKEY")?.Attribute("KEY")?.Value)
            .Where(k => !string.IsNullOrEmpty(k))
            .Select(k => k!.Replace("Macintosh HD/", "").Replace(":", "").TrimStart('/'))
            .ToList();

        var matchedTracks = keys
            .Where(trackMap.ContainsKey)
            .Select(k => trackMap[k])
            .ToList();

        return new Playlist { Name = name, Tracks = matchedTracks };
    }

    public string? Name { get; set; }
    public List<Track> Tracks { get; set; } = [];
}