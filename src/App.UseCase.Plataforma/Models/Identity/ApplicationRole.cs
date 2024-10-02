using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System.Runtime.CompilerServices;

namespace app.plataforma.Models
{
    [CollectionName("Roles")]
    public class ApplicationRole : MongoRole
    {

    }
}
