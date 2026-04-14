using KB.SharpCore.Utils;

namespace KB.SharpCore.Abstractions;

public interface ISaveable
{
    bool IsModified { get; }
    Task<Result> OnBeforeSave(CancellationToken ct);
    Task<Result> Save(CancellationToken ct);
    Task<Result> OnAfterSave(CancellationToken ct);
}