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
        public void MakeLoan_StandardMember_AtExactLimit_SucceedsThenFails()
        {
            var member = _libraryManager.AddMember(isStudent: false); // Max 3 loans
            var book = _libraryManager.AddBook("Test Book", "Author", 10);

            for (int i = 0; i < LibraryManager.StandardMaxLoans; i++)
            {
                Assert.That(_libraryManager.MakeLoan(book.BookId, member.MemberId), Is.Not.Null);
            }

            Assert.That(_libraryManager.MakeLoan(book.BookId, member.MemberId), Is.Null);
        }

        [Test]
        public void MakeLoan_StudentMember_AtExactLimit_SucceedsThenFails()
        {
            var member = _libraryManager.AddMember(isStudent: true); // Max 5 loans
            var book = _libraryManager.AddBook("Test Book", "Author", 10);

            for (int i = 0; i < LibraryManager.StudentMaxLoans; i++)
            {
                Assert.That(_libraryManager.MakeLoan(book.BookId, member.MemberId), Is.Not.Null);
            }

            Assert.That(_libraryManager.MakeLoan(book.BookId, member.MemberId), Is.Null);
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
        public void MakeLoan_WhenCopiesReachZero_RejectsSubsequentRequests()
        {
            var book = _libraryManager.AddBook("Single Copy Book", "Author", 1);
            var member1 = _libraryManager.AddMember(isStudent: false);
            var member2 = _libraryManager.AddMember(isStudent: false);

            var loan1 = _libraryManager.MakeLoan(book.BookId, member1.MemberId);
            Assert.That(loan1, Is.Not.Null);
            Assert.That(book.NumberOfCopiesAvailable, Is.EqualTo(0));

            var loan2 = _libraryManager.MakeLoan(book.BookId, member2.MemberId);
            Assert.That(loan2, Is.Null);
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
        public void ApplyPenalty_StandardMember_OnExactDueDate_HasZeroPenalty()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-PenaltyManager.StandardMaxDaysLoanDuration), member.MemberId, 100);

            PenaltyManager.ApplyPenalty(member, loan);

            // loan duration 21 days, 0 overdue
            Assert.That(member.Balance, Is.EqualTo(0.0));
        }

        [Test]
        public void ApplyPenalty_StandardMember_OneDayOverdue_AppliesSingleDayPenalty()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-(PenaltyManager.StandardMaxDaysLoanDuration + 1)), member.MemberId, 100);

            PenaltyManager.ApplyPenalty(member, loan);

            // loan duration 21 days, 1 overdue
            Assert.That(member.Balance, Is.EqualTo(-PenaltyManager.PenaltyPerDay).Within(0.001m));
        }

        [Test]
        public void ApplyPenalty_StudentMember_OnExactDueDate_HasZeroPenalty()
        {
            var member = new Member(1, isStudent: true, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-PenaltyManager.StudentMaxDaysLoanDuration), member.MemberId, 100);

            PenaltyManager.ApplyPenalty(member, loan);

            // loan duration 28 days, 0 overdue
            Assert.That(member.Balance, Is.EqualTo(0.0));
        }

        [Test]
        public void ApplyPenalty_AtExactMaxCapThreshold_ReachesCap()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            int exactDaysToCap = PenaltyManager.StandardMaxDaysLoanDuration + 50;
            var loan = new Loan(1, DateTime.Now.AddDays(-exactDaysToCap), member.MemberId, 100);

            PenaltyManager.ApplyPenalty(member, loan);

            // loan duration 50 days, 39 overdue
            Assert.That(member.Balance, Is.EqualTo(-PenaltyManager.PenaltyMax).Within(0.001m));
        }

        [Test]
        public void ApplyPenalty_ExceedMaxCapThreshold()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            int exactDaysToCap = PenaltyManager.StandardMaxDaysLoanDuration + 100;
            var loan = new Loan(1, DateTime.Now.AddDays(-exactDaysToCap), member.MemberId, 100);

            PenaltyManager.ApplyPenalty(member, loan);

            // loan duration 100 days, 89 overdue
            Assert.That(member.Balance, Is.EqualTo(-PenaltyManager.PenaltyMax).Within(0.001m));
        }

        [Test]
        public void ApplyPenalty_ExceedingMaxCapThreshold_DoesNotExceedCap()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            // 500 days ago (Far exceeds cap limit)
            var loan = new Loan(1, DateTime.Now.AddDays(-500), member.MemberId, 100);

            PenaltyManager.ApplyPenalty(member, loan);

            // Boundary test: Max Penalty Cap constraint
            Assert.That(member.Balance, Is.EqualTo(-PenaltyManager.PenaltyMax));
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