using System.Diagnostics;

namespace KB.SharpCore.DesignPatterns.Pooling;

public class GenericObjectPool<T> : IDisposable
    where T : IPooledObject, new()
{
    public event Action<IPooledObject>? ObjectReturned;
    protected readonly Stack<T> m_pool;

    public GenericObjectPool(int? maxCapacity = 80)
    {
        m_pool = new Stack<T>(maxCapacity ?? 80);
        MaxPoolSize = maxCapacity;
    }

    public int? MaxPoolSize { get; set; }

    public void Dispose()
    {
        _Dispose(!_disposed);
    }

    public T GetObject()
    {
        T obj = m_pool.Count == 0 ? this.m_CreateObject() : m_pool.Pop();
        obj.Completed += _OnPooledObjectCompleted;
        obj.Activate();
        return obj;
    }

    public void ReturnObject(T obj)
    {
        Debug.Assert(obj != null, "The given object is null.");

        obj.Completed -= _OnPooledObjectCompleted;
        obj.Deactivate();
        ObjectReturned?.Invoke(obj);

        bool poolContainsObject = m_pool.Contains(obj);
        Debug.Assert(!poolContainsObject, "The given object is already in the pool.");
        if (poolContainsObject)
        {
            return;
        }

        if (MaxPoolSize.HasValue && m_pool.Count >= MaxPoolSize)
        {
            obj.Deactivate();
            return;
        }

        m_pool.Push(obj);
    }

    private void _OnPooledObjectCompleted(IPooledObject sender)
    {
        ReturnObject((T)sender);
    }

    private void _ReleaseUnmanagedResources()
    {
        while (m_pool.Any())
        {
            T obj = m_pool.Pop();
            obj.Completed -= _OnPooledObjectCompleted;
            obj.Destroy();
        }
    }

    protected virtual T m_CreateObject()
    {
        return new T();
    }

    ~GenericObjectPool()
    {
        _Dispose(!_disposed);
    }

    private void _Dispose(bool dispose)
    {
        if (dispose)
        {
            _ReleaseUnmanagedResources();
        }

        _disposed = true;
    }

    private bool _disposed = false;
}
