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
            Assert.That(member1.isStudent, Is.True);
            Assert.That(member2.isStudent, Is.False);
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
        public void MakeLoan_ShouldThrowBookUnavailableException_WhenBookHasNoCopiesAvailable()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("Lord of the Rings", "John Ronald Reuel Tolkien", 0);

            Assert.Throws<BookUnavailableException>(new Action(() => _libraryManager.MakeLoan(book.BookId, member.MemberId)));
        }

        [Test]
        public void MakeLoan_ShouldEnforceMaxLoansLimit_ForStandardAndStudentMembers()
        {
            var standardMember = _libraryManager.AddMember(isStudent: false); // Limit:3
            var studentMember = _libraryManager.AddMember(isStudent: true);   // Limit:5

            var book = _libraryManager.AddBook("A Song of Ice and Fire", "George R. R. Martin", 10);

            for (int i = 0; i < LibraryManager.StandardMaxLoans; i++)
            {
                _libraryManager.MakeLoan(book.BookId, standardMember.MemberId);
            }
            Assert.Throws<LimitExceededException>(new Action(() => _libraryManager.MakeLoan(book.BookId, standardMember.MemberId)));

            for (int i = 0; i < LibraryManager.StudentMaxLoans; i++)
            {
                _libraryManager.MakeLoan(book.BookId, studentMember.MemberId);
            }
            Assert.Throws<LimitExceededException>(new Action(() => _libraryManager.MakeLoan(book.BookId, studentMember.MemberId)));
        }

        [Test]
        public void MakeLoan_StandardMember_AtExactLimit_SucceedsThenFails()
        {
            var member = _libraryManager.AddMember(isStudent: false); // Max 3 loans
            var book = _libraryManager.AddBook("Test Book", "Author", 10);

            for (int i = 0; i < LibraryManager.StandardMaxLoans; i++)
            {
                _libraryManager.MakeLoan(book.BookId, member.MemberId);
            }

            Assert.Throws<LimitExceededException>(new Action(() => _libraryManager.MakeLoan(book.BookId, member.MemberId)));
        }

        [Test]
        public void MakeLoan_StudentMember_AtExactLimit_SucceedsThenFails()
        {
            var member = _libraryManager.AddMember(isStudent: true); // Max 5 loans
            var book = _libraryManager.AddBook("Test Book", "Author", 10);

            for (int i = 0; i < LibraryManager.StudentMaxLoans; i++)
            {
                _libraryManager.MakeLoan(book.BookId, member.MemberId);
            }

            Assert.Throws<LimitExceededException>(new Action(() => _libraryManager.MakeLoan(book.BookId, member.MemberId)));
        }

        [Test]
        public void ReturnLoan_ShouldIncrementCopies_AndReturnTrue()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("The Stormlight Archive", "Brandon Sanderson", 1);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);
            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(0));

            _libraryManager.ReturnLoan(loan.LoanId);

            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(1));
        }

        [Test]
        public void MakeLoan_WhenCopiesReachZero_RejectsSubsequentRequests()
        {
            var book = _libraryManager.AddBook("Single Copy Book", "Author", 1);
            var member1 = _libraryManager.AddMember(isStudent: false);
            var member2 = _libraryManager.AddMember(isStudent: false);

            var loan1 = _libraryManager.MakeLoan(book.BookId, member1.MemberId);
            Assert.That(loan1, Is.Not.Null);
            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(0));

            Assert.Throws<BookUnavailableException>(new Action(() => _libraryManager.MakeLoan(book.BookId, member2.MemberId)));
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

            double expectedPenalty = -4 * 0.20;
            Assert.That(_libraryManager.GetBalance(member.MemberId), Is.EqualTo(expectedPenalty).Within(0.001));
        }


        [Test]
        public void RemoveMember_ShouldThrow_IfMemberHasActiveLoansOrBalance()
        {
            var member = _libraryManager.AddMember(isStudent: false, initialBalance: 5.0m);
            var book = _libraryManager.AddBook("The Stormlight Archive", "Brandon Sanderson", 2);

            // Fails due to positive balance (not 0)
            Assert.Throws<InvalidOperationExceptionCustom>(new Action(() => _libraryManager.RemoveMember(member.MemberId)));

            member.Balance = 0;

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            // Fails due to active loan
            Assert.Throws<InvalidOperationExceptionCustom>(new Action(() => _libraryManager.RemoveMember(member.MemberId)));

            // Succeeds after loan return
            _libraryManager.ReturnLoan(loan.LoanId);
            Assert.DoesNotThrow(new Action(() => _libraryManager.RemoveMember(member.MemberId)));
        }

        [Test]
        public void RemoveBook_ShouldThrow_IfBookIsCurrentlyOnLoan()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("Steelborn", "Taylor J. LaRue", 2);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            // Fails because copies are out on loan
            Assert.Throws<InvalidOperationExceptionCustom>(new Action(() => _libraryManager.RemoveBook(book.BookId)));

            // Succeeds after loan return
            _libraryManager.ReturnLoan(loan.LoanId);
            Assert.DoesNotThrow(new Action(() => _libraryManager.RemoveBook(book.BookId)));
        }

        [Test]
        public void MakeLoan_ShouldBeThreadSafe_WhenMultipleThreadsBorrowLastCopy()
        {
            var book = _libraryManager.AddBook("Steelborn", "Taylor J. LaRue", 1);

            var m1 = _libraryManager.AddMember(isStudent: false);
            var m2 = _libraryManager.AddMember(isStudent: false);

            Loan loan1 = null;
            Loan loan2 = null;
            int exceptionCount = 0;

            Parallel.Invoke(
                () =>
                {
                    try
                    {
                        loan1 = _libraryManager.MakeLoan(book.BookId, m1.MemberId);
                    }
                    catch (BookUnavailableException)
                    {
                        Interlocked.Increment(ref exceptionCount);
                    }
                },
                () =>
                {
                    try
                    {
                        loan2 = _libraryManager.MakeLoan(book.BookId, m2.MemberId);
                    }
                    catch (BookUnavailableException)
                    {
                        Interlocked.Increment(ref exceptionCount);
                    }
                }
            );

            // Exactly one thread succeeded in creating a loan
            bool onlyOneLoanCreated = (loan1 != null && loan2 == null) || (loan1 == null && loan2 != null);
            Assert.That(onlyOneLoanCreated, Is.True);

            Assert.That(exceptionCount, Is.EqualTo(1));

            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(0));
        }

        [Test]
        public void MakeLoan_ShouldThrowEntityNotFoundException_WhenMemberDoesNotExist()
        {
            var book = _libraryManager.AddBook("Steelborn", "Taylor J. LaRue", 2);
            int invalidMemberId = 9999;

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.MakeLoan(book.BookId, invalidMemberId)));

            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(2));
        }

        [Test]
        public void GetLoans_ShouldReturnEmptyList_WhenMemberDoesNotExist()
        {
            int invalidMemberId = 9999;

            var loans = _libraryManager.GetLoans(invalidMemberId);

            Assert.That(loans, Is.Not.Null);
            Assert.That(loans, Is.Empty);
        }

        [Test]
        public void GetBalance_ShouldReturnZero_WhenMemberDoesNotExist()
        {
            int invalidMemberId = 9999;

            decimal balance = _libraryManager.GetBalance(invalidMemberId);

            Assert.That(balance, Is.EqualTo(0.0));
        }

        [Test]
        public void RemoveMember_ShouldThrowEntityNotFoundException_WhenMemberDoesNotExist()
        {
            int invalidMemberId = 9999;

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.RemoveMember(invalidMemberId)));
        }

        [Test]
        public void MakeLoan_ShouldThrowEntityNotFoundException_WhenBookDoesNotExist()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            int invalidBookId = 8888;

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.MakeLoan(invalidBookId, member.MemberId)));

            Assert.That(_libraryManager.GetLoans(member.MemberId), Is.Empty);
        }

        [Test]
        public void RemoveBook_ShouldThrowEntityNotFoundException_WhenBookDoesNotExist()
        {
            int invalidBookId = 8888;

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.RemoveBook(invalidBookId)));
        }

        [Test]
        public void ReturnLoan_ShouldThrowEntityNotFoundException_WhenLoanIdDoesNotExist()
        {
            int invalidLoanId = 7777;

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.ReturnLoan(invalidLoanId)));
        }

        [Test]
        public void ReturnLoan_ShouldThrow_WhenReturningSameLoanIdTwice()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("Steelborn", "Taylor J. LaRue", 1);
            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            Assert.DoesNotThrow(new Action(() => _libraryManager.ReturnLoan(loan.LoanId)));

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.ReturnLoan(loan.LoanId)));

            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(1));
        }


        [Test]
        public void MakeLoan_ShouldThrowEntityNotFoundException_WhenBothBookAndMemberDoNotExist()
        {
            int invalidBookId = 8888;
            int invalidMemberId = 9999;

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.MakeLoan(invalidBookId, invalidMemberId)));
        }

        [Test]
        public void MakeLoan_ThrowsEntityNotFoundException_WhenMemberDoesNotExist()
        {
            var book = _libraryManager.AddBook("A Song of Ice and Fire", "George R. R. Martin", 2);

            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.MakeLoan(book.BookId, memberId: 999)));
        }

        [Test]
        public void MakeLoan_ThrowsBookUnavailableException_WhenCopiesAreZero()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("A Song of Ice and Fire", "George R. R. Martin", 0);

            Assert.Throws<BookUnavailableException>(new Action(() => _libraryManager.MakeLoan(book.BookId, member.MemberId)));
        }

        [Test]
        public void ReturnLoan_ThrowsEntityNotFoundException_WhenLoanDoesNotExist()
        {
            Assert.Throws<EntityNotFoundException>(new Action(() => _libraryManager.ReturnLoan(loanId: 888)));
        }

        [Test]
        public void RemoveMember_ThrowsInvalidOperationException_WhenMemberHasActiveLoan()
        {
            var member = _libraryManager.AddMember(isStudent: false);
            var book = _libraryManager.AddBook("A Song of Ice and Fire", "George R. R. Martin", 2);

            var loan = _libraryManager.MakeLoan(book.BookId, member.MemberId);

            Assert.Throws<InvalidOperationExceptionCustom>(new Action(() => _libraryManager.RemoveMember(member.MemberId)));
        }
    }
}