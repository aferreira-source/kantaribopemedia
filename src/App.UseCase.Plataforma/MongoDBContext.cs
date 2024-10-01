using app.plataforma.Models;
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

        public IMongoCollection<Documentos> Documentos => _database.GetCollection<Documentos>("Documentos");
        public IMongoCollection<Usuarios> Usuarios => _database.GetCollection<Usuarios>("Usuarios");
    }
}
