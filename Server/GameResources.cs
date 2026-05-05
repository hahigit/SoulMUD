using System.Text.Json;

namespace SoulKnightMud.Server;

/// <summary>
/// Loads UI texts, banners, prompts and error messages from an external resources JSON file.
/// These are application-level content (not game data) and can be changed without recompilation.
/// </summary>
public class GameResources
{
    private static readonly JsonSerializerOptions _opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ── Properties loaded from resources.json ────────────────────────────────

    public string GameTitle { get; set; } = "Soul Knight MUD";
    public List<string> Banner { get; set; } = new();
    public string WelcomeMessage { get; set; } = "Vítej zpět, {name}! Jsi v: {room}";
    public string NewPlayerWelcome { get; set; } = "Účet '{name}' vytvořen! Dobrodružství začíná...";
    public string LoginPromptTitle { get; set; } = "  [1] Přihlásit se\n  [2] Vytvořit nový účet";
    public string PromptChoice { get; set; } = "  Volba: ";
    public string PromptName { get; set; } = "  Jméno: ";
    public string PromptPassword { get; set; } = "  Heslo: ";
    public string PromptNewName { get; set; } = "  Nové jméno (2–20 znaků): ";
    public string PromptNewPassword { get; set; } = "  Heslo (min. 4 znaky): ";
    public string HelpTip { get; set; } = "Napiš 'pomoc' pro nápovědu.";
    public string GameCompletedNote { get; set; } = "  (Hru jsi již dokončil — legenda žije dál!)";
    public string DisconnectSaved { get; set; } = "Odpojen. Stav uložen.";
    public Dictionary<string, string> ErrorMessages { get; set; } = new();
    public Dictionary<string, string> CombatMessages { get; set; } = new();
    public List<string> VictoryBanner { get; set; } = new();

    // ── Helpers ──────────────────────────────────────────────────────────────

    public string GetBannerText() => string.Join("\n", Banner);

    public string GetError(string key)
    {
        return ErrorMessages.TryGetValue(key, out var msg) ? msg : $"[ERR] {key}";
    }

    public string GetCombat(string key)
    {
        return CombatMessages.TryGetValue(key, out var msg) ? msg : key;
    }

    public string GetVictoryText() => string.Join("\n", VictoryBanner);

    // ── Factory ─────────────────────────────────────────────────────────────

    public static GameResources Load(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"[WARN] Resources soubor '{path}' nenalezen — použijí se výchozí texty.");
            return new GameResources();
        }

        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        return JsonSerializer.Deserialize<GameResources>(json, _opts) ?? new GameResources();
    }
}
