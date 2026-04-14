using System.Collections;
using System.Reflection;

namespace KB.SharpCore.DesignPatterns.UserAction.CommonActions;

/// <summary>
/// User action that removes an item from a collection-like property and restores it on undo.
/// </summary>
public sealed class RemoveItemFromCollectionUserAction<TTarget, TItem> : UserActionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveItemFromCollectionUserAction{TTarget, TItem}"/> class.
    /// </summary>
    public RemoveItemFromCollectionUserAction(
        IUserActionInitiator initiator,
        TTarget target,
        TItem item,
        string propertyName,
        string name,
        bool isSignificant)
        : base(initiator, name, isSignificant)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        _target = target;
        _item = item;
        _property = typeof(TTarget).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Collection property '{propertyName}' was not found on '{typeof(TTarget).FullName}'.");
    }

    protected override void m_Do()
    {
        object collection = GetCollection();
        s_RemoveItem(collection, _item);
    }

    protected override void m_UnDo()
    {
        object collection = GetCollection();
        s_AddItem(collection, _item);
    }

    private object GetCollection()
    {
        return _property.GetValue(_target)
            ?? throw new InvalidOperationException($"Collection property '{_property.Name}' is null on '{typeof(TTarget).FullName}'.");
    }

    private static void s_AddItem(object collection, TItem item)
    {
        if (collection is IDictionary dictionary)
        {
            (object? key, object? value) = s_ExtractKeyValue(item);
            dictionary.Add(key, value);
            return;
        }

        if (collection is IList list)
        {
            list.Add(item);
            return;
        }

        MethodInfo? addMethod = collection.GetType().GetMethod("Add", [typeof(TItem)]);
        if (addMethod is null)
        {
            throw new InvalidOperationException($"Collection type '{collection.GetType().FullName}' does not expose a compatible Add method.");
        }

        addMethod.Invoke(collection, [item]);
    }

    private static void s_RemoveItem(object collection, TItem item)
    {
        if (collection is IDictionary dictionary)
        {
            (object? key, _) = s_ExtractKeyValue(item);
            dictionary.Remove(key);
            return;
        }

        if (collection is IList list)
        {
            list.Remove(item);
            return;
        }

        MethodInfo? removeMethod = collection.GetType().GetMethod("Remove", [typeof(TItem)]);
        if (removeMethod is null)
        {
            throw new InvalidOperationException($"Collection type '{collection.GetType().FullName}' does not expose a compatible Remove method.");
        }

        removeMethod.Invoke(collection, [item]);
    }

    private static (object? Key, object? Value) s_ExtractKeyValue(TItem item)
    {
        Type itemType = typeof(TItem);
        PropertyInfo? keyProperty = itemType.GetProperty("Key");
        PropertyInfo? valueProperty = itemType.GetProperty("Value");

        if (keyProperty is null || valueProperty is null)
        {
            throw new InvalidOperationException($"Item type '{itemType.FullName}' must expose Key and Value properties for dictionary actions.");
        }

        return (keyProperty.GetValue(item), valueProperty.GetValue(item));
    }

    private readonly TTarget _target;
    private readonly TItem _item;
    private readonly PropertyInfo _property;
}