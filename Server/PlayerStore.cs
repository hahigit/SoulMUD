using System.Text.Json;
using BCrypt.Net;

namespace SoulKnightMud.Server;

public class PlayerStore
{
    private readonly string _playersPath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _opts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public PlayerStore(string playersPath)
    {
        _playersPath = playersPath;
        Directory.CreateDirectory(playersPath);
    }

    private string FilePath(string name) =>
        Path.Combine(_playersPath, $"{name.ToLower()}.json");

    public bool Exists(string name) => File.Exists(FilePath(name));

    public async Task<PlayerData?> Load(string name)
    {
        string path = FilePath(name);
        if (!File.Exists(path)) return null;
        string json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<PlayerData>(json, _opts);
    }

    public async Task Save(PlayerData data)
    {
        await _lock.WaitAsync();
        try
        {
            string json = JsonSerializer.Serialize(data, _opts);
            await File.WriteAllTextAsync(FilePath(data.Name), json);
        }
        finally { _lock.Release(); }
    }

    public async Task<PlayerData> CreateNew(string name, string password)
    {
        var data = new PlayerData
        {
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            RoomId = "vstupni_hala",
            Gold = 10,
            Hp = 100,
            MaxHp = 100,
            BaseAttack = 5,
            BaseDefense = 2
        };
        await Save(data);
        return data;
    }

    public bool VerifyPassword(PlayerData data, string password) =>
        BCrypt.Net.BCrypt.Verify(password, data.PasswordHash);
}
