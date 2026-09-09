using System.Threading.Tasks;
using static Microsoft.Extensions.DependencyModel.Library;
using NUnit.Framework;
using Library;


namespace LibraryTests
{
    [TestFixture]
    public class LibraryManagerTests
    {
        private LibraryManager _libraryManager;

        [SetUp]
        public void Setup()
        {
            _libraryManager = new LibraryManager();
        }


        [Test]
        public void AddMember_ShouldCreateMemberWithUniqueId()
        {
            var member1 = _libraryManager.AddMember(isStudent: true);
            var member2 = _libraryManager.AddMember(isStudent: false);

            Assert.That(member1, Is.Not.Null);
            Assert.That(member2, Is.Not.Null);
            Assert.That(member1.MemberId, Is.Not.EqualTo(member2.MemberId));
            Assert.That(member1.IsMemberStudent, Is.True);
            Assert.That(member2.IsMemberStudent, Is.False);
        }

        [Test]
        public void AddBook_ShouldCreateBookWithUniqueId()
        {
            var book1 = _libraryManager.AddBook("Lord of the Rings", "John Ronald Reuel Tolkien", 3);
            var book2 = _libraryManager.AddBook("A Song of Ice and Fire", "George R. R. Martin", 2);

            Assert.That(book1, Is.Not.Null);
            Assert.That(book2, Is.Not.Null);
            Assert.That(book1.BookId, Is.Not.EqualTo(book2.BookId));
            Assert.That(book1.NumberOfCopiesAvailable, Is.EqualTo(3));
        }

        [Test]
        public void MakeLoan_ShouldSucceed_WhenValidMemberAndAvailableBook()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("Lord of the Rings", "John Ronald Reuel Tolkien", 2);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            Assert.That(loan, Is.Not.Null);
            Assert.That(loan.BookId, Is.EqualTo(book.BookId));
            Assert.That(loan.MemberId, Is.EqualTo(member.MemberId));
            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(1));
        }

        [Test]
        public void MakeLoan_ShouldFail_WhenBookHasNoCopiesAvailable()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("Lord of the Rings", "John Ronald Reuel Tolkien", 0);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            Assert.That(loan, Is.Null);
        }

        [Test]
        public void MakeLoan_ShouldEnforceMaxLoansLimit_ForStandardAndStudentMembers()
        {
            var standardMember = _libraryManager.AddMember(isStudent: false);// Limit: 3
            var studentMember = _libraryManager.AddMember(isStudent: true);// Limit: 5

            var book = _libraryManager.AddBook("A Song of Ice and Fire", "George R. R. Martin", 10);

            //Standard Member
            for (int i = 0; i < LibraryManager.StandardMaxLoans; i++)
            {
                Assert.That(_libraryManager.MakeLoan(book.BookId, standardMember.MemberId), Is.Not.Null);
            }
            Assert.That(_libraryManager.MakeLoan(book.BookId, standardMember.MemberId), Is.Null);

            //Student Member
            for (int i = 0; i < LibraryManager.StudentMaxLoans; i++)
            {
                Assert.That(_libraryManager.MakeLoan(book.BookId, studentMember.MemberId), Is.Not.Null);
            }
            Assert.That(_libraryManager.MakeLoan(book.BookId, studentMember.MemberId), Is.Null);
        }

        [Test]
        public void ReturnLoan_ShouldIncrementCopies_AndReturnTrue()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("The Stormlight Archive", "Brandon Sanderson", 1);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);
            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(0));

            bool success = _libraryManager.ReturnLoan(loan.LoanId);

            Assert.That(success, Is.True);
            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(1));
        }

        [Test]
        public void ReturnLoan_ShouldApplyFine_WhenLoanIsOverdue()
        {
            var member = _libraryManager.AddMember(isStudent: false); //Max 21 days
            var book = _libraryManager.AddBook("The Stormlight Archive", "Brandon Sanderson", 1);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            // 4 days extratime
            loan.LoanStartDate = DateTime.Now.AddDays(-25);

            _libraryManager.ReturnLoan(loan.LoanId);

            double expectedPenalty = 4 * 0.20;
            Assert.That(_libraryManager.GetBalance(member.MemberId), Is.EqualTo(expectedPenalty).Within(0.001));
        }


        [Test]
        public void RemoveMember_ShouldFail_IfMemberHasActiveLoansOrBalance()
        {
            var member = _libraryManager.AddMember(isStudent: false, initialBalance: 5.0m);
            var book = _libraryManager.AddBook("The Stormlight Archive", "Brandon Sanderson", 2);

            Assert.That(_libraryManager.RemoveMember(member.MemberId), Is.False);

            member.Balance = 0;

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);
            Assert.That(_libraryManager.RemoveMember(member.MemberId), Is.False);

            _libraryManager.ReturnLoan(loan.LoanId);
            Assert.That(_libraryManager.RemoveMember(member.MemberId), Is.True);
        }

        [Test]
        public void RemoveBook_ShouldFail_IfBookIsCurrentlyOnLoan()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("Steelborn", "Taylor J. LaRue", 2);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            Assert.That(_libraryManager.RemoveBook(book.BookId), Is.False);

            _libraryManager.ReturnLoan(loan.LoanId);
            Assert.That(_libraryManager.RemoveBook(book.BookId), Is.True);
        }

        [Test]
        public void MakeLoan_ShouldBeThreadSafe_WhenMultipleThreadsBorrowLastCopy()
        {
            var book = _libraryManager.AddBook("Steelborn", "Taylor J. LaRue", 1);

            var m1 = _libraryManager.AddMember(isStudent: false);
            var m2 = _libraryManager.AddMember(isStudent: false);

            Loan loan1 = null;
            Loan loan2 = null;

            Parallel.Invoke(
                () => loan1 = _libraryManager.MakeLoan(book.BookId, m1.MemberId),
                () => loan2 = _libraryManager.MakeLoan(book.BookId, m2.MemberId)
            );

            bool onlyOneLoanCreated = (loan1 != null && loan2 == null) || (loan1 == null && loan2 != null);
            Assert.That(onlyOneLoanCreated, Is.True);
            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(0));
        }
    }
}