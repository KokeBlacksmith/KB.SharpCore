namespace KB.SharpCore.Abstractions;

public interface IDirtyState
{
    bool IsDirty { get; }
    void MarkDirty();
    void ClearDirty();
}