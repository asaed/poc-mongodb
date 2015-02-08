﻿using System.Collections.Generic;
using System.Linq;
using ASaed.Poc.MongoDb.Data.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace ASaed.Poc.MongoDb.Data.Repository
{
    public interface IBookRepository
    {
        void Add(Book book);
        IEnumerable<Book> FindByTitle(string title);
        IEnumerable<Book> FindByAuthor(string author);
        void Delete(Book book);
    }

    public class BookRepository : IBookRepository 
    {
        private readonly MongoDatabase _mongoDatabase;
        private readonly MongoCollection<Book> _mongoCollection;
        private QueryDocument _queryDocument;

        public BookRepository(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            _mongoCollection = _mongoDatabase.GetCollection<Book>("books");
        }

        public void Add(Book book)
        {
            _mongoCollection.Insert(book);
        }

        public IEnumerable<Book> FindByTitle(string title)
        {
            return _mongoCollection.AsQueryable().Where(book => book.Title == title);
        }

        public IEnumerable<Book> FindByAuthor(string author)
        {
            return _mongoCollection.AsQueryable().Where(book => book.Author == author);
        }

        public void Delete(Book book)
        {
            var mongoQuery = new QueryBuilder<Book>().EQ(x => x.Id, book.Id);

            _mongoCollection.Remove(mongoQuery);
        }
    }
}