using System.Diagnostics;
using TraktorNmlParser;

public class Program
{
    static void Main(string[] args)
    {
        var stopWatch = Stopwatch.StartNew();
        var content = File.ReadAllText("test-data/collection.nml");
        var parser = new NmlParser();

        var collection = parser.Load(content);
        var tracks = collection.Tracks.Count;
        Console.WriteLine($"Parsed collection with {tracks} tracks ({stopWatch.Elapsed.TotalSeconds}s)");
    }
}
