using System.Xml.Linq;

namespace TraktorNmlParser.Models;

public class Track
{
    public string Path { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public string? Comment { get; set; }
    public float? Bpm { get; set; }
    public string? Genre { get; set; }
    public string? Key { get; set; }
    public int? Bitrate { get; set; }
    public int? Playtime { get; set; }
    public List<CuePoint> CuePoints { get; set; } = new();

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

        var info = entry.Element("INFO");
        var tempo = entry.Element("TEMPO");

        return new Track
        {
            Path = path,
            Title = (string?)entry.Attribute("TITLE"),
            Artist = (string?)entry.Attribute("ARTIST"),
            CuePoints = cues,
            Comment = (string?)info?.Attribute("COMMENT"),
            Genre = (string?)info?.Attribute("GENRE"),
            Key = (string?)info?.Attribute("KEY"),
            Bitrate = (int?)info?.Attribute("BITRATE"),
            Playtime = (int?)info?.Attribute("PLAYTIME"),
            Bpm = (float?)tempo?.Attribute("BPM")
        };
    }
}
