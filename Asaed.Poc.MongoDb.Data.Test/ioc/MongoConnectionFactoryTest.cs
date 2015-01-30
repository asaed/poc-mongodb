using System;
using System.Linq;
using ASaed.Poc.MongoDb.Data.ioc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NUnit.Framework;

namespace Asaed.Poc.MongoDb.Data.Test.ioc
{
    [TestFixture]
    public class MongoConnectionFactoryTest
    {

        [Test]
        public void ShouldConnectToMongoDatabase()
        {
            MongoDatabase mongoDatabase = MongoConnectionFactory.GetMongoDatabase();

            Assert.IsNotNull(mongoDatabase);
            MongoCollection<BsonDocument> mongoCollection = mongoDatabase.GetCollection("students");
            Assert.IsNotNull(mongoCollection);

            var asQueryable = mongoCollection.AsQueryable();
            foreach (var bsonDoc in asQueryable)
            {
                Console.Write(string.Format("student = {0}", bsonDoc["name"]));
            }
        }
    }
}