using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app.plataforma
{
    public class Postagens
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("usuarioId")]
        public string? usuarioid { get; set; }

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

        [BsonElement("dtHora_Inicio")]
        public DateTime? dtHora_Inicio { get; set; }
        [BsonElement("dtHora_Fim")]
        public DateTime? dtHora_Fim { get; set; }
        [BsonElement("dtHora_Publicacao")]
        public DateTime? dtHora_Publicacao { get; set; }
        [BsonElement("dtHora_Expiracao")]
        public DateTime? dtHora_Expiracao { get; set; }
    }
}



