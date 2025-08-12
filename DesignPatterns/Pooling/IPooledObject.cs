using System;

namespace KB.SharpCore.DesignPatterns.Pooling;

public interface IPooledObject
{
    /// <summary>
    /// Called when the object is activated from the pool.
    /// <br/>Fired by the pool manager when an object is requested.
    /// </summary>
    void Activate();

    /// <summary>
    /// Called when the object is deactivated and returned to the pool.
    /// </summary>
    void Deactivate();

    /// <summary>
    /// Called when the object is destroyed and should not be reused.
    /// </summary>
    void Destroy();

    /// <summary>
    /// Event triggered when the object has completed its task and is ready to be returned to the pool.
    /// <br/>Notifies the pool manager that the object is ready for reuse.
    /// </summary>
    event Action<IPooledObject>? Completed;

    /// <summary>
    /// Event triggered when the object is activated.
    /// <br/>Notifies the pool manager that the object is now actived.
    /// </summary>
    event Action<IPooledObject>? Activated;

    /// <summary>
    /// Event triggered when the object is deactivated.
    /// <br/>Notifies the pool manager that the object is now deactivated and can be reused.
    /// </summary>
    event Action<IPooledObject>? Deactivated;
}