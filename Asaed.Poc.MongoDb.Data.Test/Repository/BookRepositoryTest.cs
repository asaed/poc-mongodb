using System.Linq;
using ASaed.Poc.MongoDb.Data.ioc;
using ASaed.Poc.MongoDb.Data.Model;
using ASaed.Poc.MongoDb.Data.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NUnit.Framework;

namespace Asaed.Poc.MongoDb.Data.Test.Repository
{
    [TestFixture]
    public class BookRepositoryTest
    {
        private const string Author = "John Smith";
        private const string Title1 = "Best Mongodb Starter";
        private const string Title2 = "Second Best Mongodb Starter";
        private const string Publisher = "Greatest Publisher";
        private const string Isbn = "12254-444447";
        private IBookRepository _repository;
        private MongoDatabase _mongoDatabase;
        private MongoCollection<BsonDocument> _plainCollection;
        private MongoCollection<Book> _bookCollection;

        [SetUp]
        public void Setup()
        {
            _mongoDatabase = MongoConnectionFactory.GetMongoDatabase();
            _plainCollection = _mongoDatabase.GetCollection("books");
            _bookCollection = _mongoDatabase.GetCollection<Book>("books");

            _repository = new BookRepository(_mongoDatabase);

            _plainCollection.Drop();
        }

        [Test]
        public void SaveShouldUseCustomPropertyMapping()
        {
            var newBook = new Book
            {
                Author = Author,
                Title = Title1,
                Publisher = Publisher,
                Isbn = Isbn
            };

            _repository.Add(newBook);

            
            var asQueryable = _plainCollection.AsQueryable();
            Assert.AreEqual(1, asQueryable.Count());
            var firstBook = asQueryable.First();
            Assert.AreEqual(Author, firstBook["author"].AsString);
            Assert.AreEqual(Title1, firstBook["title"].AsString);
            Assert.AreEqual(Isbn, firstBook["isbn"].AsString);
            Assert.AreEqual(Publisher, firstBook["publisher"].AsString);
        }

        [Test]
        public void ShouldNotSaveNullForOptionalProperties()
        {
            var newBook = new Book
            {
            };

            _repository.Add(newBook);


            var asQueryable = _plainCollection.AsQueryable();
            Assert.AreEqual(1, asQueryable.Count());
            var firstBook = asQueryable.First();
            //these properties should always be saved even if null
            Assert.IsTrue(firstBook.Contains("author"));
            Assert.IsTrue(firstBook.Contains("title"));
            Assert.IsTrue(firstBook.Contains("publisher"));

            //these properties should be not saved if null
            Assert.IsFalse(firstBook.Contains("isbn"));
        }


        [Test]
        public void FindBooksByTitle()
        {
            var expectedBook = new Book
            {
                Author = Author,
                Title = Title1,
                Publisher = Publisher,
                Isbn = Isbn
            };

            var differentBook = new Book
            {
                Author = Author,
                Title = Title1 + " Different",
                Publisher = Publisher,
                Isbn = Isbn
            };

            _repository.Add(expectedBook);
            _repository.Add(differentBook);

            var books = _repository.FindByTitle(Title1).ToList();

            Assert.AreEqual(1, books.Count);
            var foundBook = books[0];

            Assert.AreNotSame(expectedBook, foundBook);
            Assert.AreEqual(expectedBook.Author, foundBook.Author);
            Assert.AreEqual(expectedBook.Title, foundBook.Title);
            Assert.AreEqual(expectedBook.Publisher, foundBook.Publisher);
            Assert.AreEqual(expectedBook.Isbn, foundBook.Isbn);
        }

        [Test]
        public void FindBooksByAuthor()
        {
            var expectedBook1 = new Book
            {
                Author = Author,
                Title = Title1,
                Publisher = Publisher,
                Isbn = Isbn
            };

            var expectedBook2 = new Book
            {
                Author = Author,
                Title = Title2,
                Publisher = Publisher,
            };

            var differentBook = new Book
            {
                Author = Author + " Different",
                Title = Title1 + "Different",
                Publisher = Publisher + "Different",
                Isbn = Isbn + "Different",
            };

            _repository.Add(expectedBook2);
            _repository.Add(differentBook);
            _repository.Add(expectedBook1);

            var books = _repository.FindByAuthor(Author).OrderBy(x=>x.Title).ToList();

            Assert.AreEqual(2, books.Count);
            var foundBook = books[0];

            Assert.AreNotSame(expectedBook1, foundBook);
            Assert.AreEqual(expectedBook1.Author, foundBook.Author);
            Assert.AreEqual(expectedBook1.Title, foundBook.Title);
            Assert.AreEqual(expectedBook1.Publisher, foundBook.Publisher);
            Assert.AreEqual(expectedBook1.Isbn, foundBook.Isbn);
            
            foundBook = books[1];

            Assert.AreNotSame(expectedBook2, foundBook);
            Assert.AreEqual(expectedBook2.Author, foundBook.Author);
            Assert.AreEqual(expectedBook2.Title, foundBook.Title);
            Assert.AreEqual(expectedBook2.Publisher, foundBook.Publisher);
            Assert.AreEqual(expectedBook2.Isbn, foundBook.Isbn);
        }

    }
}