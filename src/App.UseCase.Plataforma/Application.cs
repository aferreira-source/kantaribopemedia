namespace app.plataforma
{
    using AspNetCore.Identity.Mongo;
    using AspNetCore.Identity.Mongo.Model;
    using MongoDbGenericRepository.Attributes;

    [CollectionName("Users")]
    public class ApplicationUser : MongoUser<Guid>
    {
    }

    [CollectionName("Acessos")]
    public class ApplicationRole : MongoRole<Guid>
    {
    }
}
