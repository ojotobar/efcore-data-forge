namespace EFCore.CrudKit.Library.Data.Interfaces
{
    public interface IEFCoreDataForgeManager
    {
        IEFCoreCrudKit SQL {  get; }
        IEFCoreMongoCrudKit Mongo { get; }
    }
}
