using System.Reflection;

namespace KB.SharpCore.Registry;

public sealed class ModelOutputFactory<TModel, TOutput>
    where TModel : class
    where TOutput : class
{
    public ModelOutputFactory(params Assembly[] assembliesToScan)
        : this(new LocalInjectionServiceProvider(), assembliesToScan)
    {
    }

    public ModelOutputFactory(LocalInjectionServiceProvider services, params Assembly[] assembliesToScan)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrations = BuildRegistrations(assembliesToScan ?? []);
    }

    public TOutput? Create(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return Create(model.GetType(), model);
    }

    public TOutput? Create(Type modelType)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        return Create(modelType, null);
    }

    private TOutput? Create(Type modelType, TModel? model)
    {
        if (!TryGetOutputType(modelType, out Type? outputType))
        {
            return null;
        }

        object instance = CreateInstance(modelType, outputType!, model);
        return (TOutput)instance;
    }

    private bool TryGetOutputType(Type modelType, out Type? outputType)
    {
        if (_registrations.TryGetValue(modelType, out outputType))
        {
            return true;
        }

        foreach ((Type registeredModelType, Type registeredOutputType) in _registrations)
        {
            if (registeredModelType.IsAssignableFrom(modelType))
            {
                outputType = registeredOutputType;
                return true;
            }
        }

        outputType = null;
        return false;
    }

    private object CreateInstance(Type modelType, Type outputType, TModel? model)
    {
        ConstructorInfo[] constructors = outputType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        Array.Sort(constructors, static (left, right) => right.GetParameters().Length.CompareTo(left.GetParameters().Length));

        foreach (ConstructorInfo constructor in constructors)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            object?[] args = new object?[parameters.Length];
            bool canConstruct = true;

            for (int index = 0; index < parameters.Length; index++)
            {
                Type parameterType = parameters[index].ParameterType;

                if (model is not null && parameterType.IsInstanceOfType(model))
                {
                    args[index] = model;
                    continue;
                }

                if (_services.TryGet(parameterType, out object? service))
                {
                    args[index] = service;
                    continue;
                }

                if (parameters[index].HasDefaultValue)
                {
                    args[index] = parameters[index].DefaultValue;
                    continue;
                }

                canConstruct = false;
                break;
            }

            if (canConstruct)
            {
                return constructor.Invoke(args);
            }
        }

        throw new InvalidOperationException($"Unable to construct '{outputType.FullName}' for model '{modelType.FullName}'.");
    }

    private static Dictionary<Type, Type> BuildRegistrations(Assembly[] assemblies)
    {
        Dictionary<Type, Type> registrations = new();

        foreach (Assembly assembly in assemblies)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface || !typeof(TOutput).IsAssignableFrom(type))
                {
                    continue;
                }

                ModelTypeAttribute? attribute = type.GetCustomAttribute<ModelTypeAttribute>(inherit: false);
                if (attribute is null || !typeof(TModel).IsAssignableFrom(attribute.ModelType))
                {
                    continue;
                }

                registrations[attribute.ModelType] = type;
            }
        }

        return registrations;
    }

    private readonly LocalInjectionServiceProvider _services;
    private readonly Dictionary<Type, Type> _registrations;
}