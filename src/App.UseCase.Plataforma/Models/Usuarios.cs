using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace app.plataforma.Models
{
    public class Usuarios
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nome")]
        public string? nome { get; set; }
        [BsonElement("email")]
        public string? email { get; set; }
        [BsonElement("senha")]
        public string? senha { get; set; }
        [BsonElement("ativo")]
        public int? ativo { get; set; }
    }
}
