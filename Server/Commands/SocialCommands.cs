namespace SoulKnightMud.Server.Commands;

// ══════════════════════════════════════════════════════════════════════════════
// TALK — mluv / talk
// ══════════════════════════════════════════════════════════════════════════════

public class TalkCommand : IGameCommand
{
    public string[] Aliases => ["mluv", "talk"];
    public string Description => "Promluv s NPC postavou";
    public string Usage => "mluv <jméno>";
    public CommandCategory Category => CommandCategory.Social;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) { await ctx.Send("Napiš: mluv <jméno>"); return; }

        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        if (room == null) return;

       string searchArg = arg.ToLower().RemoveDiacritics();
        var npc = room.Npcs.FirstOrDefault(n => n.Def.Name.ToLower().RemoveDiacritics().Contains(searchArg) && n.IsAlive);

        if (npc == null)
        {
            var deadNpc = room.Npcs.FirstOrDefault(n => n.Def.Name.ToLower().Contains(arg.ToLower()));
            if (deadNpc != null)
                await ctx.Send(ctx.Res.GetError("NpcDead").Replace("{name}", deadNpc.Def.Name));
            else
                await ctx.Send(ctx.Res.GetError("NpcNotHere").Replace("{name}", arg));
            return;
        }

        var rng = new Random();
        var line = npc.Def.DialogLines[rng.Next(npc.Def.DialogLines.Count)];

        // Show NPC portrait art
        await ctx.ShowArt("npc_talk");

        await ctx.Send($"\n  {npc.Def.Name} říká:\n  {line}\n");

        if (npc.Def.IsShop)
            await ctx.Send($"  (Tip: Napiš 'obchod {npc.Def.Name.Split(' ')[0].ToLower()}' pro zobrazení nabídky)");
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// SAY — rekni / řekni / say
// ══════════════════════════════════════════════════════════════════════════════

public class SayCommand : IGameCommand
{
    public string[] Aliases => ["rekni", "řekni", "say"];
    public string Description => "Zpráva hráčům v místnosti";
    public string Usage => "řekni <zpráva>";
    public CommandCategory Category => CommandCategory.Social;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) return;
        string formatted = $"\n  [{ctx.Player.Data.Name}]: {arg}";
        await ctx.Send(formatted);
        await ctx.BroadcastRoom(ctx.Player.RoomId, formatted, exceptSelf: true);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// SHOUT — krik / křik / shout
// ══════════════════════════════════════════════════════════════════════════════

public class ShoutCommand : IGameCommand
{
    public string[] Aliases => ["krik", "křik", "shout"];
    public string Description => "Zpráva všem hráčům na serveru";
    public string Usage => "křik <zpráva>";
    public CommandCategory Category => CommandCategory.Social;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) return;
        string formatted = $"\n  *** {ctx.Player.Data.Name} křičí: {arg} ***";
        await ctx.Sessions.BroadcastAll(formatted);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// WHISPER — soptej / šeptej / whisper / msg
// ══════════════════════════════════════════════════════════════════════════════

public class WhisperCommand : IGameCommand
{
    public string[] Aliases => ["soptej", "šeptej", "whisper", "msg"];
    public string Description => "Soukromá zpráva jinému hráči";
    public string Usage => "šeptej <hráč> <zpráva>";
    public CommandCategory Category => CommandCategory.Social;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg))
        { await ctx.Send("Napiš: šeptej <hráč> <zpráva>"); return; }

        int spaceIdx = arg.IndexOf(' ');
        if (spaceIdx < 0)
        { await ctx.Send("Napiš: šeptej <hráč> <zpráva>"); return; }

        string targetName = arg[..spaceIdx].Trim();
        string message = arg[(spaceIdx + 1)..].Trim();

        if (string.IsNullOrWhiteSpace(message))
        { await ctx.Send("Napiš: šeptej <hráč> <zpráva>"); return; }

        if (targetName.Equals(ctx.Player.Data.Name, StringComparison.OrdinalIgnoreCase))
        { await ctx.Send(ctx.Res.GetError("CannotWhisperSelf")); return; }

        bool sent = await ctx.Sessions.SendToPlayer(targetName,
            $"\n  🔒 [{ctx.Player.Data.Name} ti šeptá]: {message}");

        if (sent)
            await ctx.Send($"\n  🔒 [Šeptáš hráči {targetName}]: {message}");
        else
            await ctx.Send(ctx.Res.GetError("PlayerNotFound").Replace("{name}", targetName));
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// ONLINE — online / kdo / who
// ══════════════════════════════════════════════════════════════════════════════

public class OnlineCommand : IGameCommand
{
    public string[] Aliases => ["online", "kdo", "who"];
    public string Description => "Zobraz přihlášené hráče";
    public string Usage => "online / kdo / who";
    public CommandCategory Category => CommandCategory.Social;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        var players = ctx.Sessions.GetOnlinePlayerNames().ToList();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"╔══ ONLINE HRÁČI ({players.Count}) ══");
        foreach (var name in players)
            sb.AppendLine($"│  • {name}");
        sb.AppendLine("╚" + new string('═', 30));
        await ctx.Send(sb.ToString());
    }
}
