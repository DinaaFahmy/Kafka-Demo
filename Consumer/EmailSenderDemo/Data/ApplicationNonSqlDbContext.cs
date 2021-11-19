using MongoDB.Driver;

namespace EmailSenderDemo.Data
{
    public class ApplicationNonSqlDbContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public ApplicationNonSqlDbContext()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("KafkaDemo");
        }

        public IMongoCollection<T> GetCollection<T>() where T : class
        {
            var collectionName = typeof(T).Name.ToString();
            return _database.GetCollection<T>(collectionName);
        }
    }
}
