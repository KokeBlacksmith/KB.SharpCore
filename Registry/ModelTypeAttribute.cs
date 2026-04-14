namespace KB.SharpCore.Registry;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ModelTypeAttribute : Attribute
{
    public ModelTypeAttribute(Type modelType)
    {
        ModelType = modelType ?? throw new ArgumentNullException(nameof(modelType));
    }

    public Type ModelType { get; }
}