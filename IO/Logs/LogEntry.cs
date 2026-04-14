namespace KB.SharpCore.IO.Logs;

public readonly record struct LogEntry(
    long Sequence,
    DateTime TimestampUtc,
    ELogLevel Level,
    string Category,
    string Message,
    string? Source)
{
    public DateTimeOffset Timestamp => new(TimestampUtc, TimeSpan.Zero);
}