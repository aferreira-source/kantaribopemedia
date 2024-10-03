using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace app.plataforma.Identity
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoUser
    {
    }
}
