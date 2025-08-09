using EFCore.CrudKit.Library.Models;

namespace EFCore.CrudKit.Tests.Entities
{
    public class DataForgeTestUser : EntityBase
    {
        public string Name { get; set; } = string.Empty;
    }
}