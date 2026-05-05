using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SoulKnightMud.Client;

class Program
{
    // ── State ─────────────────────────────────────────────────────────────────
    static readonly List<string> _history = new();
    static int _historyIndex = -1;
    static readonly StringBuilder _inputBuffer = new();
    static int _cursorCol = 0;

    static async Task Main(string[] args)
    {
        // Load client config
        var cfg = LoadClientConfig();

        string host = cfg.Host;
        int    port = cfg.Port;

        // CLI args override config
        if (args.Length >= 1) host = args[0];
        if (args.Length >= 2 && int.TryParse(args[1], out int p)) port = p;

        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding  = Encoding.UTF8;
        Console.Title          = cfg.AppName;

        ShowClientBanner(host, port, cfg.AppName);

        TcpClient? tcp = null;
        try
        {
            tcp = new TcpClient();
            if (cfg.TimeoutSeconds > 0)
                tcp.ReceiveTimeout = cfg.TimeoutSeconds * 1000;
            await tcp.ConnectAsync(host, port);
        }
        catch (Exception ex)
        {
            WriteError($"Nelze se připojit na {host}:{port} — {ex.Message}");
            WriteInfo("Stiskni Enter pro ukončení.");
            Console.ReadLine();
            return;
        }

        WriteSuccess($"Připojeno na {host}:{port}");

        var stream = tcp.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = false };

        var cts = new CancellationTokenSource();

        // ── Receive task ──────────────────────────────────────────────────────
        var receiveTask = Task.Run(async () =>
        {
            try
            {
                char[] buf = new char[4096];
                while (!cts.Token.IsCancellationRequested)
                {
                    int n = await reader.ReadAsync(buf, 0, buf.Length);
                    if (n == 0) break;

                    string chunk = new string(buf, 0, n);
                    PrintServerChunk(chunk);
                }
            }
            catch (IOException) { }
            catch (ObjectDisposedException) { }
            finally { cts.Cancel(); }
        }, cts.Token);

        // ── Send loop ─────────────────────────────────────────────────────────
        try
        {
            while (!cts.IsCancellationRequested)
            {
                string? line = await ReadLineAsync(cts.Token);
                if (line == null || cts.IsCancellationRequested) break;

                // Local client commands
                if (line.Trim().ToLower() is "/exit" or "/quit" or "/konec")
                {
                    WriteInfo("Odpojuji se...");
                    break;
                }
                if (line.Trim().ToLower() == "/help")
                {
                    ShowClientHelp();
                    continue;
                }
                if (line.Trim().ToLower() == "/clear")
                {
                    Console.Clear();
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    await writer.WriteLineAsync(line);
                    await writer.FlushAsync();
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            cts.Cancel();
            tcp.Close();
            WriteInfo("Spojení ukončeno. Na shledanou, dobrodruhu!");
            await Task.Delay(800);
        }

        await receiveTask.WaitAsync(TimeSpan.FromSeconds(2)).ContinueWith(_ => { });
    }

    // ── Client config ─────────────────────────────────────────────────────────

    static ClientConfig LoadClientConfig()
    {
        string path = "client_settings.json";
        if (!File.Exists(path))
            return new ClientConfig();

        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ClientConfig>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new ClientConfig();
        }
        catch
        {
            return new ClientConfig();
        }
    }

    // ── Async readline with history and arrow keys ────────────────────────────

    static async Task<string?> ReadLineAsync(CancellationToken ct)
    {
        _inputBuffer.Clear();
        _historyIndex = _history.Count;
        _cursorCol = 0;

        return await Task.Run(() =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

                var key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                    {
                        string line = _inputBuffer.ToString();
                        Console.WriteLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            _history.Add(line);
                            if (_history.Count > 50) _history.RemoveAt(0);
                        }
                        return line;
                    }

                    case ConsoleKey.Backspace:
                        if (_inputBuffer.Length > 0 && _cursorCol > 0)
                        {
                            _inputBuffer.Remove(_inputBuffer.Length - 1, 1);
                            _cursorCol--;
                            Console.Write("\b \b");
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        if (_history.Count > 0 && _historyIndex > 0)
                        {
                            _historyIndex--;
                            ReplaceInput(_history[_historyIndex]);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (_historyIndex < _history.Count - 1)
                        {
                            _historyIndex++;
                            ReplaceInput(_history[_historyIndex]);
                        }
                        else
                        {
                            _historyIndex = _history.Count;
                            ReplaceInput("");
                        }
                        break;

                    case ConsoleKey.Escape:
                        ReplaceInput("");
                        break;

                    default:
                        if (key.KeyChar >= ' ')
                        {
                            _inputBuffer.Append(key.KeyChar);
                            _cursorCol++;
                            Console.Write(key.KeyChar);
                        }
                        break;
                }
            }
            return null;
        }, ct);
    }

    static void ReplaceInput(string newText)
    {
        // Clear current input visually
        int len = _inputBuffer.Length;
        Console.Write(new string('\b', len));
        Console.Write(new string(' ', len));
        Console.Write(new string('\b', len));

        _inputBuffer.Clear();
        _inputBuffer.Append(newText);
        _cursorCol = newText.Length;
        Console.Write(newText);
    }

    // ── Server output printer ─────────────────────────────────────────────────

    static void PrintServerChunk(string chunk)
    {
        // Print line by line with color coding
        foreach (var line in chunk.Split('\n'))
        {
            string trimmed = line.TrimEnd('\r');

            if (trimmed.StartsWith("╔") || trimmed.StartsWith("╠") ||
                trimmed.StartsWith("╚") || trimmed.StartsWith("║"))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  [VÝCHODY]"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  [PŘEDMĚTY]"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  [POSTAVY]") || trimmed.StartsWith("  [HRÁČI]"))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  [PORAŽENI]"))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.Contains("HP:") && trimmed.Contains("]>"))
            {
                // Prompt line
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("⚔") || trimmed.StartsWith("─"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.Contains("VÍTĚZSTVÍ") || trimmed.Contains("★"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  >>"))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  ***"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.Contains("🔒"))
            {
                // Whisper / private message
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  ⚠"))
            {
                // Room status effect warning
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  [") && trimmed.Contains("říká"))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else if (trimmed.StartsWith("  [ERR]") || trimmed.StartsWith("[ERR]"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(trimmed);
                Console.ResetColor();
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(trimmed);
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static void WriteError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[CHYBA] {msg}");
        Console.ResetColor();
    }

    static void WriteInfo(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"[INFO] {msg}");
        Console.ResetColor();
    }

    static void WriteSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK] {msg}");
        Console.ResetColor();
    }

    static void ShowClientBanner(string host, int port, string appName)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"""
        ╔══════════════════════════════════════════════════╗
        ║       {appName,-39} ║
        ╚══════════════════════════════════════════════════╝
        """);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  Server: {host}:{port}");
        Console.WriteLine("  /help  — nápověda klienta");
        Console.WriteLine("  /clear — vyčistit obrazovku");
        Console.WriteLine("  /exit  — odpojit se");
        Console.WriteLine("  ↑↓     — historie příkazů");
        Console.WriteLine();
        Console.ResetColor();
    }

    static void ShowClientHelp()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("""
        ╔══ NÁPOVĚDA KLIENTA ══
        ║  /help   — tato nápověda
        ║  /clear  — vyčistit obrazovku
        ║  /exit   — odpojit se od serveru
        ║  ↑ / ↓   — procházet historii příkazů
        ║  Escape  — vymazat aktuální vstup
        ╚══════════════════════════════════════
        Herní příkazy — napiš 'pomoc' po přihlášení.
        """);
        Console.ResetColor();
    }
}

// ── Client config model ───────────────────────────────────────────────────────

public class ClientConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 4000;
    public string AppName { get; set; } = "Soul Knight MUD — Klient";
    public int TimeoutSeconds { get; set; } = 30;
    public bool EnableLogging { get; set; } = false;
}
