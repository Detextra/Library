namespace Library.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        private int _numberOfCopiesAvailable;
        public int NumberOfCopiesAvailable
        {
            get => _numberOfCopiesAvailable;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Copies available cannot be negative.");
                _numberOfCopiesAvailable = value;
            }
        }

        public Book(int bookId, string title, string author, int numberOfCopiesAvailable)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty or consist only of whitespace.", nameof(title));

            BookId = bookId;
            Title = title;
            Author = author;
            NumberOfCopiesAvailable = numberOfCopiesAvailable;
        }
    }
}
