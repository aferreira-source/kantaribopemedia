using MongoDB.Driver;

namespace app.plataforma
{
    public class MongoDBContext 
    {
        private readonly IMongoDatabase _database;
        public MongoDBContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Postagens> Postagens => _database.GetCollection<Postagens>("Postagens");
        //public IMongoCollection<User> Usuarios => _database.GetCollection<User>("User");

        public IMongoCollection<Favoritos> Favoritos => _database.GetCollection<Favoritos>("Favoritos");
    }
}
