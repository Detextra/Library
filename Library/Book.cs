namespace Library
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int NumberOfCopiesAvailable { get; set; }

        public Book(int BookId, string Title, string Author, int NumberOfCopiesAvailable)
        {
            this.BookId = BookId;
            this.Title = Title;
            this.Author = Author;
            this.NumberOfCopiesAvailable = NumberOfCopiesAvailable;
        }
    }
}
