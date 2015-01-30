using System.Configuration;

namespace ASaed.Poc.MongoDb.Data
{
    public static class AppSettings
    {
        public static string MongoDbHostName
        {
            get { return ConfigurationManager.AppSettings["mongo-host-name"] ?? "mongo-host-name key not found"; }
        }

        public static int MongoDbHostPort
        {
            get {
                var portAsString = ConfigurationManager.AppSettings["mongo-host-port"];
                return portAsString!=null ? int.Parse(portAsString) : -1; 
            }
        }

        public static string MongoDbCredentialDb
        {
            get { return ConfigurationManager.AppSettings["mongo-credential-db"] ?? "mongo-credential-db key not found"; }
        }

        public static string MongoDbCredentialUserName
        {
            get { return ConfigurationManager.AppSettings["mongo-credential-username"] ?? "mongo-credential-username key not found"; }
        }

        public static string MongoDbCredentialPassword
        {
            get { return ConfigurationManager.AppSettings["mongo-credential-password"] ?? "mongo-credential-password key not found"; }
        }

        public static string MongoDbDatabaseName
        {
            get { return ConfigurationManager.AppSettings["mongo-database-name"] ?? "mongo-database-name key not found"; }
        }
    }
}