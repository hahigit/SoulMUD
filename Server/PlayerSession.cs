using System.Net.Sockets;
using System.Text;

namespace SoulKnightMud.Server;

public class PlayerSession
{
    public Guid Id { get; } = Guid.NewGuid();
    public Player? Player { get; private set; }
    public bool IsAuthenticated => Player != null;

    private readonly TcpClient _client;
    private readonly GameWorld _world;
    private readonly SessionManager _sessions;
    private readonly PlayerStore _store;
    private readonly GameLogger _log;
    private readonly Leaderboard _leaderboard;
    private readonly GameResources _res;
    private readonly bool _debug;

    private StreamWriter? _writer;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public PlayerSession(TcpClient client, GameWorld world, SessionManager sessions,
        PlayerStore store, GameLogger log, Leaderboard leaderboard,
        GameResources resources, bool debugMode)
    {
        _client = client;
        _world = world;
        _sessions = sessions;
        _store = store;
        _log = log;
        _leaderboard = leaderboard;
        _res = resources;
        _debug = debugMode;
    }

    public Func<string, Task> SendAsync => msg => SendInternalAsync(msg);

    public async Task HandleAsync(CancellationToken ct)
    {
        string remote = _client.Client.RemoteEndPoint?.ToString() ?? "?";
        await _log.Info("NET", $"Připojení: {remote}");

        try
        {
            var stream = _client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = false };
            _writer = writer;

            await ShowBanner();

            // ── Auth loop ──────────────────────────────────────────────────────
            Player = await RunAuthLoop(reader, ct);
            if (Player == null) return;

            _sessions.Add(this);
            await _log.Auth(Player.Data.Name, $"Přihlášen z {remote}");

            // Restore inventory from saved IDs — filter out invalid/consumed items
            var validIds = new List<string>();
            foreach (var id in Player.Data.InventoryItemIds)
            {
                var item = _world.GetItem(id);
                if (item != null)
                {
                    Player.Inventory.Add(item);
                    validIds.Add(id);
                }
            }
            // Sync save data — remove any ghost item IDs that no longer exist
            if (validIds.Count != Player.Data.InventoryItemIds.Count)
            {
                Player.Data.InventoryItemIds = validIds;
                await _store.Save(Player.Data);
                await _log.Info(Player.Data.Name, "Inventář vyčištěn — odstraněny neplatné předměty.");
            }

            // Welcome message from resources
            string welcome = _res.WelcomeMessage
                .Replace("{name}", Player.Data.Name)
                .Replace("{room}", _world.GetRoom(Player.RoomId)?.Def.Title ?? Player.RoomId);
            await SendInternalAsync($"\n{welcome}");
            await SendInternalAsync($"HP: {Player.Data.Hp}/{Player.Data.MaxHp}  |  Zlaté: {Player.Data.Gold}");
            if (Player.Data.GameCompleted)
                await SendInternalAsync(_res.GameCompletedNote);
            await SendInternalAsync($"{_res.HelpTip}\n");

            // Notify room
            await _sessions.BroadcastAll($"  >> {Player.Data.Name} se přihlásil.");

            var handler = new CommandHandler(
                Player, _world, _sessions,
                SendAsync, _log, _store, _leaderboard, _res, _debug);

            await handler.ShowRoom();

            // ── Command loop ───────────────────────────────────────────────────
            while (!ct.IsCancellationRequested)
            {
                await SendRawAsync($"\n[{Player.Data.Name} | HP:{Player.Data.Hp}]> ");
                string? line = await reader.ReadLineAsync(ct);
                if (line == null) break;
                await handler.HandleAsync(line);
            }
        }
        catch (OperationCanceledException) { }
        catch (IOException) { }
        catch (Exception ex)
        {
            await _log.Error("SESSION", $"Chyba pro {Player?.Data.Name ?? remote}: {ex.Message}");
        }
        finally
        {
            if (Player != null)
            {
                await _store.Save(Player.Data);
                await _log.Info(Player.Data.Name, _res.DisconnectSaved);
                _sessions.Remove(Id);
                await _sessions.BroadcastAll($"  >> {Player.Data.Name} se odpojil.");
            }
            try { _client.Close(); } catch { }
        }
    }

    // ── AUTH ──────────────────────────────────────────────────────────────────

    private async Task<Player?> RunAuthLoop(StreamReader reader, CancellationToken ct)
    {
        while (true)
        {
            // Send login prompt from resources
            foreach (var line in _res.LoginPromptTitle.Split('\n'))
                await SendInternalAsync(line);
            await SendRawAsync(_res.PromptChoice);

            string? choice = await reader.ReadLineAsync(ct);
            if (choice == null) return null;

            switch (choice.Trim())
            {
                case "1":
                    return await LoginFlow(reader, ct);
                case "2":
                    return await RegisterFlow(reader, ct);
                default:
                    await SendInternalAsync(_res.GetError("InvalidChoice"));
                    break;
            }
        }
    }

    private async Task<Player?> LoginFlow(StreamReader reader, CancellationToken ct)
    {
        await SendRawAsync(_res.PromptName);
        string? name = await reader.ReadLineAsync(ct);
        if (name == null) return null;
        name = name.Trim();

        await SendRawAsync(_res.PromptPassword);
        string? password = await reader.ReadLineAsync(ct);
        if (password == null) return null;

        var data = await _store.Load(name);
        if (data == null || !_store.VerifyPassword(data, password.Trim()))
        {
            await SendInternalAsync(_res.GetError("BadCredentials"));
            await _log.Auth(name, "Neúspěšné přihlášení.");
            return null;
        }

        // Prevent duplicate login
        if (_sessions.IsPlayerOnline(name))
        {
            await SendInternalAsync("  Tento účet je již přihlášen z jiného klienta.");
            await _log.Auth(name, "Odmítnuto — duplicitní přihlášení.");
            return null;
        }

        return new Player(data);
    }

    private async Task<Player?> RegisterFlow(StreamReader reader, CancellationToken ct)
    {
        await SendRawAsync(_res.PromptNewName);
        string? name = await reader.ReadLineAsync(ct);
        if (name == null) return null;
        name = name.Trim();

        if (name.Length < 2 || name.Length > 20)
        { await SendInternalAsync(_res.GetError("NameLength")); return null; }

        if (_store.Exists(name))
        { await SendInternalAsync(_res.GetError("NameTaken").Replace("{name}", name)); return null; }

        await SendRawAsync(_res.PromptNewPassword);
        string? password = await reader.ReadLineAsync(ct);
        if (password == null) return null;
        password = password.Trim();

        if (password.Length < 4)
        { await SendInternalAsync(_res.GetError("PasswordLength")); return null; }

        var data = await _store.CreateNew(name, password);
        await SendInternalAsync($"  {_res.NewPlayerWelcome.Replace("{name}", name)}");
        await _log.Auth(name, "Nový účet vytvořen.");
        return new Player(data);
    }

    // ── SEND ──────────────────────────────────────────────────────────────────

    private async Task SendInternalAsync(string message)
    {
        await _writeLock.WaitAsync();
        try
        {
            if (_writer == null) return;
            await _writer.WriteLineAsync(message);
            await _writer.FlushAsync();
        }
        catch (IOException) { }
        finally { _writeLock.Release(); }
    }

    private async Task SendRawAsync(string message)
    {
        await _writeLock.WaitAsync();
        try
        {
            if (_writer == null) return;
            await _writer.WriteAsync(message);
            await _writer.FlushAsync();
        }
        catch (IOException) { }
        finally { _writeLock.Release(); }
    }

    private async Task ShowBanner()
    {
        string banner = _res.GetBannerText();
        if (string.IsNullOrWhiteSpace(banner))
        {
            // Fallback hardcoded banner
            await SendInternalAsync("=== SOUL KNIGHT MUD ===");
        }
        else
        {
            await SendInternalAsync(banner);
        }
    }
}
