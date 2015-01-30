using MongoDB.Driver;

namespace ASaed.Poc.MongoDb.Data.ioc
{
    public class MongoConnectionFactory
    {
        public static MongoDatabase GetMongoDatabase()
        {
            var mongoClientSettings = new MongoClientSettings();
            mongoClientSettings.Credentials = new[] { MongoCredential.CreateCredential("admin", "poc_csharp", "password") };
            mongoClientSettings.Server = new MongoServerAddress("localhost", 27017);

            var mongoClient = new MongoClient(mongoClientSettings);

            return mongoClient.GetServer().GetDatabase("db-poc");
        }
    }
}