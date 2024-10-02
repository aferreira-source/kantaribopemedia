using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace app.plataforma.Models
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoUser
    {
    }
}
