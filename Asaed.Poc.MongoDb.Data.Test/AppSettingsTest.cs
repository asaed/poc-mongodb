using ASaed.Poc.MongoDb.Data;
using NUnit.Framework;

namespace Asaed.Poc.MongoDb.Data.Test
{
    [TestFixture]
    public class AppSettingsTest
    {

        [Test]
        public void ConfigValuesArePopulatedFromTestProject()
        {
            Assert.AreEqual("localhost", AppSettings.MongoDbHostName);
            Assert.AreEqual(27017, AppSettings.MongoDbHostPort);
            Assert.AreEqual("admin", AppSettings.MongoDbCredentialDb);
            Assert.AreEqual("poc_csharp", AppSettings.MongoDbCredentialUserName);
            Assert.AreEqual("password", AppSettings.MongoDbCredentialPassword);
            Assert.AreEqual("poc-db", AppSettings.MongoDbDatabaseName);
        }
         
    }
}