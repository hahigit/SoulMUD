using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace SoulKnightMud.Server;

public class MudServer
{
    private readonly int _port;
    private readonly int _maxPlayers;
    private readonly bool _debugMode;
    private readonly int _autoSaveInterval;
    private readonly GameWorld _world;
    private readonly SessionManager _sessions;
    private readonly PlayerStore _store;
    private readonly GameLogger _log;
    private readonly Leaderboard _leaderboard;
    private readonly GameResources _resources;

    public MudServer(ServerConfig cfg)
    {
        _port = cfg.Port;
        _maxPlayers = cfg.MaxPlayers;
        _debugMode = cfg.DebugMode;
        _autoSaveInterval = cfg.AutoSaveIntervalSeconds;
        _log = new GameLogger(cfg.LogPath);
        _world = new GameWorld();
        _world.Load(cfg.DataPath);
        _sessions = new SessionManager();
        _store = new PlayerStore(cfg.PlayersPath);
        _leaderboard = new Leaderboard(cfg.LeaderboardPath);
        _resources = GameResources.Load(cfg.ResourcesPath);

        _log.Info("SERVER", $"Herní svět načten z '{cfg.DataPath}'.").Wait();
        _log.Info("SERVER", $"Resources načteny z '{cfg.ResourcesPath}'.").Wait();
        if (_debugMode)
            _log.Info("SERVER", "DEBUG režim aktivní.").Wait();

        // Vytvoření testovacích účtů podle TestCases.md
        SeedTestPlayers().Wait();
    }

    private async Task SeedTestPlayers()
    {
        if (!_store.Exists("test_player"))
        {
            await _store.CreateNew("test_player", "Test123");
            await _log.Info("SERVER", "Vytvořen testovací účet: test_player");
        }
        if (!_store.Exists("saved_player"))
        {
            var saved = await _store.CreateNew("saved_player", "Save123");
            saved.RoomId = "boss_komnata"; // Přesun do Boss room
            saved.Gold = 1000;
            await _store.Save(saved);
            await _log.Info("SERVER", "Vytvořen testovací účet: saved_player");
        }
    }

    public async Task RunAsync()
    {
        var listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        await _log.Info("SERVER", $"Nasloucháme na portu {_port} (max {_maxPlayers} hráčů)...");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        // Start auto-save timer
        _ = RunAutoSaveAsync(cts.Token);

        try
        {
            while (!cts.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(cts.Token);

                // Check max players
                if (_sessions.Count >= _maxPlayers)
                {
                    await RejectClient(client);
                    continue;
                }

                var session = new PlayerSession(
                    client, _world, _sessions, _store,
                    _log, _leaderboard, _resources, _debugMode);
                _ = Task.Run(() => session.HandleAsync(cts.Token));
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            listener.Stop();
            await _log.Info("SERVER", "Server zastaven.");
            _log.Dispose();
        }
    }

    private async Task RejectClient(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            using var writer = new StreamWriter(stream, System.Text.Encoding.UTF8) { AutoFlush = true };
            string msg = _resources.GetError("ServerFull").Replace("{max}", _maxPlayers.ToString());
            await writer.WriteLineAsync(msg);
            await _log.Info("SERVER", $"Odmítnuto připojení — server plný ({_sessions.Count}/{_maxPlayers}).");
        }
        catch { }
        finally { try { client.Close(); } catch { } }
    }

    private async Task RunAutoSaveAsync(CancellationToken ct)
    {
        if (_autoSaveInterval <= 0) return;

        await _log.Info("SERVER", $"Auto-save každých {_autoSaveInterval} sekund.");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_autoSaveInterval), ct);
                int count = await _sessions.SaveAllPlayers(_store);
                if (count > 0)
                    await _log.Info("SERVER", $"Auto-save: uloženo {count} hráčů.");
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                await _log.Error("SERVER", $"Chyba auto-save: {ex.Message}");
            }
        }
    }
}

public class ServerConfig
{
    public int Port { get; set; } = 4000;
    public int MaxPlayers { get; set; } = 20;
    public string DataPath { get; set; } = "Data/world";
    public string PlayersPath { get; set; } = "Data/players";
    public string LogPath { get; set; } = "logs/server.log";
    public string LeaderboardPath { get; set; } = "Data/leaderboard.json";
    public string ResourcesPath { get; set; } = "Data/resources.json";
    public bool DebugMode { get; set; } = false;
    public int AutoSaveIntervalSeconds { get; set; } = 300;
}
