using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app.plataforma
{
    public class Postagens
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("dtCadastro")]
        public string? dtCadastro { get; set; }
        [BsonElement("titulo")]
        public string? titulo { get; set; }
        [BsonElement("descricao")]
        public string? descricao { get; set; }
        [BsonElement("tipo")]
        public int? tipo { get; set; }
        [BsonElement("arquivo")]
        public object? arquivo { get; set; }

        [BsonElement("usuarioId")]
        public string? usuarioid { get; set; }
    }

}
