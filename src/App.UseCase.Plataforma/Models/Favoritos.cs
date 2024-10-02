using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace app.plataforma.Models
{
    public class Favoritos : EntityBase
    {
        public object? documentoId { get; set; }
        public object? usuarioId { get; set; }
        public string? dtCadastro { get; set; }
    }

}
