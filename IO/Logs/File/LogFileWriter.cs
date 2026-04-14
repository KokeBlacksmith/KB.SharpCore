using System.Text;

namespace KB.SharpCore.IO.Logs.File;

public sealed class LogFileWriter : IDisposable, IAsyncDisposable
{
    public LogFileWriter(LogHub hub, KB.SharpCore.IO.Path path, HumanReadableLogEntryFormatter formatter)
    {
        ArgumentNullException.ThrowIfNull(hub);
        ArgumentNullException.ThrowIfNull(formatter);

        _path = path;
        _formatter = formatter;
        _subscription = hub.Subscribe(WriteBatch);
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    private void WriteBatch(IReadOnlyList<LogEntry> batch)
    {
        if (batch.Count == 0)
        {
            return;
        }

        string? directory = System.IO.Path.GetDirectoryName(_path.StringPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        StringBuilder builder = new();
        foreach (LogEntry entry in batch)
        {
            builder.AppendLine(_formatter.Format(entry));
        }

        System.IO.File.AppendAllText(_path.StringPath, builder.ToString());
    }

    private readonly IDisposable _subscription;
    private readonly KB.SharpCore.IO.Path _path;
    private readonly HumanReadableLogEntryFormatter _formatter;
}