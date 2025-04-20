using System.Xml.Linq;

namespace TraktorNmlParser.Models;

public class Folder
{
    public static Folder FromXml(XElement node, Dictionary<string, Track> trackMap)
    {
        var name = (string?)node.Attribute("NAME")!;

        var subnodes = node.Elements("SUBNODES");
        var folders = node.Elements("NODE")
            .Where(n => string.Equals((string?)n.Attribute("TYPE"), "FOLDER", StringComparison.OrdinalIgnoreCase))
            .Select(n => Folder.FromXml(n, trackMap))
            .ToList();

        var playlists = subnodes.Elements("NODE")
            .Where(n => string.Equals((string?)n.Attribute("TYPE"), "PLAYLIST", StringComparison.OrdinalIgnoreCase))
            .Select(n => Playlist.FromXml(n, trackMap))
            .ToList();

        return new Folder { Name = name, Folders = folders, Playlists = playlists };
    }

    public string? Name { get; set; }
    public List<Folder> Folders { get; set; } = new();
    public List<Playlist> Playlists { get; set; } = new();
}
