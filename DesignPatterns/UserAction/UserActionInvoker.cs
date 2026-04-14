namespace KB.SharpCore.DesignPatterns.UserAction;

/// <summary>
/// Handle Do / Undo actions.
/// </summary>
public sealed class UserActionInvoker
{
    public event EventHandler<UserActionExecutedEventArgs>? UserActionExecuted;
    public event EventHandler? Saved;

    public UserActionInvoker(int capacity = 100)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _capacity = capacity;
        _history = new List<IUserAction>(capacity);
    }

    public bool CanUndo => _index >= 0;
    public bool CanRedo => _index < _history.Count - 1;
    public bool IsPaused => _pauseCount > 0;
    public bool IsModified => _index != _savedIndex;

    public void Execute(IUserAction action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        action.Do();
        if (IsPaused)
        {
            return;
        }

        // drop future redo steps
        if (_index < _history.Count - 1)
        {
            _history.RemoveRange(_index + 1, _history.Count - (_index + 1));
        }

        // append
        _history.Add(action);
        _index++;

        // enforce capacity (evict oldest)
        if (_history.Count > _capacity)
        {
            _history.RemoveAt(0);
            _index--;
            _savedIndex = Math.Max(_savedIndex, -1);
        }

        UserActionExecuted?.Invoke(this, new UserActionExecutedEventArgs(action, isUndo: false, isRedo: false));
    }

    public void ExecuteUserAction(IUserAction action)
    {
        Execute(action);
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            return;
        }

        IUserAction action = _history[_index];
        action.Undo();
        _index--;
        UserActionExecuted?.Invoke(this, new UserActionExecutedEventArgs(action, isUndo: true, isRedo: false));
    }

    public void UnDo()
    {
        Undo();
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            return;
        }

        _index++;
        IUserAction action = _history[_index];
        action.Do();
        UserActionExecuted?.Invoke(this, new UserActionExecutedEventArgs(action, isUndo: false, isRedo: true));
    }

    public void ReDo()
    {
        Redo();
    }

    public void Clear()
    {
        _history.Clear();
        _index = -1;
        _savedIndex = -1;
    }

    public IDisposable PauseRecording()
    {
        _pauseCount++;
        return new DisposableAction(() => _pauseCount--);
    }

    public void MarkSaved()
    {
        _savedIndex = _index;
        Saved?.Invoke(this, EventArgs.Empty);
    }

    private sealed class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;
        private bool _disposed;
        public DisposableAction(Action onDispose) => _onDispose = onDispose;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _onDispose();
        }
    }

    private readonly List<IUserAction> _history;
    private readonly int _capacity;
    private int _index = -1;
    private int _pauseCount;
    private int _savedIndex = -1;
}
