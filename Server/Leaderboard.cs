using System.Text.Json;

namespace SoulKnightMud.Server;

public class Leaderboard
{
    private readonly string _path;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

    public Leaderboard(string path)
    {
        _path = path;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    }

    public async Task AddEntry(string playerName, int gold)
    {
        var entries = await Load();
        entries.Add(new LeaderboardEntry
        {
            PlayerName = playerName,
            CompletedAt = DateTime.Now,
            GoldAtCompletion = gold
        });
        await Save(entries);
    }

    public async Task<List<LeaderboardEntry>> Load()
    {
        if (!File.Exists(_path)) return new();
        string json = await File.ReadAllTextAsync(_path);
        return JsonSerializer.Deserialize<List<LeaderboardEntry>>(json, _opts) ?? new();
    }

    private async Task Save(List<LeaderboardEntry> entries)
    {
        await _lock.WaitAsync();
        try
        {
            string json = JsonSerializer.Serialize(entries, _opts);
            await File.WriteAllTextAsync(_path, json);
        }
        finally { _lock.Release(); }
    }
}
