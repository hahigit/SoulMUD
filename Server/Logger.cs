namespace SoulKnightMud.Server;

public enum LogLevel { Info, Warning, Error, Combat, Auth }

public class GameLogger : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public GameLogger(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        _writer = new StreamWriter(path, append: true, System.Text.Encoding.UTF8)
        {
            AutoFlush = false
        };
        _ = Log(LogLevel.Info, "SERVER", "Server spuštěn.");
    }

    public async Task Log(LogLevel level, string context, string message)
    {
        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level,-7}] [{context}] {message}";
        Console.WriteLine(line);

        await _lock.WaitAsync();
        try
        {
            await _writer.WriteLineAsync(line);
            await _writer.FlushAsync();
        }
        finally { _lock.Release(); }
    }

    public Task Info(string context, string msg) => Log(LogLevel.Info, context, msg);
    public Task Warn(string context, string msg) => Log(LogLevel.Warning, context, msg);
    public Task Error(string context, string msg) => Log(LogLevel.Error, context, msg);
    public Task Combat(string context, string msg) => Log(LogLevel.Combat, context, msg);
    public Task Auth(string context, string msg) => Log(LogLevel.Auth, context, msg);

    public void Dispose()
    {
        _writer.Flush();
        _writer.Dispose();
    }
}
