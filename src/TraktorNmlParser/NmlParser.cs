using System.Xml.Linq;

namespace TraktorNmlParser;

public class NmlParser
{
    public async Task<Collection> LoadAsync(Stream stream)
    {
        var doc = await XDocument.LoadAsync(stream, LoadOptions.None, default);
        return Collection.FromXml(doc);
    }
}
