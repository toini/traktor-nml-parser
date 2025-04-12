using System.Xml.Linq;
using TraktorNmlParser.Models;

public class Collection
{
    public static Collection FromXml(XDocument doc)
    {
        var trackMap = doc.Descendants("ENTRY")
            .Select(Track.FromXml)
            .Where(t => !string.IsNullOrWhiteSpace(t.Path) && !t.Path.Contains("silent", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(t => t.Path!);

        var playlists = doc.Descendants("NODE")
            .Where(n => string.Equals((string?)n.Attribute("TYPE"), "PLAYLIST", StringComparison.OrdinalIgnoreCase))
            .Select(n => Playlist.FromXml(n, trackMap))
            .ToList();

        return new Collection
        {
            Tracks = trackMap.Values.ToList(),
            Playlists = playlists
        };
    }

    public List<Track> Tracks { get; set; } = new();
    public List<Playlist> Playlists { get; set; } = new();
}
