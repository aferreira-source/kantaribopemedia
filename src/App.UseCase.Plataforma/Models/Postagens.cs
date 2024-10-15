using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
namespace app.plataforma;
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

    [BsonIgnore]
    public string? arquivoUpload { get; set; }


    [BsonIgnore]
    public string? linkFile { get; set; }//{ get { return $"http://127.0.0.1:10000/devstoreaccount1/portal-blob/{nomeArquivo}"; } }

    [BsonIgnore]
    public Object? fileBlob { get; set; }


    //[BsonElement("arquivoId")]
    //public ObjectId? arquivoId { get; set; }

    [BsonElement("nomeArquivo")]
    public string? nomeArquivo { get; set; }
    public DateTime? dtHora_Fim { get; set; }
    [BsonElement("dtHora_Publicacao")]
    public DateTime? dtHora_Publicacao { get; set; }
    [BsonElement("dtHora_Expiracao")]
    public DateTime? dtHora_Expiracao { get; set; }
    [NotMapped]
    public string? usuario { get; set; }

    [BsonElement("bucketFileInfo")]
    public BucketFileInfo bucketFileInfo { get; set; }

    public Postagens()
    {
        bucketFileInfo = new BucketFileInfo();
    }

}



