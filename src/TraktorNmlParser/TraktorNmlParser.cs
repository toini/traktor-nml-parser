using System.Xml.Linq;

namespace TraktorNmlParser;

public class TraktorParser
{
    static readonly HashSet<string> KnownEntryElements = new(
        ["LOCATION", "ALBUM", "MODIFICATION_INFO", "INFO", "TEMPO", "LOUDNESS", "MUSICAL_KEY", "LOOPINFO", "CUE_V2", "STEMS", "PRIMARYKEY"]);

    static readonly HashSet<string> KnownEntryAttributes = new(
        ["MODIFIED_DATE", "MODIFIED_TIME", "LOCK", "LOCK_MODIFICATION_TIME", "AUDIO_ID", "TITLE", "ARTIST"]);

    public async Task<Collection> LoadAsync(Stream stream)
    {
        var doc = await XDocument.LoadAsync(stream, LoadOptions.None, default);
        return Collection.FromXml(doc);
    }
}
