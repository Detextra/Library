using Library.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IRepository
{
    public interface IBookRepository
    {
        Book Add(string title, string author, int copies);
        Book? GetById(int id);
        IEnumerable<Book> GetAll();
        bool Remove(int id);
    }

    public class InMemoryBookRepository : IBookRepository
    {
        private readonly ConcurrentDictionary<int, Book> _books = new();
        private int _nextId = 1;

        public Book Add(string title, string author, int copies)
        {
            int id = Interlocked.Increment(ref _nextId);
            Book book = new Book(id, title, author, copies);
            _books.TryAdd(id, book);
            return book;
        }

        public Book? GetById(int id) => _books.TryGetValue(id, out var book) ? book : null;

        public IEnumerable<Book> GetAll() => _books.Values;

        public bool Remove(int id) => _books.TryRemove(id, out _);
    }
}
