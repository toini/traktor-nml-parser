using System.Xml.Linq;

namespace TraktorNmlParser.Models;
public enum CuePointType
{
    Unknown = -1,
    Cue = 0,
    AutoGrid = 4,
    Loop = 5
}

public class CuePoint
{
    public double Start { get; set; }
    public CuePointType Type { get; set; }
    public string? Name { get; set; }
    public string? Hotcue { get; set; }
    public double? Length { get; set; }
    public int? Order { get; set; }
    public int? Repeats { get; set; }

    public static CuePoint FromXml(XElement element) => new()
    {
        Start = (double?)element.Attribute("START") ?? 0,
        Type = int.TryParse((string?)element.Attribute("TYPE"), out var t) ? (CuePointType)t : CuePointType.Unknown,
        Name = (string?)element.Attribute("NAME"),
        Hotcue = (string?)element.Attribute("HOTCUE"),
        Length = (double?)element.Attribute("LEN"),
        Order = (int?)element.Attribute("DISPL_ORDER"),
        Repeats = (int?)element.Attribute("REPEATS")
    };
}
