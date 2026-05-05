using System.Text;

namespace SoulKnightMud.Server.Commands;

// ══════════════════════════════════════════════════════════════════════════════
// ATTACK — utoc / útoc / attack / bojuj
// ══════════════════════════════════════════════════════════════════════════════

public class AttackCommand : IGameCommand
{
    public string[] Aliases => ["utoc", "útoc", "attack", "bojuj"];
    public string Description => "Zaútoč na NPC";
    public string Usage => "utoc <jméno>";
    public CommandCategory Category => CommandCategory.Combat;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(arg)) { await ctx.Send("Napiš: utoc <jméno>"); return; }

        var room = ctx.World.GetRoom(ctx.Player.RoomId);
        if (room == null) return;

        string searchArg = arg.ToLower().RemoveDiacritics();
        var npc = room.Npcs.FirstOrDefault(n => n.Def.Name.ToLower().RemoveDiacritics().Contains(searchArg) && n.IsAlive);

        if (npc == null)
        { await ctx.Send(ctx.Res.GetError("NoCombatTarget").Replace("{name}", arg)); return; }

        if (!npc.Def.IsCombatant)
        { await ctx.Send(ctx.Res.GetError("NpcNotCombatant").Replace("{name}", npc.Def.Name)); return; }

        // Play attack animation
        if (npc.Def.Name.ToLower().Contains("sliz"))
            await ctx.PlayArt("slime", 350);
        else
            await ctx.PlayArt("sword_attack", 300);

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"⚔  BOJ: {ctx.Player.Data.Name} vs {npc.Def.Name}");
        sb.AppendLine("─────────────────────────────────────");

        // Player attacks NPC
        int playerDmg = Math.Max(1, ctx.Player.TotalAttack - npc.Def.Defense + ctx.RollVariance());
        npc.CurrentHp -= playerDmg;
        sb.AppendLine($"  → Zaútočíš na {npc.Def.Name} za {playerDmg} poškození. (NPC HP: {Math.Max(0, npc.CurrentHp)}/{npc.Def.MaxHp})");

        if (npc.CurrentHp <= 0)
        {
            npc.CurrentHp = 0;
            sb.AppendLine($"\n  ✓ {npc.Def.Name} je poražen!");
            sb.AppendLine($"  Získáváš {npc.Def.GoldReward} zlatých.");
            ctx.Player.Data.Gold += npc.Def.GoldReward;

            if (npc.Def.ItemReward != null)
            {
                var reward = ctx.World.GetItem(npc.Def.ItemReward);
                if (reward != null)
                {
                    room.Items.Add(reward);
                    sb.AppendLine($"  {npc.Def.Name} upustil: {reward.Name}");
                }
            }

            // Play defeat animation
            if (npc.Def.IsBoss)
            {
                await ctx.Send(sb.ToString());
                sb.Clear();
                await ctx.PlayArt("boss_defeat", 500);
                sb.AppendLine("\n  ★ BOSS PORAŽEN! Koruna stínu leží na trůně... Vezmi ji!");
            }
            else
            {
                await ctx.PlayArt("npc_defeat", 300);
            }

            await ctx.Log.Combat(ctx.Player.Data.Name, $"Porazil NPC '{npc.Def.Name}'.");
        }
        else
        {
            // NPC counterattack
            int npcDmg = Math.Max(1, npc.Def.Attack - ctx.Player.TotalDefense + ctx.RollVariance());
            ctx.Player.Data.Hp -= npcDmg;
            sb.AppendLine($"  ← {npc.Def.Name} protiútočí za {npcDmg} poškození. (Tvoje HP: {ctx.Player.Data.Hp}/{ctx.Player.Data.MaxHp})");

            // Chance of boss applying status
            if (npc.Def.IsBoss && ctx.Roll(0, 100) < 30)
            {
                string[] bossEffects = ["poison", "weakened"];
                string eff = bossEffects[ctx.Roll(0, bossEffects.Length)];
                await ctx.ApplyStatus(eff);
                sb.AppendLine($"  ! Temný rytíř tě zasáhl kouzlem! Status: {eff}");
            }

            if (ctx.Player.Data.Hp <= 0)
            {
                ctx.Player.Data.Hp = 1;
                await ctx.Send(sb.ToString());
                sb.Clear();
                await ctx.ShowArt("player_ko");
                sb.AppendLine("\n  ✗ Upadáš do bezvědomí! Vzpamatováváš se s 1 HP.");
                sb.AppendLine("  Útěk z boje nutný — napiš 'jdi <směr>'!");
            }
        }

        sb.AppendLine("─────────────────────────────────────");
        await ctx.Send(sb.ToString());
        await ctx.Store.Save(ctx.Player.Data);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// STATUS — zdravi / zdraví / hp / status
// ══════════════════════════════════════════════════════════════════════════════

public class StatusCommand : IGameCommand
{
    public string[] Aliases => ["zdravi", "zdraví", "hp", "status"];
    public string Description => "Zobraz stav hrdiny a aktivní statusy";
    public string Usage => "zdraví / hp / status";
    public CommandCategory Category => CommandCategory.Combat;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        var d = ctx.Player.Data;
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"╔══ STAV: {d.Name} ══");
        sb.AppendLine($"│  HP:     {d.Hp}/{d.MaxHp}");
        sb.AppendLine($"│  Útok:   {ctx.Player.TotalAttack} (základ: {d.BaseAttack})");
        sb.AppendLine($"│  Obrana: {ctx.Player.TotalDefense} (základ: {d.BaseDefense})");
        sb.AppendLine($"│  Zlaté:  {d.Gold}");

        if (ctx.Player.StatusEffects.Count > 0)
        {
            sb.AppendLine("│  [STATUSY]");
            foreach (var se in ctx.Player.StatusEffects)
                sb.AppendLine($"│    • {se.Def.Name} (zbývá: {se.TurnsRemaining} tahů)");
        }

        sb.AppendLine("╚" + new string('═', 40));
        await ctx.Send(sb.ToString());
    }
}
