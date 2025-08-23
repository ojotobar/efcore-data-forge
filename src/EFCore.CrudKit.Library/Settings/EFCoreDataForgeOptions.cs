namespace EFCore.CrudKit.Library.Settings
{
    public class EFCoreDataForgeOptions
    {
        public MongoDbOptions MongoDb { get; set; } = new();
    }
}