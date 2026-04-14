using KB.SharpCore.Events;

namespace KB.SharpCore.Synchronization;

public sealed class RaiiOperationBoolean
{
    public event EventHandler<ValueChangedEventArgs<bool>>? OnAquireChanged;

    public bool IsExecuting => _operation.GetResource();

    public bool CanExecute()
    {
        return _operation.CanExecute();
    }

    public IDisposable Execute()
    {
        bool oldValue = _operation.GetResource();
        IDisposable disposable = _operation.Execute();
        RaiseChanged(oldValue);
        return new ReleaseScope(this, disposable);
    }

    public bool GetResource()
    {
        return _operation.GetResource();
    }

    private void RaiseChanged(bool oldValue)
    {
        bool newValue = _operation.GetResource();
        OnAquireChanged?.Invoke(this, new ValueChangedEventArgs<bool>(oldValue, newValue));
    }

    private sealed class ReleaseScope : IDisposable
    {
        public ReleaseScope(RaiiOperationBoolean owner, IDisposable inner)
        {
            _owner = owner;
            _inner = inner;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            bool oldValue = _owner._operation.GetResource();
            _inner.Dispose();
            _owner.RaiseChanged(oldValue);
        }

        private readonly RaiiOperationBoolean _owner;
        private readonly IDisposable _inner;
        private bool _disposed;
    }

    private readonly BooleanRAIIOperation _operation = new();
}