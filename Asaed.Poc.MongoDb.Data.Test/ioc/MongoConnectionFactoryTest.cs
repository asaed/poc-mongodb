using System;
using System.Linq;
using ASaed.Poc.MongoDb.Data.ioc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NLog;
using NUnit.Framework;

namespace Asaed.Poc.MongoDb.Data.Test.ioc
{
    [TestFixture]
    public class MongoConnectionFactoryTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void ShouldConnectToMongoDatabase()
        {
            
            MongoDatabase mongoDatabase = MongoConnectionFactory.GetMongoDatabase();

            Assert.IsNotNull(mongoDatabase);
            MongoCollection<BsonDocument> mongoCollection = mongoDatabase.GetCollection("students");
            Assert.IsNotNull(mongoCollection);

            var asQueryable = mongoCollection.AsQueryable();
            logger.Debug("Spinning through students");
            foreach (var bsonDoc in asQueryable)
            {
                logger.Debug("student = {0}", bsonDoc["name"]);
            }
        }
    }
}