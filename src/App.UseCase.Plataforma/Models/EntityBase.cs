using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app.plataforma.Models
{
    public class EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public EntityBase()
        {
         Id = Guid.NewGuid().ToString();
        }
    }
}
