using System.Text;

namespace SoulKnightMud.Server.Commands;

// ══════════════════════════════════════════════════════════════════════════════
// HELP — pomoc / help / ?
// ══════════════════════════════════════════════════════════════════════════════

public class HelpCommand : IGameCommand
{
    private readonly CommandDispatcher _dispatcher;

    public HelpCommand(CommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public string[] Aliases => ["pomoc", "help", "?"];
    public string Description => "Zobraz nápovědu";
    public string Usage => "pomoc / help / ?";
    public CommandCategory Category => CommandCategory.Other;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("╔══════════════════════════════════════════════════╗");
        sb.AppendLine($"║         {ctx.Res.GameTitle} — NÁPOVĚDA              ║");
        sb.AppendLine("╠══════════════════════════════════════════════════╣");

        // Group commands by category
        var grouped = _dispatcher.AllCommands
            .GroupBy(c => c.Category)
            .OrderBy(g => g.Key);

        foreach (var group in grouped)
        {
            string categoryName = group.Key switch
            {
                CommandCategory.Navigation => "POHYB",
                CommandCategory.Items      => "PŘEDMĚTY",
                CommandCategory.Combat     => "POSTAVY & BOJ",
                CommandCategory.Trade      => "OBCHOD",
                CommandCategory.Social     => "KOMUNIKACE",
                CommandCategory.Other      => "OSTATNÍ",
                _                          => group.Key.ToString()
            };

            sb.AppendLine($"║ {categoryName,-48} ║");

            foreach (var cmd in group)
            {
                string line = $"║  {cmd.Usage,-22} — {cmd.Description,-22} ║";
                sb.AppendLine(line);
            }

            sb.AppendLine("╠══════════════════════════════════════════════════╣");
        }

        // Replace last separator with bottom border
        string result = sb.ToString();
        int lastSep = result.LastIndexOf("╠══");
        if (lastSep >= 0)
            result = result[..lastSep] + "╚══════════════════════════════════════════════════╝\n";

        await ctx.Send(result);
    }
}

// ══════════════════════════════════════════════════════════════════════════════
// LEADERBOARD — zebricek / žebříček / top / leaderboard
// ══════════════════════════════════════════════════════════════════════════════

public class LeaderboardCommand : IGameCommand
{
    public string[] Aliases => ["zebricek", "žebříček", "top", "leaderboard"];
    public string Description => "Síň slávy";
    public string Usage => "žebříček / top";
    public CommandCategory Category => CommandCategory.Other;

    public async Task ExecuteAsync(string arg, CommandContext ctx)
    {
        var entries = await ctx.Leaderboard.Load();
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("╔══ SÍŇ SLÁVY — PORAZITELÉ TEMNÉHO RYTÍŘE ══");
        if (entries.Count == 0)
            sb.AppendLine("│  Zatím nikdo. Budeš první?");
        else
        {
            int rank = 1;
            foreach (var e in entries.OrderBy(x => x.CompletedAt))
                sb.AppendLine($"│  {rank++,2}. {e.PlayerName,-20} {e.CompletedAt:dd.MM.yyyy HH:mm}  {e.GoldAtCompletion} zlatých");
        }
        sb.AppendLine("╚" + new string('═', 46));
        await ctx.Send(sb.ToString());
    }
}
