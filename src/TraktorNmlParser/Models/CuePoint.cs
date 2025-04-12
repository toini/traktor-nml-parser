using System.Xml.Linq;

namespace TraktorNmlParser.Models;
public class CuePoint
{
    public static CuePoint FromXml(XElement element) => new()
    {
        Start = (double?)element.Attribute("START") ?? 0,
        Type = (string?)element.Attribute("TYPE"),
        Name = (string?)element.Attribute("NAME"),
        Hotcue = (string?)element.Attribute("HOTCUE")
    };

    public double Start { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Hotcue { get; set; }
}
