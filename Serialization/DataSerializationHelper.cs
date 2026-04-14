using KB.SharpCore.Utils;
using Path = KB.SharpCore.IO.Path;

namespace KB.SharpCore.Serialization;

public static class DataSerializationHelper
{
    public enum ESerializationType
    {
        Xml,
        Json,
        Binary,
    }

    public static async Task<Result> SaveAsync<T>(T serializable, Path path, ESerializationType serializationType, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        return serializationType switch
        {
            ESerializationType.Xml => await Task.Run(() => XmlSerializableHelper.Save(serializable, path.StringPath), ct),
            ESerializationType.Json => await DataContractSerializableHelper.SaveAsync(serializable, path, DataContractSerializableHelper.ESerializationType.Json),
            ESerializationType.Binary => await DataContractSerializableHelper.SaveAsync(serializable, path, DataContractSerializableHelper.ESerializationType.Binary),
            _ => throw new ArgumentOutOfRangeException(nameof(serializationType), serializationType, null),
        };
    }

    public static async Task<Result<T>> LoadAsync<T>(Path path, ESerializationType serializationType, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        return serializationType switch
        {
            ESerializationType.Xml => await Task.Run(() => XmlSerializableHelper.Load<T>(path.StringPath), ct),
            ESerializationType.Json => await DataContractSerializableHelper.LoadAsync<T>(path, DataContractSerializableHelper.ESerializationType.Json),
            ESerializationType.Binary => await DataContractSerializableHelper.LoadAsync<T>(path, DataContractSerializableHelper.ESerializationType.Binary),
            _ => throw new ArgumentOutOfRangeException(nameof(serializationType), serializationType, null),
        };
    }

    public static async Task<Result> LoadAsync<T>(T fillObject, Path path, ESerializationType serializationType, CancellationToken ct = default)
        where T : class
    {
        ct.ThrowIfCancellationRequested();

        return serializationType switch
        {
            ESerializationType.Xml => await Task.Run(() => XmlSerializableHelper.Load(path.StringPath, fillObject), ct),
            ESerializationType.Json => await DataContractSerializableHelper.LoadAsync(fillObject, path, DataContractSerializableHelper.ESerializationType.Json),
            ESerializationType.Binary => await DataContractSerializableHelper.LoadAsync(fillObject, path, DataContractSerializableHelper.ESerializationType.Binary),
            _ => throw new ArgumentOutOfRangeException(nameof(serializationType), serializationType, null),
        };
    }
}