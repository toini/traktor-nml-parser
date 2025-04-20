using System.Xml.Linq;
using TraktorNmlParser.Models;

public class Collection
{
    public static Collection FromXml(XDocument doc)
    {
        var nmlRoot = doc.Element("NML");
        var trackMap = nmlRoot!
            .Element("COLLECTION")?
            .Elements("ENTRY")
            .Select(Track.FromXml)
            .Where(t => !string.IsNullOrWhiteSpace(t.Path) && !t.Path.Contains("silent", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(t => t.Path!) ?? new Dictionary<string, Track>();

        var folders = nmlRoot.Element("PLAYLISTS")!
            .Elements("NODE")
            .Elements("SUBNODES")
            .Elements("NODE")
            .Where(n => string.Equals((string?)n.Attribute("TYPE"), "FOLDER", StringComparison.OrdinalIgnoreCase))
            .Select(n => Folder.FromXml(n, trackMap))
            .ToList();

        return new Collection
        {
            Tracks = trackMap.Values.ToList(),
            Folders = folders
        };
    }

    public List<Track> Tracks { get; set; } = new();
    public List<Folder> Folders { get; set; } = new();
}
