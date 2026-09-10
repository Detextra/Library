using Library;
using System.Collections.Concurrent;
public class LibraryManager
{
    private readonly ConcurrentDictionary<int, Book> _listBooks = new();
    private int _nextBookId = 1;
    private readonly ConcurrentDictionary<int, Loan> _listLoans = new();
    private int _nextLoanId = 1;
    private readonly ConcurrentDictionary<int, Member> _listMembers = new();
    private int _nextMemberId = 1;

    public const int StudentMaxLoans = 5;
    public const int StandardMaxLoans = 3;

    public Loan MakeLoan(int bookId, int memberId)
    {
        if (!_listMembers.TryGetValue(memberId, out var member))
        {
            throw new EntityNotFoundException($"Member with ID {memberId} was not found.");
        }

        if (!_listBooks.TryGetValue(bookId, out var book))
        {
            throw new EntityNotFoundException($"Book with ID {bookId} was not found.");
        }

        int maxAllowedLoans = member.isStudent ? StudentMaxLoans : StandardMaxLoans;
        if (GetLoans(memberId).Count >= maxAllowedLoans)
        {
            throw new LimitExceededException($"Member {memberId} has reached the maximum loan limit of {maxAllowedLoans}.");
        }

        lock (book)
        {
            if (book.NumberOfCopiesAvailable <= 0)
            {
                throw new BookUnavailableException($"Book ID {bookId} ('{book.Title}') has no available copies.");
            }
            book.NumberOfCopiesAvailable--;
        }

        int uniqueId = Interlocked.Increment(ref _nextLoanId);
        Loan newLoan = new Loan(uniqueId, DateTime.Now, memberId, bookId);

        if (_listLoans.TryAdd(uniqueId, newLoan))
        {
            return newLoan;
        }

        lock (book)
        {
            book.NumberOfCopiesAvailable++;
        }

        throw new InvalidOperationExceptionCustom($"Failed to create loan record for Book ID {bookId} and Member ID {memberId}.");
    }

    public void ReturnLoan(int loanId)
    {
        if (!_listLoans.TryRemove(loanId, out var loan))
        {
            throw new EntityNotFoundException($"Loan with ID {loanId} was not found or has already been returned.");
        }

        if (_listMembers.TryGetValue(loan.MemberId, out var member))
        {
            PenaltyManager.ApplyPenalty(member, loan);
        }

        if (_listBooks.TryGetValue(loan.BookId, out var book))
        {
            lock (book)
            {
                book.NumberOfCopiesAvailable++;
            }
        }
    }

    public List<Loan> GetLoans (int memberId)
    {
        List<Loan> loans = new List<Loan>();
        foreach (Loan n in _listLoans.Values)
        {
            if (n.MemberId == memberId)
            {
                loans.Add(n);
            }
        }
        return loans;
    }

    public decimal GetBalance(int memberId)
    {
        if (_listMembers.TryGetValue(memberId, out var member))
        {
            lock (member)
            {
                return member.Balance;
            }
        }
        return 0;
    }

    public Member AddMember(bool isStudent, decimal initialBalance = 0.0m)
    {
        int id = Interlocked.Increment(ref _nextMemberId);
        Member member = new Member(id, isStudent, initialBalance);
        _listMembers.TryAdd(id, member);
        return member;
    }

    public void RemoveMember(int memberId)
    {
        if (!_listMembers.TryGetValue(memberId, out var member))
        {
            throw new EntityNotFoundException($"Member with ID {memberId} was not found.");
        }

        if (GetLoans(memberId).Count > 0)
        {
            throw new InvalidOperationExceptionCustom($"Cannot remove Member {memberId}: Member currently has active loans.");
        }

        lock (member)
        {
            if (member.Balance > 0)
            {
                throw new InvalidOperationExceptionCustom($"Cannot remove Member {memberId}: Member has an outstanding balance of {member.Balance:C}.");
            }
        }

        _listMembers.TryRemove(memberId, out _);
    }

    public Book AddBook(string title, string author, int copies)
    {
        int id = Interlocked.Increment(ref _nextBookId);
        Book book = new Book(id, title, author, copies);
        _listBooks.TryAdd(id, book);
        return book;
    }

    public void RemoveBook(int bookId)
    {
        if (!_listBooks.ContainsKey(bookId))
        {
            throw new EntityNotFoundException($"Book with ID {bookId} was not found.");
        }

        foreach (Loan loan in _listLoans.Values)
        {
            if (loan.BookId == bookId)
            {
                throw new InvalidOperationExceptionCustom($"Cannot remove Book {bookId}: Book is currently on loan.");
            }
        }

        _listBooks.TryRemove(bookId, out _);
    }
}

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message) { }
}

public class BookUnavailableException : Exception
{
    public BookUnavailableException(string message) : base(message) { }
}

public class LimitExceededException : Exception
{
    public LimitExceededException(string message) : base(message) { }
}

public class InvalidOperationExceptionCustom : Exception
{
    public InvalidOperationExceptionCustom(string message) : base(message) { }
}