namespace KB.SharpCore.DesignPatterns.UserAction;

public abstract class UserActionBase : IUserAction
{
    protected UserActionBase(IUserActionInitiator initiator, string name, bool isSignificant)
    {
        Initiator = initiator ?? throw new ArgumentNullException(nameof(initiator));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsSignificant = isSignificant;
    }

    public string Name { get; }
    public bool IsSignificant { get; }

    public void AddUserAction(IUserAction userAction)
    {
        ArgumentNullException.ThrowIfNull(userAction);
        _childActions.Add(userAction);
    }

    public void Do()
    {
        foreach (IUserAction childAction in _childActions)
        {
            childAction.Do();
        }

        m_Do();
        Initiator.OnUserActionExecuted(IsSignificant);
    }

    public void Undo()
    {
        m_UnDo();

        for (int index = _childActions.Count - 1; index >= 0; index--)
        {
            _childActions[index].Undo();
        }

        Initiator.OnUserActionExecuted(IsSignificant);
    }

    protected abstract void m_Do();
    protected abstract void m_UnDo();

    protected IUserActionInitiator Initiator { get; }
    private readonly List<IUserAction> _childActions = new();
}