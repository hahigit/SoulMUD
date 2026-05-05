using System.Text;

namespace SoulKnightMud.Server.Commands;

// ══════════════════════════════════════════════════════════════════════════════
// TAKE — vezmi / seber / take
// ══════════════════════════════════════════════════════════════════════════════

public class TakeCommand : IGameCommand
{
    public string[] Aliases => ["vezmi", "seber", "take"];
    public string Description => "Vezmi předmět z místnosti";
    public string Usage => "vezmi <předmět>";
    public CommandCategory Category => CommandCategory.Items;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) { await ctx.Send("Napiš: vezmi <předmět>"); return; }
        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        if (room == null) return;

        string searchArg = arg.ToLower().RemoveDiacritics();
        var item = room.Items.FirstOrDefault(i => i.Name.ToLower().RemoveDiacritics().Contains(searchArg));
        if (item == null)
        { await ctx.Send(ctx.Res.GetError("ItemNotInRoom").Replace("{name}", arg)); return; }

        if (!ctx.Player.CanCarry(item))
        { await ctx.Send(ctx.Res.GetError("InventoryFull").Replace("{name}", item.Name)); return; }

        room.Items.Remove(item);
        ctx.Player.Inventory.Add(item);
        ctx.Player.Data.InventoryItemIds.Add(item.Id);
        await ctx.Send($"Vezmeš: {item.Name}. {item.Description}");

        // Check win condition
        if (item.IsWinCondition && !ctx.Player.Data.GameCompleted)
        {
            await ctx.ShowArt("crown");
            await TriggerWin(ctx);
        }

        await ctx.Store.Save(ctx.Player.Data);
    }

    private async Task TriggerWin(CommandContext ctx)
    {
        ctx.Player.Data.GameCompleted = true;
        ctx.Player.Data.CompletedAt = DateTime.Now;
        await ctx.Leaderboard.AddEntry(ctx.Player.Data.Name, ctx.Player.Data.Gold);

        string victory = ctx.Res.GetVictoryText();
        if (string.IsNullOrWhiteSpace(victory))
        {
            victory = $"\n  ★ ★ ★  VÍTĚZSTVÍ! — {ctx.Player.Data.Name} dobyl Soul Knight!  ★ ★ ★\n";
        }
        else
        {
            victory = victory
                .Replace("{name}", ctx.Player.Data.Name)
                .Replace("{gold}", ctx.Player.Data.Gold.ToString())
                .Replace("{time}", ctx.Player.Data.CompletedAt?.ToString("dd.MM.yyyy HH:mm") ?? "");
        }

        // Play victory animation
        await ctx.PlayArt("victory", 600);

        await ctx.Send(victory);
        await ctx.Sessions.BroadcastAll($"\n  *** {ctx.Player.Data.Name} porazil Temného rytíře a dokončil hru! ***\n");
        await ctx.Log.Info(ctx.Player.Data.Name, "HRA DOKONČENA.");
        await ctx.Store.Save(ctx.Player.Data);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// DROP — odlož / odloz / drop
// ══════════════════════════════════════════════════════════════════════════════

public class DropCommand : IGameCommand
{
    public string[] Aliases => ["odlož", "odloz", "drop"];
    public string Description => "Odlož předmět z inventáře";
    public string Usage => "odlož <předmět>";
    public CommandCategory Category => CommandCategory.Items;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) { await ctx.Send("Napiš: odlož <předmět>"); return; }

        string searchArg = arg.ToLower().RemoveDiacritics();
        var item = ctx.Player.Inventory.FirstOrDefault(i => i.Name.ToLower().RemoveDiacritics().Contains(searchArg));
        if (item == null)
        { await ctx.Send(ctx.Res.GetError("ItemNotInInventory").Replace("{name}", arg)); return; }

        ctx.Player.Inventory.Remove(item);
        ctx.Player.Data.InventoryItemIds.Remove(item.Id);
        ctx.World.GetRoom(ctx.Player.RoomId)?.Items.Add(item);
        await ctx.Send($"Odložíš {item.Name} na zem.");
        await ctx.Store.Save(ctx.Player.Data);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// INVENTORY — inventar / inventář / inv / i
// ══════════════════════════════════════════════════════════════════════════════

public class InventoryCommand : IGameCommand
{
    public string[] Aliases => ["inventar", "inventář", "inv", "i"];
    public string Description => "Zobraz inventář";
    public string Usage => "inventar / inv / i";
    public CommandCategory Category => CommandCategory.Items;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"╔══ INVENTÁŘ ({ctx.Player.UsedWeight()}/{ctx.Player.Data.MaxInventoryWeight} váha) — Zlaté: {ctx.Player.Data.Gold} ══");

        if (ctx.Player.Inventory.Count == 0)
            sb.AppendLine("│  Nic nemáš.");
        else
            foreach (var item in ctx.Player.Inventory)
            {
                string bonus = "";
                if (item.AttackBonus != 0) bonus += $" Útok+{item.AttackBonus}";
                if (item.DefenseBonus != 0) bonus += $" Obrana+{item.DefenseBonus}";
                if (item.Usable) bonus += " [použitelný]";
                sb.AppendLine($"│  • {item.Name} (váha:{item.Weight}){bonus}");
            }

        sb.AppendLine("╚" + new string('═', 50));
        await ctx.Send(sb.ToString());
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// USE ITEM — pouzij / použij / use
// ══════════════════════════════════════════════════════════════════════════════

public class UseItemCommand : IGameCommand
{
    public string[] Aliases => ["pouzij", "použij", "use"];
    public string Description => "Použij předmět z inventáře";
    public string Usage => "použij <předmět>";
    public CommandCategory Category => CommandCategory.Items;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) { await ctx.Send("Napiš: použij <předmět>"); return; }

        string searchArg = arg.ToLower().RemoveDiacritics();
        var item = ctx.Player.Inventory.FirstOrDefault(i => i.Name.ToLower().RemoveDiacritics().Contains(searchArg));
        if (item == null)
        { await ctx.Send(ctx.Res.GetError("ItemNotInInventory").Replace("{name}", arg)); return; }
        if (!item.Usable || item.UseEffect == null)
        { await ctx.Send(ctx.Res.GetError("ItemNotUsable").Replace("{item}", item.Name)); return; }

        var effect = item.UseEffect;
        await ctx.Send($"\n  Použiješ: {item.Name}");
        await ctx.Send($"  {effect.Message}");

        // Show contextual ASCII art based on effect type
        switch (effect.Type)
        {
            case "heal":
            case "heal_and_status":
                await ctx.ShowArt("potion");
                break;
            case "status":
            case "attack_boost_temp":
                await ctx.PlayArt("magic", 300);
                break;
        }

        switch (effect.Type)
        {
            case "heal":
                ctx.Player.Data.Hp = Math.Min(ctx.Player.Data.MaxHp, ctx.Player.Data.Hp + effect.Amount);
                await ctx.Send($"  HP: {ctx.Player.Data.Hp}/{ctx.Player.Data.MaxHp}");
                break;

            case "heal_and_status":
                ctx.Player.Data.Hp = Math.Min(ctx.Player.Data.MaxHp, ctx.Player.Data.Hp + effect.Amount);
                if (effect.StatusId != null) await ctx.ApplyStatus(effect.StatusId);
                break;

            case "status":
                if (effect.StatusId != null) await ctx.ApplyStatus(effect.StatusId);
                break;

            case "attack_boost_temp":
                string tempId = ctx.GuessStatusId(item.Id);
                if (!string.IsNullOrEmpty(tempId)) await ctx.ApplyStatus(tempId);
                break;

            case "gold":
                ctx.Player.Data.Gold += effect.Amount;
                await ctx.Send($"  Zlaté: {ctx.Player.Data.Gold}");
                break;

            case "info":
                // message already sent
                break;
        }

        if (item.Consumable)
        {
            ctx.Player.Inventory.Remove(item);
            ctx.Player.Data.InventoryItemIds.Remove(item.Id);
            await ctx.Send($"  (Předmět '{item.Name}' byl spotřebován.)");
        }

        await ctx.Store.Save(ctx.Player.Data);
    }
}
