using System;

namespace KB.SharpCore.DesignPatterns.Pooling;

public interface IPooledObject
{
    void Activate();
    void Deactivate();
    void Destroy();

    event Action<IPooledObject> Completed;
    public event Action<IPooledObject>? Activated;
    public event Action<IPooledObject>? Deactivated;
}