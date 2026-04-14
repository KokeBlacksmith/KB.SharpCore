using System.Threading;

namespace KB.SharpCore.Synchronization;

public sealed class AsyncGuard : IDisposable
{
    public event EventHandler<GuardedChangedEventArgs>? GuardedChanged;

    public AsyncGuard(int concurrency)
    {
        if (concurrency <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(concurrency));
        }

        _semaphore = new SemaphoreSlim(concurrency, concurrency);
    }

    public async ValueTask<IDisposable> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        int activeCount = Interlocked.Increment(ref _activeCount);
        if (activeCount == 1)
        {
            GuardedChanged?.Invoke(this, new GuardedChangedEventArgs(isGuarded: true));
        }

        return new Releaser(this);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _semaphore.Dispose();
    }

    private void Release()
    {
        if (_disposed)
        {
            return;
        }

        _semaphore.Release();

        int activeCount = Interlocked.Decrement(ref _activeCount);
        if (activeCount == 0)
        {
            GuardedChanged?.Invoke(this, new GuardedChangedEventArgs(isGuarded: false));
        }
    }

    private sealed class Releaser : IDisposable
    {
        public Releaser(AsyncGuard owner)
        {
            _owner = owner;
        }

        public void Dispose()
        {
            AsyncGuard? owner = Interlocked.Exchange(ref _owner, null);
            owner?.Release();
        }

        private AsyncGuard? _owner;
    }

    private readonly SemaphoreSlim _semaphore;
    private int _activeCount;
    private bool _disposed;
}