namespace SoulKnightMud.Server.Commands;

/// <summary>
/// Command pattern interface — each game command implements this.
/// </summary>
public interface IGameCommand
{
    /// <summary>All recognized aliases for this command (lowercase).</summary>
    string[] Aliases { get; }

    /// <summary>Short description shown in help.</summary>
    string Description { get; }

    /// <summary>Usage syntax shown in help (e.g. "jdi &lt;směr&gt;").</summary>
    string Usage { get; }

    /// <summary>Category for grouping in help output.</summary>
    CommandCategory Category { get; }

    /// <summary>Execute the command with the given argument and context.</summary>
    Task ExecuteAsync(string arg, CommandContext ctx);
}

public enum CommandCategory
{
    Navigation,
    Items,
    Combat,
    Trade,
    Social,
    Other
}
