using System.Text;

namespace SoulKnightMud.Server.Commands;

/// <summary>
/// Replaces the switch-case — auto-registers all IGameCommand implementations
/// and dispatches input to the matching command.
/// </summary>
public class CommandDispatcher
{
    private readonly Dictionary<string, IGameCommand> _commands = new();
    private readonly List<IGameCommand> _allCommands = new();
    private readonly CommandContext _ctx;

    public CommandDispatcher(CommandContext ctx)
    {
        _ctx = ctx;

        // Register all command implementations
        Register(new LookCommand());
        Register(new GoCommand());
        Register(new TakeCommand());
        Register(new DropCommand());
        Register(new InventoryCommand());
        Register(new UseItemCommand());
        Register(new TalkCommand());
        Register(new AttackCommand());
        Register(new StatusCommand());
        Register(new ShopCommand());
        Register(new BuyCommand());
        Register(new SellCommand());
        Register(new GoldCommand());
        Register(new SayCommand());
        Register(new ShoutCommand());
        Register(new WhisperCommand());
        Register(new OnlineCommand());
        Register(new LeaderboardCommand());
        Register(new HelpCommand(this));
    }

    private void Register(IGameCommand cmd)
    {
        _allCommands.Add(cmd);
        foreach (var alias in cmd.Aliases)
        {
            string cleanAlias = alias.ToLower().RemoveDiacritics();
            _commands[cleanAlias] = cmd;
        }
    }

    public IReadOnlyList<IGameCommand> AllCommands => _allCommands;

    /// <summary>Execute ShowRoom — used on login and by GoCommand.</summary>
    public async Task ShowRoom() => await new LookCommand().ExecuteAsync("", _ctx);

    public async Task HandleAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        string trimmed = input.Trim();
        string lower = trimmed.ToLower().RemoveDiacritics();
        int spaceIdx = lower.IndexOf(' ');
        string cmd = spaceIdx >= 0 ? lower[..spaceIdx] : lower;
        string arg = spaceIdx >= 0 ? trimmed[(spaceIdx + 1)..].Trim() : "";

        await _ctx.Log.Info(_ctx.Player.Data.Name, $"CMD: {trimmed}");

        // Tick status effects each command
        await _ctx.TickStatusEffects();

        // Tick NPC respawns globally and notify rooms
        var respawned = _ctx.World.TickNpcRespawns();
        foreach (var (roomId, npcName) in respawned)
        {
            await _ctx.BroadcastRoom(roomId, $"\n  >> {npcName} se znovu objevuje!");
        }

        if (_commands.TryGetValue(cmd, out var command))
        {
            await command.ExecuteAsync(arg, _ctx);
        }
        else
        {
            await _ctx.Send(_ctx.Res.GetError("UnknownCommand").Replace("{cmd}", cmd));
        }
    }
}
