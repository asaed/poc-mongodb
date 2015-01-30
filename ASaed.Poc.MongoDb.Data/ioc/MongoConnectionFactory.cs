using MongoDB.Driver;

namespace ASaed.Poc.MongoDb.Data.ioc
{
    public class MongoConnectionFactory
    {
        public static MongoDatabase GetMongoDatabase()
        {
            var mongoClientSettings = new MongoClientSettings();
            mongoClientSettings.Credentials = new[] { MongoCredential.CreateCredential(AppSettings.MongoDbCredentialDb, AppSettings.MongoDbCredentialUserName, AppSettings.MongoDbCredentialPassword) };
            mongoClientSettings.Server = new MongoServerAddress(AppSettings.MongoDbHostName, AppSettings.MongoDbHostPort);

            var mongoClient = new MongoClient(mongoClientSettings);

            return mongoClient.GetServer().GetDatabase(AppSettings.MongoDbDatabaseName);
        }
    }
}