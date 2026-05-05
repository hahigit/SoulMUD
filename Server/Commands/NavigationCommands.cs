using System.Text;

namespace SoulKnightMud.Server.Commands;

// ══════════════════════════════════════════════════════════════════════════════
// LOOK — prozkoumej / look / l / rozhlédni
// ══════════════════════════════════════════════════════════════════════════════

public class LookCommand : IGameCommand
{
    public string[] Aliases => ["prozkoumej", "look", "l", "rozhlédni"];
    public string Description => "Prohlédni místnost";
    public string Usage => "prozkoumej / look / l";
    public CommandCategory Category => CommandCategory.Navigation;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        if (room == null) { await ctx.Send(ctx.Res.GetError("RoomNotFound")); return; }

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"╔══ {room.Def.Title.ToUpper()} {'═', -1}");
        sb.AppendLine($"│  {room.Def.Description}");
        sb.AppendLine("│");

        // Exits
        sb.AppendLine("│  [VÝCHODY]  " + (room.Def.Exits.Count > 0
            ? string.Join("  ", room.Def.Exits.Keys.Select(k => k.ToUpper()))
            : "žádné — slepá ulička!"));

        // Items
        var visibleItems = room.Items.ToList();
        sb.AppendLine("│  [PŘEDMĚTY] " + (visibleItems.Count > 0
            ? string.Join(", ", visibleItems.Select(i => i.Name))
            : "nic tu neleží"));

        // NPCs
        var aliveNpcs = room.Npcs.Where(n => n.IsAlive).ToList();
        if (aliveNpcs.Count > 0)
            sb.AppendLine("│  [POSTAVY]  " + string.Join(", ", aliveNpcs.Select(n =>
                n.Def.IsCombatant ? $"{n.Def.Name} (HP: {n.CurrentHp}/{n.Def.MaxHp})" : n.Def.Name)));

        // Dead NPCs
        var deadNpcs = room.Npcs.Where(n => !n.IsAlive).ToList();
        if (deadNpcs.Count > 0)
            sb.AppendLine("│  [PORAŽENI] " + string.Join(", ", deadNpcs.Select(n => n.Def.Name)));

        // Other players
        var others = ctx.Sessions.GetPlayersInRoom(ctx.Player.RoomId)
            .Where(n => n != ctx.Player.Data.Name).ToList();
        if (others.Count > 0)
            sb.AppendLine("│  [HRÁČI]    " + string.Join(", ", others));

        sb.AppendLine("╚" + new string('═', 52));
        await ctx.Send(sb.ToString());
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// GO — jdi / go
// ══════════════════════════════════════════════════════════════════════════════

public class GoCommand : IGameCommand
{
    public string[] Aliases => ["jdi", "go"];
    public string Description => "Pohyb (sever/jih/vychod/zapad/nahoru/dolu)";
    public string Usage => "jdi <směr>";
    public CommandCategory Category => CommandCategory.Navigation;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg))
        { await ctx.Send("Napiš: jdi <směr>"); return; }

        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        if (room == null) return;

        string cleanArg = arg.ToLower().RemoveDiacritics();
        var exitKey = room.Def.Exits.Keys.FirstOrDefault(k => k.ToLower().RemoveDiacritics() == cleanArg);

        if (exitKey == null || !room.Def.Exits.TryGetValue(exitKey, out string? targetId))
        { await ctx.Send(ctx.Res.GetError("NoExitDirection").Replace("{dir}", arg)); return; }
        var target = ctx.World.GetRoom(targetId!);
        if (target == null) { await ctx.Send(ctx.Res.GetError("TargetRoomMissing")); return; }

        // Notify old room
        await ctx.BroadcastRoom(ctx.Player.RoomId, $"  >> {ctx.Player.Data.Name} odchází na {arg}.");

        // Move
        ctx.Player.RoomId = targetId!;

        // Notify new room
        await ctx.BroadcastRoom(ctx.Player.RoomId, $"  >> {ctx.Player.Data.Name} přichází.");

        // Show new room
        await new LookCommand().ExecuteAsync("", ctx);

        // Apply room status effect on entry
        await ctx.ApplyRoomEffect(target);

        // Save
        await ctx.Store.Save(ctx.Player.Data);
    }
}
