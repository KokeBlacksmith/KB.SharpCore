using System.Reflection;

namespace KB.SharpCore.DesignPatterns.UserAction.CommonActions;

/// <summary>
/// User action that changes a writable property value and can restore the previous value on undo.
/// </summary>
public sealed class ChangeValueUserAction<TTarget, TValue> : UserActionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeValueUserAction{TTarget, TValue}"/> class.
    /// </summary>
    public ChangeValueUserAction(
        IUserActionInitiator initiator,
        TTarget target,
        TValue newValue,
        string propertyName,
        string name,
        bool isSignificant)
        : base(initiator, name, isSignificant)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        _target = target;
        _property = typeof(TTarget).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found on '{typeof(TTarget).FullName}'.");

        if (_property.SetMethod is null)
        {
            throw new InvalidOperationException($"Property '{propertyName}' on '{typeof(TTarget).FullName}' is not writable.");
        }

        _oldValue = (TValue?)_property.GetValue(_target);
        _newValue = newValue;
    }

    protected override void m_Do()
    {
        _property.SetValue(_target, _newValue);
    }

    protected override void m_UnDo()
    {
        _property.SetValue(_target, _oldValue);
    }

    private readonly TTarget _target;
    private readonly PropertyInfo _property;
    private readonly TValue? _oldValue;
    private readonly TValue _newValue;
}