using EFCore.CrudKit.Library.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EFCore.CrudKit.Tests.Entities
{
    public class MongoDataForgeTestUser : MongoEntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}
