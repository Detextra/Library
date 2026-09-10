using Library;
using Library.Entities;

namespace LibraryTests
{
    [TestFixture]
    public class BookTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Book_Constructor_ThrowsArgumentException_WhenTitleIsEmpty()
        {
            Assert.Throws<ArgumentException>(new Action(() => new Book(1, "", "Valid Author", 2)));
            Assert.Throws<ArgumentException>(new Action(() => new Book(1, "   ", "Valid Author", 2)));
        }

        [Test]
        public void Book_Constructor_AllowsEmptyOrWhitespaceAuthor()
        {
            Assert.DoesNotThrow(new Action(() => new Book(1, "Valid Title", "", 2)));
            Assert.DoesNotThrow(new Action(() => new Book(1, "Valid Title", "   ", 2)));
        }




        [Test]
        public void Book_NumberOfCopiesAvailable_ThrowsArgumentOutOfRangeException_WhenSetToNegative()
        {
            var book = new Book(1, "A Song of Ice and Fire", "George R. R. Martin", 2);

            Assert.Throws<ArgumentOutOfRangeException>(new Action(() => book.NumberOfCopiesAvailable = -1));
        }
    }
}