using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Book
    {
        public int BookId;
        public string Title;
        public string Author;
        public int NumberOfCopiesAvailable;

        public Book(int BookId, string Title, string Author, int NumberOfCopiesAvailable)
        {
            this.BookId = BookId;
            this.Title = Title;
            this.Author = Author;
            this.NumberOfCopiesAvailable = NumberOfCopiesAvailable;
        }
    }
}
