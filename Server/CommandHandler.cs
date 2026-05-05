using SoulKnightMud.Server.Commands;

namespace SoulKnightMud.Server;

/// <summary>
/// Thin wrapper that creates CommandContext and CommandDispatcher.
/// Kept for backwards compatibility with PlayerSession.
/// </summary>
public class CommandHandler
{
    private readonly CommandDispatcher _dispatcher;

    public CommandHandler(
        Player player, GameWorld world, SessionManager sessions,
        Func<string, Task> send, GameLogger log,
        PlayerStore store, Leaderboard leaderboard,
        GameResources res, bool debug)
    {
        var ctx = new CommandContext(
            player, world, sessions, send,
            log, store, leaderboard, res, debug);
        _dispatcher = new CommandDispatcher(ctx);
    }

    public Task HandleAsync(string input) => _dispatcher.HandleAsync(input);
    public Task ShowRoom() => _dispatcher.ShowRoom();
}
