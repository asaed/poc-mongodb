using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASaed.Poc.MongoDb.Data.Model
{
    
    public class Book
    {
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("publisher")]
        public string Publisher { get; set; }

        [BsonElement("isbn")]
        [BsonIgnoreIfNull]
        public string Isbn { get; set; } 
    }
}