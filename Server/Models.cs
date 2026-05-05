using System.Text.Json.Serialization;

namespace SoulKnightMud.Server;

// ── JSON-loaded world data ───────────────────────────────────────────────────

public class ItemDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Weight { get; set; } = 1;
    public int Value { get; set; } = 0;
    public bool Usable { get; set; } = false;
    public bool Consumable { get; set; } = false;
    public UseEffect? UseEffect { get; set; }
    public int AttackBonus { get; set; } = 0;
    public int DefenseBonus { get; set; } = 0;
    public bool IsQuestItem { get; set; } = false;
    public bool IsWinCondition { get; set; } = false;
}

public class UseEffect
{
    public string Type { get; set; } = "";  // heal, status, info, gold, attack_boost_temp, heal_and_status
    public int Amount { get; set; } = 0;
    public string? StatusId { get; set; }
    public int Duration { get; set; } = 0;
    public string Message { get; set; } = "";
}

public class NpcDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsShop { get; set; } = false;
    public string? ShopId { get; set; }
    public bool IsCombatant { get; set; } = false;
    public bool IsBoss { get; set; } = false;
    public int Hp { get; set; } = 0;
    public int MaxHp { get; set; } = 0;
    public int Attack { get; set; } = 0;
    public int Defense { get; set; } = 0;
    public int GoldReward { get; set; } = 0;
    public string? ItemReward { get; set; }
    public List<string> DialogLines { get; set; } = new();
    /// <summary>Number of global ticks until NPC respawns after death. 0 = never respawn (boss).</summary>
    public int RespawnTurns { get; set; } = 0;
}

public class RoomDef
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, string> Exits { get; set; } = new();
    public List<string> ItemIds { get; set; } = new();
    public List<string> NpcIds { get; set; } = new();
    public string? StatusEffect { get; set; }
}

public class ShopDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public List<ShopItem> Items { get; set; } = new();
}

public class ShopItem
{
    public string ItemId { get; set; } = "";
    public int Price { get; set; } = 0;
}

public class StatusEffectDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int AttackBonus { get; set; } = 0;
    public int DefenseBonus { get; set; } = 0;
    public int HpPerTurn { get; set; } = 0;
    public int DurationTurns { get; set; } = 0;
    public string? TickMessage { get; set; }
}

// ── Runtime NPC instance (per room, resettable HP) ───────────────────────────

public class NpcInstance
{
    public NpcDef Def { get; }
    public int CurrentHp { get; set; }
    public bool IsAlive => !Def.IsCombatant || CurrentHp > 0;

    /// <summary>Turns spent dead. Only tracked for NPCs with RespawnTurns > 0.</summary>
    public int DeadTurns { get; set; } = 0;

    public NpcInstance(NpcDef def)
    {
        Def = def;
        CurrentHp = def.MaxHp > 0 ? def.MaxHp : 1;
    }

    /// <summary>Check if this NPC should respawn, and if so, reset it.</summary>
    public bool TryRespawn()
    {
        if (IsAlive) return false;
        if (Def.RespawnTurns <= 0) return false; // 0 = never respawn (boss)

        DeadTurns++;
        if (DeadTurns >= Def.RespawnTurns)
        {
            CurrentHp = Def.MaxHp;
            DeadTurns = 0;
            return true;
        }
        return false;
    }
}

// ── Runtime Room ──────────────────────────────────────────────────────────────

public class Room
{
    public RoomDef Def { get; }
    public List<ItemDef> Items { get; } = new();
    public List<NpcInstance> Npcs { get; } = new();

    public Room(RoomDef def) => Def = def;
}

// ── Active status effect on a player ─────────────────────────────────────────

public class ActiveStatusEffect
{
    public StatusEffectDef Def { get; }
    public int TurnsRemaining { get; set; }

    public ActiveStatusEffect(StatusEffectDef def)
    {
        Def = def;
        TurnsRemaining = def.DurationTurns;
    }
}

// ── Persisted player data ─────────────────────────────────────────────────────

public class PlayerData
{
    public string Name { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string RoomId { get; set; } = "vstupni_hala";
    public List<string> InventoryItemIds { get; set; } = new();
    public int Gold { get; set; } = 10;
    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;
    public int BaseAttack { get; set; } = 5;
    public int BaseDefense { get; set; } = 2;
    public int MaxInventoryWeight { get; set; } = 15;
    public bool GameCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
}

// ── Runtime player (combines persisted + live state) ─────────────────────────

public class Player
{
    public PlayerData Data { get; }
    public List<ItemDef> Inventory { get; } = new();
    public List<ActiveStatusEffect> StatusEffects { get; } = new();

    public Player(PlayerData data) => Data = data;

    public int TotalAttack
    {
        get
        {
            int bonus = Inventory.Sum(i => i.AttackBonus)
                      + StatusEffects.Sum(e => e.Def.AttackBonus);
            return Data.BaseAttack + bonus;
        }
    }

    public int TotalDefense
    {
        get
        {
            int bonus = Inventory.Sum(i => i.DefenseBonus)
                      + StatusEffects.Sum(e => e.Def.DefenseBonus);
            return Data.BaseDefense + bonus;
        }
    }

    public int UsedWeight() => Inventory.Sum(i => i.Weight);
    public bool CanCarry(ItemDef item) => UsedWeight() + item.Weight <= Data.MaxInventoryWeight;

    public string RoomId
    {
        get => Data.RoomId;
        set => Data.RoomId = value;
    }
}

// ── Leaderboard ───────────────────────────────────────────────────────────────

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = "";
    public DateTime CompletedAt { get; set; }
    public int GoldAtCompletion { get; set; }
}
