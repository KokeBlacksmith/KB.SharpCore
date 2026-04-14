namespace KB.SharpCore.Synchronization;

public sealed class GuardedChangedEventArgs : EventArgs
{
    public GuardedChangedEventArgs(bool isGuarded)
    {
        IsGuarded = isGuarded;
    }

    public bool IsGuarded { get; }
}