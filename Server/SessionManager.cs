using System.Collections.Concurrent;

namespace SoulKnightMud.Server;

public class SessionManager
{
    private readonly ConcurrentDictionary<Guid, PlayerSession> _sessions = new();

    public int Count => _sessions.Count;

    public void Add(PlayerSession session) => _sessions[session.Id] = session;
    public void Remove(Guid id) => _sessions.TryRemove(id, out _);

    /// <summary>Check if a player with the given name is already logged in.</summary>
    public bool IsPlayerOnline(string name) =>
        _sessions.Values.Any(s =>
            s.IsAuthenticated &&
            s.Player!.Data.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<string> GetPlayersInRoom(string roomId) =>
        _sessions.Values
            .Where(s => s.IsAuthenticated && s.Player!.RoomId == roomId)
            .Select(s => s.Player!.Data.Name);

    public IEnumerable<(string Name, Func<string, Task> Send)> GetSendersInRoom(string roomId) =>
        _sessions.Values
            .Where(s => s.IsAuthenticated && s.Player!.RoomId == roomId)
            .Select(s => (s.Player!.Data.Name, s.SendAsync));

    public async Task BroadcastAll(string message)
    {
        foreach (var session in _sessions.Values)
            if (session.IsAuthenticated)
                await session.SendAsync(message);
    }

    /// <summary>
    /// Find a specific online player by name and send them a message (whisper).
    /// Returns true if the player was found and message sent.
    /// </summary>
    public async Task<bool> SendToPlayer(string playerName, string message)
    {
        var session = _sessions.Values.FirstOrDefault(s =>
            s.IsAuthenticated &&
            s.Player!.Data.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));

        if (session == null) return false;
        await session.SendAsync(message);
        return true;
    }

    /// <summary>
    /// Auto-save: persist all online players' data.
    /// </summary>
    public async Task<int> SaveAllPlayers(PlayerStore store)
    {
        int saved = 0;
        foreach (var session in _sessions.Values)
        {
            if (session.IsAuthenticated && session.Player != null)
            {
                try
                {
                    await store.Save(session.Player.Data);
                    saved++;
                }
                catch { /* Individual save errors don't halt the loop */ }
            }
        }
        return saved;
    }

    /// <summary>
    /// Get all online player names (for whisper auto-complete etc.)
    /// </summary>
    public IEnumerable<string> GetOnlinePlayerNames() =>
        _sessions.Values
            .Where(s => s.IsAuthenticated)
            .Select(s => s.Player!.Data.Name);
}
