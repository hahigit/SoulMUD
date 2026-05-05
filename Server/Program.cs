using System.Text.Json;
using SoulKnightMud.Server;

var cfgPath = "appsettings.json";
ServerConfig cfg;

if (File.Exists(cfgPath))
{
    var json = await File.ReadAllTextAsync(cfgPath);
    cfg = JsonSerializer.Deserialize<ServerConfig>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? new ServerConfig();
}
else
{
    cfg = new ServerConfig();
    Console.WriteLine($"[WARN] {cfgPath} nenalezen, používám výchozí nastavení.");
}

// Allow port override via CLI arg
if (args.Length > 0 && int.TryParse(args[0], out int cliPort))
    cfg.Port = cliPort;

Console.WriteLine("=== SOUL KNIGHT MUD SERVER ===");
var server = new MudServer(cfg);
await server.RunAsync();
