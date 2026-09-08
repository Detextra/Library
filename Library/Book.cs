using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Book
    {
        public int IdBook;
        public string Title;
        public string Author;
        public int NumberOfCopies;

        public Book(int IdBook, string Title, string Author, int NumberOfCopies)
        {
            this.IdBook = IdBook;
            this.Title = Title;
            this.Author = Author;
            this.NumberOfCopies = NumberOfCopies;
        }
    }
}
