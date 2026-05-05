namespace SoulKnightMud.Server;

public class GameWorld
{
    private readonly Dictionary<string, Room> _rooms = new();
    private readonly Dictionary<string, ItemDef> _items = new();
    private readonly Dictionary<string, NpcDef> _npcs = new();
    private readonly Dictionary<string, ShopDef> _shops = new();
    private readonly Dictionary<string, StatusEffectDef> _statusEffects = new();

    /// <summary>ASCII art loader — loaded from Data/ascii/*.txt files.</summary>
    public AsciiArtLoader AsciiArt { get; } = new();

    public void Load(string dataPath)
    {
        // ASCII art (from sibling ascii/ folder)
        string asciiPath = Path.Combine(Path.GetDirectoryName(dataPath) ?? dataPath, "ascii");
        AsciiArt.Load(asciiPath);

        // Items
        foreach (var item in WorldLoader.Load<ItemDef>(Path.Combine(dataPath, "items.json")))
            _items[item.Id] = item;

        // NPCs
        foreach (var npc in WorldLoader.Load<NpcDef>(Path.Combine(dataPath, "npcs.json")))
            _npcs[npc.Id] = npc;

        // Shops
        foreach (var shop in WorldLoader.Load<ShopDef>(Path.Combine(dataPath, "shops.json")))
            _shops[shop.Id] = shop;

        // Status effects
        foreach (var se in WorldLoader.Load<StatusEffectDef>(Path.Combine(dataPath, "status_effects.json")))
            _statusEffects[se.Id] = se;

        // Rooms — build runtime rooms with item/npc instances
        foreach (var def in WorldLoader.Load<RoomDef>(Path.Combine(dataPath, "rooms.json")))
        {
            var room = new Room(def);
            foreach (var id in def.ItemIds)
                if (_items.TryGetValue(id, out var item))
                    room.Items.Add(item);
            foreach (var id in def.NpcIds)
                if (_npcs.TryGetValue(id, out var npc))
                    room.Npcs.Add(new NpcInstance(npc));
            _rooms[def.Id] = room;
        }
    }

    public Room? GetRoom(string id) => _rooms.TryGetValue(id, out var r) ? r : null;
    public ItemDef? GetItem(string id) => _items.TryGetValue(id, out var i) ? i : null;
    public ShopDef? GetShop(string id) => _shops.TryGetValue(id, out var s) ? s : null;
    public StatusEffectDef? GetStatusEffect(string id) => _statusEffects.TryGetValue(id, out var se) ? se : null;
    public ItemDef? FindItemByName(string name) =>
        _items.Values.FirstOrDefault(i => i.Name.ToLower().Contains(name.ToLower()));

    /// <summary>
    /// Tick all dead NPCs across all rooms. Returns (roomId, npcName) of respawned NPCs
    /// so the caller can notify players in that room.
    /// </summary>
    public List<(string RoomId, string NpcName)> TickNpcRespawns()
    {
        var respawned = new List<(string, string)>();
        foreach (var (roomId, room) in _rooms)
        {
            foreach (var npc in room.Npcs)
            {
                if (npc.TryRespawn())
                    respawned.Add((roomId, npc.Def.Name));
            }
        }
        return respawned;
    }
}
