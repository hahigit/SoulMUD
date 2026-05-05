using System.Text;

namespace SoulKnightMud.Server.Commands;

/// <summary>
/// Shared context passed to every command — contains all dependencies
/// and common helper methods (broadcast, status effects, etc.).
/// </summary>
public class CommandContext
{
    public Player Player { get; }
    public GameWorld World { get; }
    public SessionManager Sessions { get; }
    public Func<string, Task> Send { get; }
    public GameLogger Log { get; }
    public PlayerStore Store { get; }
    public Leaderboard Leaderboard { get; }
    public GameResources Res { get; }
    public AsciiArtLoader AsciiArt { get; }
    public bool Debug { get; }

    private readonly Random _rng = new();

    public CommandContext(
        Player player, GameWorld world, SessionManager sessions,
        Func<string, Task> send, GameLogger log, PlayerStore store,
        Leaderboard leaderboard, GameResources res, bool debug)
    {
        Player = player;
        World = world;
        Sessions = sessions;
        Send = send;
        Log = log;
        Store = store;
        Leaderboard = leaderboard;
        Res = res;
        AsciiArt = world.AsciiArt;
        Debug = debug;
    }

    // ── Shared helpers ───────────────────────────────────────────────────────

    public int Roll(int min, int max) => _rng.Next(min, max);
    public int RollVariance() => _rng.Next(-2, 3);

    /// <summary>Broadcast a message to all players in a specific room.</summary>
    public async Task BroadcastRoom(string roomId, string msg, bool exceptSelf = false)
    {
        foreach (var (name, sendFn) in Sessions.GetSendersInRoom(roomId))
        {
            if (exceptSelf && name == Player.Data.Name) continue;
            await sendFn(msg);
        }
    }

    /// <summary>Apply a status effect by ID to the current player.</summary>
    public async Task ApplyStatus(string id)
    {
        var def = World.GetStatusEffect(id);
        if (def == null) return;
        Player.StatusEffects.RemoveAll(s => s.Def.Id == id);
        Player.StatusEffects.Add(new ActiveStatusEffect(def));
        await Send($"  Status '{def.Name}' aktivní po {def.DurationTurns} tahů.");
    }

    /// <summary>Tick all active status effects (called once per command).</summary>
    public async Task TickStatusEffects()
    {
        var toRemove = new List<ActiveStatusEffect>();
        foreach (var se in Player.StatusEffects)
        {
            if (!string.IsNullOrEmpty(se.Def.TickMessage))
            {
                Player.Data.Hp = Math.Max(0, Player.Data.Hp + se.Def.HpPerTurn);
                await Send($"  [{se.Def.Name}] {se.Def.TickMessage}  HP: {Player.Data.Hp}/{Player.Data.MaxHp}");
            }
            se.TurnsRemaining--;
            if (se.TurnsRemaining <= 0)
            {
                toRemove.Add(se);
                await Send($"  [Status '{se.Def.Name}' skončil.]");
            }
        }
        Player.StatusEffects.RemoveAll(se => toRemove.Contains(se));
    }

    /// <summary>Apply room status effect if the room defines one.</summary>
    public async Task ApplyRoomEffect(Room room)
    {
        if (!string.IsNullOrEmpty(room.Def.StatusEffect))
        {
            var def = World.GetStatusEffect(room.Def.StatusEffect);
            if (def != null)
            {
                await Send($"\n  ⚠ Cítíš podivnou energii v místnosti...");
                await ApplyStatus(room.Def.StatusEffect);
            }
        }
    }

    /// <summary>Guess status ID for attack_boost_temp items.</summary>
    public string GuessStatusId(string itemId) => itemId switch
    {
        "magicky_svitek" => "fire_weapon",
        "lektvár_sily" => "strength",
        "bomba" => "bomb_blast",
        _ => ""
    };

    // ── ASCII Art helpers ─────────────────────────────────────────────────────

    /// <summary>Send static ASCII art (first frame only).</summary>
    public async Task ShowArt(string id)
    {
        await AsciiArt.SendStaticAsync(id, Send);
    }

    /// <summary>Play animated ASCII art (multi-frame with delay).</summary>
    public async Task PlayArt(string id, int delayMs = 400)
    {
        await AsciiArt.PlayAnimationAsync(id, Send, delayMs);
    }
}
