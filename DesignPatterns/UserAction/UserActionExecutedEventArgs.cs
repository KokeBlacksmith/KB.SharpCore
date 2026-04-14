namespace KB.SharpCore.DesignPatterns.UserAction;

public sealed class UserActionExecutedEventArgs : EventArgs
{
    public UserActionExecutedEventArgs(IUserAction action, bool isUndo, bool isRedo)
    {
        Action = action;
        IsUndo = isUndo;
        IsRedo = isRedo;
    }

    public IUserAction Action { get; }
    public bool IsUndo { get; }
    public bool IsRedo { get; }
}