using System.Xml.Linq;

namespace TraktorNmlParser;

public class NmlParser
{
    public async Task<Collection> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var doc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
        return Collection.FromXml(doc);
    }

    public Collection Load(string content)
    {
        var doc = XDocument.Parse(content, LoadOptions.None);
        if (doc is null)
            throw new ApplicationException("Unable to parse");
        return Collection.FromXml(doc);
    }
}
