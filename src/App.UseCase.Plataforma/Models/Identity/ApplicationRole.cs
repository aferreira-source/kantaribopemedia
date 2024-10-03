using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace app.plataforma.Identity
{
    [CollectionName("Roles")]
    public class ApplicationRole : MongoRole
    {

    }
}
