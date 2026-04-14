namespace KB.SharpCore.IO.Logs;

public sealed class LogHub : IDisposable, IAsyncDisposable
{
    public LogHub(int capacity = 10_000)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _capacity = capacity;
    }

    public void Start()
    {
    }

    public void StartStdCapture(bool tryParseWarning = false)
    {
    }

    public void AddLog(ELogLevel level, string category, string message, string? source = null)
    {
        LogEntry entry = new(
            Sequence: Interlocked.Increment(ref _sequence),
            TimestampUtc: DateTime.UtcNow,
            Level: level,
            Category: category,
            Message: message,
            Source: source);

        Action<IReadOnlyList<LogEntry>>[] subscribers;
        lock (_gate)
        {
            _entries.Add(entry);
            if (_entries.Count > _capacity)
            {
                _entries.RemoveAt(0);
            }

            subscribers = _subscribers.ToArray();
        }

        if (subscribers.Length == 0)
        {
            return;
        }

        IReadOnlyList<LogEntry> batch = [entry];
        foreach (Action<IReadOnlyList<LogEntry>> subscriber in subscribers)
        {
            subscriber(batch);
        }
    }

    public LogEntry[] GetSnapshot()
    {
        lock (_gate)
        {
            return _entries.ToArray();
        }
    }

    public IDisposable Subscribe(Action<IReadOnlyList<LogEntry>> onBatch)
    {
        ArgumentNullException.ThrowIfNull(onBatch);

        lock (_gate)
        {
            _subscribers.Add(onBatch);
        }

        return new Subscription(this, onBatch);
    }

    public void Dispose()
    {
        lock (_gate)
        {
            _subscribers.Clear();
            _entries.Clear();
        }
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    private void Unsubscribe(Action<IReadOnlyList<LogEntry>> onBatch)
    {
        lock (_gate)
        {
            _subscribers.Remove(onBatch);
        }
    }

    private sealed class Subscription : IDisposable
    {
        public Subscription(LogHub owner, Action<IReadOnlyList<LogEntry>> onBatch)
        {
            _owner = owner;
            _onBatch = onBatch;
        }

        public void Dispose()
        {
            LogHub? owner = Interlocked.Exchange(ref _owner, null);
            if (owner is null)
            {
                return;
            }

            owner.Unsubscribe(_onBatch);
        }

        private LogHub? _owner;
        private readonly Action<IReadOnlyList<LogEntry>> _onBatch;
    }

    private readonly object _gate = new();
    private readonly int _capacity;
    private readonly List<LogEntry> _entries = new();
    private readonly List<Action<IReadOnlyList<LogEntry>>> _subscribers = new();
    private long _sequence;
}