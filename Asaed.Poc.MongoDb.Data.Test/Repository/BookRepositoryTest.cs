using System.Collections.Generic;
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
        private Book _expectedBook;
        private MongoDatabase _mongoDatabase;
        private MongoCollection<BsonDocument> _plainCollection;
        private MongoCollection<Book> _bookCollection;
        private IBookRepository _repository;
        private Book _differentBook;
        
        [SetUp]
        public void Setup()
        {

            _expectedBook = new Book
            {
                Author = Author,
                Title = Title1,
                Publisher = Publisher,
                Isbn = Isbn
            };

            _differentBook = new Book
            {
                Author = Author + " Different",
                Title = Title1 + "Different",
                Publisher = Publisher + "Different",
                Isbn = Isbn + "Different",
            };

            _mongoDatabase = MongoConnectionFactory.GetMongoDatabase();
            _plainCollection = _mongoDatabase.GetCollection("books");
            _bookCollection = _mongoDatabase.GetCollection<Book>("books");

            _repository = new BookRepository(_mongoDatabase);

            _plainCollection.Drop();
        }

        [Test]
        public void SaveShouldUseCustomPropertyMapping()
        {
            _repository.Add(_expectedBook);

            
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
            

            _repository.Add(_expectedBook);
            _repository.Add(_differentBook);

            var books = _repository.FindByTitle(Title1).ToList();

            Assert.AreEqual(1, books.Count);
            var foundBook = books[0];

            Assert.AreNotSame(_expectedBook, foundBook);
            Assert.AreEqual(_expectedBook.Author, foundBook.Author);
            Assert.AreEqual(_expectedBook.Title, foundBook.Title);
            Assert.AreEqual(_expectedBook.Publisher, foundBook.Publisher);
            Assert.AreEqual(_expectedBook.Isbn, foundBook.Isbn);
        }

        [Test]
        public void FindBooksByAuthor()
        {
            var expectedBook2 = new Book
            {
                Author = Author,
                Title = Title2,
                Publisher = Publisher,
            };

            _repository.Add(expectedBook2);
            _repository.Add(_differentBook);
            _repository.Add(_expectedBook);

            var books = _repository.FindByAuthor(Author).OrderBy(x=>x.Title).ToList();

            Assert.AreEqual(2, books.Count);
            var foundBook = books[0];

            Assert.AreNotSame(_expectedBook, foundBook);
            Assert.AreEqual(_expectedBook.Author, foundBook.Author);
            Assert.AreEqual(_expectedBook.Title, foundBook.Title);
            Assert.AreEqual(_expectedBook.Publisher, foundBook.Publisher);
            Assert.AreEqual(_expectedBook.Isbn, foundBook.Isbn);
            
            foundBook = books[1];

            Assert.AreNotSame(expectedBook2, foundBook);
            Assert.AreEqual(expectedBook2.Author, foundBook.Author);
            Assert.AreEqual(expectedBook2.Title, foundBook.Title);
            Assert.AreEqual(expectedBook2.Publisher, foundBook.Publisher);
            Assert.AreEqual(expectedBook2.Isbn, foundBook.Isbn);
        }

        [Test]
        public void IgnoreExtraElementsFoundInMongo()
        {
            var bsonDocument = new BsonDocument()
            {
                
            };
            bsonDocument["elementNotFoundInBook"] = "elementValue";
            bsonDocument["author"] = Author;
            _plainCollection.Insert(bsonDocument);

            var books = _repository.FindByAuthor(Author).ToList();

            Assert.AreEqual(1, books.Count);
            var actualBook = books[0];

            Assert.AreEqual(Author, actualBook.Author);
            Assert.IsNull(actualBook.Title);
            Assert.IsNull(actualBook.Publisher);
            Assert.IsNull(actualBook.Isbn);
        }

        [Test]
        public void DeleteById()
        {
            _repository.Add(_expectedBook);
            _repository.Add(_differentBook);

            _repository.Delete(_expectedBook);

            var asQueryable = _bookCollection.AsQueryable();
            Assert.AreEqual(1, asQueryable.Count());
            var actual = asQueryable.First();

            Assert.AreNotSame(_differentBook, actual);
            Assert.AreEqual(_differentBook.Author, actual.Author);
            Assert.AreEqual(_differentBook.Title, actual.Title);
            Assert.AreEqual(_differentBook.Publisher, actual.Publisher);
            Assert.AreEqual(_differentBook.Isbn, actual.Isbn);
        }

        [Test]
        public void ShouldUpdateBookRetrievedFromRepository()
        {
            _repository.Add(_expectedBook);
            var retrievedBook = _repository.FindByTitle(Title1).Single();

            retrievedBook.Author = "New Author";
            retrievedBook.Isbn = "NewIsn9887-88777";

            _repository.Update(retrievedBook);

            var modifiedBook = _repository.FindByTitle(Title1).Single();
            Assert.AreNotSame(_expectedBook, modifiedBook);
            Assert.AreNotSame(retrievedBook, modifiedBook);
            Assert.AreEqual(retrievedBook.Id, modifiedBook.Id);
            Assert.AreEqual("New Author", modifiedBook.Author);
            Assert.AreEqual("NewIsn9887-88777", modifiedBook.Isbn);
            Assert.AreEqual(Publisher, modifiedBook.Publisher);
            Assert.AreEqual(Title1, modifiedBook.Title);
        }

        [Test]
        public void ShouldRetrieveAllBooks()
        {
            _repository.Add(new List<Book>{_expectedBook, _differentBook});

            var findAll = _repository.FindAll().ToList();

            Assert.AreEqual(2, findAll.Count);
            Assert.IsNotNull(findAll.Single(x => x.Author == _expectedBook.Author));
            Assert.IsNotNull(findAll.Single(x => x.Author == _differentBook.Author));
        }
    }
}