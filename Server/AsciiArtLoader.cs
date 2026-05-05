namespace SoulKnightMud.Server;

/// <summary>
/// Loads ASCII art and animations from external .txt files in Data/ascii/.
/// Single-frame files = static art. Multi-frame files use "---FRAME---" delimiter.
/// Art is cached at startup and referenced by ID (filename without extension).
/// </summary>
public class AsciiArtLoader
{
    private const string FrameDelimiter = "---FRAME---";
    private const int DefaultFrameDelayMs = 400;

    private readonly Dictionary<string, AsciiArt> _arts = new();

    /// <summary>Load all .txt files from the given directory.</summary>
    public void Load(string asciiPath)
    {
        if (!Directory.Exists(asciiPath))
        {
            Console.WriteLine($"[WARN] ASCII art složka '{asciiPath}' nenalezena — přeskakuji.");
            return;
        }

        foreach (var file in Directory.GetFiles(asciiPath, "*.txt"))
        {
            string id = Path.GetFileNameWithoutExtension(file);
            string content = File.ReadAllText(file, System.Text.Encoding.UTF8);
            content = content.Replace("\r\n", "\n").Replace("\n", "\r\n");
            var frames = content.Split(FrameDelimiter, StringSplitOptions.None)
                .Select(f => f.Trim('\r', '\n'))
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .ToList();

            _arts[id] = new AsciiArt(id, frames);
        }
    }

    /// <summary>Get ASCII art by ID. Returns null if not found.</summary>
    public AsciiArt? Get(string id) =>
        _arts.TryGetValue(id, out var art) ? art : null;

    /// <summary>Check if an ASCII art exists.</summary>
    public bool Exists(string id) => _arts.ContainsKey(id);

    /// <summary>Get the first frame as a string (for static display).</summary>
    public string? GetStatic(string id)
    {
        var art = Get(id);
        return art?.Frames.FirstOrDefault();
    }

    /// <summary>Send static ASCII art to a client (single frame, no animation).</summary>
    public async Task SendStaticAsync(string id, Func<string, Task> send)
    {
        var frame = GetStatic(id);
        if (frame != null)
            await send(frame);
    }

    /// <summary>
    /// Play an ASCII animation by sending frames with delays.
    /// For single-frame art, just sends the one frame.
    /// </summary>
    public async Task PlayAnimationAsync(string id, Func<string, Task> send, int delayMs = DefaultFrameDelayMs)
    {
        var art = Get(id);
        if (art == null) return;

        if (art.IsAnimated)
        {
            foreach (var frame in art.Frames)
            {
                await send(frame);
                await Task.Delay(delayMs);
            }
        }
        else
        {
            await send(art.Frames[0]);
        }
    }

    public int Count => _arts.Count;
    public IEnumerable<string> AllIds => _arts.Keys;
}

/// <summary>
/// Represents a loaded ASCII art asset — can be single-frame (static) or multi-frame (animated).
/// </summary>
public class AsciiArt
{
    public string Id { get; }
    public List<string> Frames { get; }
    public bool IsAnimated => Frames.Count > 1;

    public AsciiArt(string id, List<string> frames)
    {
        Id = id;
        Frames = frames;
    }
}
