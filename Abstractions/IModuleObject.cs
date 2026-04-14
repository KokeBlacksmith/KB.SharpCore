namespace KB.SharpCore.Abstractions;

public interface IModuleObject
{
    Guid ID { get; }
    Guid ModuleID { get; set; }
}