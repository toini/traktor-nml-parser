using TraktorNmlParser.Models;

namespace TraktorNmlParser;

public class TracklistTransitionTimer
{
    public static IEnumerable<TrackPlayTiming> CalculateTransitionTiming(IList<Track> tracks, float targetBpm) =>
        tracks
            .Select(t =>
                TrackPlayTiming.FromCuePoints(t, targetBpm, isFirst: t == tracks.First(), isLast: t == tracks.Last()));
}


public class TransitionPoint
{
    public static string ReasonFromCuePoint(CuePoint cue) => $"{cue.Name} {cue.Hotcue + 1} {cue.Type}";

    public string Time { get; set; }
    public string Reason { get; set; }
    public TimeSpan LoopDuration { get; set; }
}

public class Duration
{
    public TimeSpan Original { get; set; }
    public TimeSpan Scaled { get; set; }

    public Duration(Track track, TimeSpan orginal, float targetBpm)
    {
        Original = orginal;
        if (!track.Bpm.HasValue)
            throw new ArgumentException($"Track {track.Artist} - {track.Title} does not have BPM set");
        Scaled = ScaleToBpm(orginal, track.Bpm.Value, targetBpm);
    }

    public TimeSpan ScaleToBpm(TimeSpan duration, float originalBpm, float targetBpm)
    {
        var scaleFactor = originalBpm / targetBpm;
        return TimeSpan.FromTicks((long)(duration.Ticks * scaleFactor));
    }
}

public class TrackPlayTiming
{
    public Track Track { get; set; }
    public TransitionPoint In { get; set; }
    public TransitionPoint Out { get; set; }
    public Duration MinDuration { get; set; }
    public Duration LoopDuration { get; set; }
    public Duration TotalDuration { get; set; }

    public static TrackPlayTiming FromCuePoints(Track track, float targetBpm, bool isFirst = false, bool isLast = false)
    {
        var cues = track.CuePoints;
        var inCue = isFirst
            ? cues.Single(c => c.Type == CuePointType.AutoGrid)
            : cues.SingleOrDefault(c => c.Name?.ToLower() == "in") ?? cues.FirstOrDefault(c => c.Type == CuePointType.Load);

        if (inCue is null)
        {
            Console.WriteLine($"No cue point of type load or name in found; fallback to first cue point #2; {track.Artist} - {track.Title}");
            inCue = cues.FirstOrDefault(c => c.Hotcue == 1);
        }
        if (inCue is null)
            return new TrackPlayTiming();

        var outCue = isLast
            ? null
            : cues.SingleOrDefault(c => c.Name?.ToLower() == "out")
              ?? cues.LastOrDefault(c => c.Type == CuePointType.Loop)
              ?? cues.LastOrDefault(c => c.Type == CuePointType.Cue);

        if (!isLast && outCue is null)
            throw new InvalidOperationException($"No out transition point found for {track.Artist} - {track.Title}");

        var outTime = isLast ? TimeSpan.FromSeconds(track.Playtime.Value) : TimeSpan.FromMilliseconds(outCue.Start);

        var inLoopCue = cues.FirstOrDefault(c => !isFirst && c.Type == CuePointType.Loop && c.Start >= inCue.Start);
        var inTransition = new TransitionPoint
        {
            Time = TimeSpan.FromMilliseconds(inCue.Start).ToString(@"m\:ss"),
            LoopDuration = TimeSpan.FromMilliseconds(inLoopCue?.Length ?? 0),
            Reason = TransitionPoint.ReasonFromCuePoint(inCue)
        };

        var outLoopCue = cues.FirstOrDefault(c => !isLast && c.Type == CuePointType.Loop && c.Start <= outCue.Start && c != inLoopCue);
        var outTransition = new TransitionPoint
        {
            Time = outTime.ToString(@"m\:ss"),
            LoopDuration = TimeSpan.FromMilliseconds(outLoopCue?.Length ?? 0),
            Reason = isLast ? "playlist end" : TransitionPoint.ReasonFromCuePoint(outCue)
        };

        var minOriginal = outTime - TimeSpan.FromMilliseconds(inCue.Start);
        var loopOriginal = inTransition.LoopDuration + outTransition.LoopDuration;
        var totalOriginal = minOriginal + loopOriginal;

        return new TrackPlayTiming
        {
            Track = track,
            In = inTransition,
            Out = outTransition,
            MinDuration = new Duration(track, minOriginal, targetBpm),
            LoopDuration = new Duration(track, loopOriginal, targetBpm),
            TotalDuration = new Duration(track, totalOriginal, targetBpm),
        };
    }
}
