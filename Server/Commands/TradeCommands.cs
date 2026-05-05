using System.Text;

namespace SoulKnightMud.Server.Commands;

// ══════════════════════════════════════════════════════════════════════════════
// SHOP — obchod / shop
// ══════════════════════════════════════════════════════════════════════════════

public class ShopCommand : IGameCommand
{
    public string[] Aliases => ["obchod", "shop"];
    public string Description => "Zobraz nabídku obchodníka";
    public string Usage => "obchod [jméno]";
    public CommandCategory Category => CommandCategory.Trade;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        if (room == null) return;

        string searchArg = arg.ToLower().RemoveDiacritics();
        NpcInstance? shopkeeper = string.IsNullOrWhiteSpace(searchArg)
            ? room.Npcs.FirstOrDefault(n => n.Def.IsShop)
            : room.Npcs.FirstOrDefault(n => n.Def.Name.ToLower().RemoveDiacritics().Contains(searchArg) && n.Def.IsShop);

        if (shopkeeper == null) { await ctx.Send(ctx.Res.GetError("NoShopkeeper")); return; }

        var shop = ctx.World.GetShop(shopkeeper.Def.ShopId!);
        if (shop == null) { await ctx.Send(ctx.Res.GetError("ShopEmpty")); return; }

        // Show shop art
        await ctx.ShowArt("shop");

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"╔══ {shop.Name.ToUpper()} ══");
        sb.AppendLine($"│  Tvé zlaté: {ctx.Player.Data.Gold}");
        sb.AppendLine("│");
        sb.AppendLine("│  Položka                    Cena");
        sb.AppendLine("│  ─────────────────────────────────");
        foreach (var si in shop.Items)
        {
            var item = ctx.World.GetItem(si.ItemId);
            if (item == null) continue;
            sb.AppendLine($"│  {item.Name,-27} {si.Price} zlatých");
        }
        sb.AppendLine("│");
        sb.AppendLine("│  nakup <předmět> | prodej <předmět>");
        sb.AppendLine("╚" + new string('═', 42));
        await ctx.Send(sb.ToString());
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// BUY — nakup / nákup / buy
// ══════════════════════════════════════════════════════════════════════════════

public class BuyCommand : IGameCommand
{
    public string[] Aliases => ["nakup", "nákup", "buy"];
    public string Description => "Kup předmět od obchodníka";
    public string Usage => "nakup <předmět>";
    public CommandCategory Category => CommandCategory.Trade;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) { await ctx.Send("Napiš: nakup <předmět>"); return; }

        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        var shopkeeper = room?.Npcs.FirstOrDefault(n => n.Def.IsShop && n.IsAlive);
        if (shopkeeper == null) { await ctx.Send(ctx.Res.GetError("NoShopkeeper")); return; }

        var shop = ctx.World.GetShop(shopkeeper.Def.ShopId!);
        if (shop == null) return;

        string searchArg = arg.ToLower().RemoveDiacritics();
        var si = shop.Items.FirstOrDefault(x =>
        {
            var i = ctx.World.GetItem(x.ItemId);
            return i != null && i.Name.ToLower().RemoveDiacritics().Contains(searchArg);
        });

        if (si == null)
        { await ctx.Send(ctx.Res.GetError("ShopItemNotFound").Replace("{name}", shopkeeper.Def.Name)); return; }

        var item = ctx.World.GetItem(si.ItemId)!;
        if (ctx.Player.Data.Gold < si.Price)
        {
            await ctx.Send(ctx.Res.GetError("NotEnoughGold")
                .Replace("{needed}", si.Price.ToString())
                .Replace("{have}", ctx.Player.Data.Gold.ToString()));
            return;
        }
        if (!ctx.Player.CanCarry(item))
        { await ctx.Send(ctx.Res.GetError("InventoryTooHeavy")); return; }

        ctx.Player.Data.Gold -= si.Price;
        ctx.Player.Inventory.Add(item);
        ctx.Player.Data.InventoryItemIds.Add(item.Id);
        await ctx.Send($"  Kupuješ {item.Name} za {si.Price} zlatých. Zbývá: {ctx.Player.Data.Gold} zlatých.");
        await ctx.Store.Save(ctx.Player.Data);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// SELL — prodej / sell
// ══════════════════════════════════════════════════════════════════════════════

public class SellCommand : IGameCommand
{
    public string[] Aliases => ["prodej", "sell"];
    public string Description => "Prodej předmět obchodníkovi";
    public string Usage => "prodej <předmět>";
    public CommandCategory Category => CommandCategory.Trade;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) { await ctx.Send("Napiš: prodej <předmět>"); return; }

        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        var shopkeeper = room?.Npcs.FirstOrDefault(n => n.Def.IsShop && n.IsAlive);
        if (shopkeeper == null) { await ctx.Send(ctx.Res.GetError("NoShopkeeper")); return; }

        string searchArg = arg.ToLower().RemoveDiacritics();
        var item = ctx.Player.Inventory.FirstOrDefault(i => i.Name.ToLower().RemoveDiacritics().Contains(searchArg));
        if (item == null)
        { await ctx.Send(ctx.Res.GetError("ItemNotInInventory").Replace("{name}", arg)); return; }
        if (item.IsWinCondition)
        { await ctx.Send(ctx.Res.GetError("CannotSellQuest")); return; }

        int sellPrice = item.Value / 2;
        ctx.Player.Inventory.Remove(item);
        ctx.Player.Data.InventoryItemIds.Remove(item.Id);
        ctx.Player.Data.Gold += sellPrice;
        await ctx.Send($"  Prodáš {item.Name} za {sellPrice} zlatých. Celkem: {ctx.Player.Data.Gold} zlatých.");
        await ctx.Store.Save(ctx.Player.Data);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// GOLD — zlaté / zlate / gold / penize / peníze
// ══════════════════════════════════════════════════════════════════════════════

public class GoldCommand : IGameCommand
{
    public string[] Aliases => ["zlaté", "zlate", "gold", "penize", "peníze"];
    public string Description => "Zobraz aktuální zlaté";
    public string Usage => "zlaté / gold";
    public CommandCategory Category => CommandCategory.Trade;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        await ctx.Send($"  Máš {ctx.Player.Data.Gold} zlatých.");
    }
}
