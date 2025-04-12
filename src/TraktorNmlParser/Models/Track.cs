using System.Xml.Linq;

namespace TraktorNmlParser.Models;

public class Track
{
    public static Track FromXml(XElement entry)
    {
        var location = entry.Element("LOCATION");
        var path = location is not null
            ? $"{location.Attribute("DIR")?.Value}{location.Attribute("FILE")?.Value}"
            : string.Empty;

        path = path.Replace("Macintosh HD/", "").Replace(":", "").TrimStart('/');

        var cues = entry.Elements("CUE_V2")
            .Select(CuePoint.FromXml)
            .OrderBy(c => c.Start)
            .ToList();

        return new Track
        {
            Path = path,
            Title = (string?)entry.Attribute("TITLE")!,
            CuePoints = cues
        };
    }

    public string? Path { get; set; }
    public string? Title { get; set; }
    public List<CuePoint> CuePoints { get; set; } = new();
}
