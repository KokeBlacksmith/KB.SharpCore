namespace KB.SharpCore.Registry;

public sealed class LocalInjectionServiceProvider
{
    public void Add<TService>(TService service) where TService : class
    {
        Add(typeof(TService), service);
    }

    public void Add(Type serviceType, object? service)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (service is null)
        {
            _services.Remove(serviceType);
            return;
        }

        _services[serviceType] = service;
    }

    public bool TryGet(Type serviceType, out object? service)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (_services.TryGetValue(serviceType, out service))
        {
            return true;
        }

        foreach ((Type registeredType, object registeredService) in _services)
        {
            if (serviceType.IsAssignableFrom(registeredType))
            {
                service = registeredService;
                return true;
            }
        }

        service = null;
        return false;
    }

    private readonly Dictionary<Type, object> _services = new();
}