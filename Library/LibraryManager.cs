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
        if (!_listMembers.TryGetValue(memberId, out Member member) ||
            !_listBooks.TryGetValue(bookId, out Book book))
        {
            return null;
        }

        int maxAllowedLoans = member.IsMemberStudent ? StudentMaxLoans : StandardMaxLoans;
        if (GetLoans(memberId).Count >= maxAllowedLoans)
        {
            return null;
        }

        lock (book)
        {
            if (book.NumberOfCopiesAvailable <= 0)
            {
                return null;
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

        return null;
    }

    public bool ReturnLoan(int loanId)
    {
        if (!_listLoans.TryRemove(loanId, out Loan loan))
        {
            return false;
        }

        if (_listMembers.TryGetValue(loan.MemberId, out Member member))
        {
            PenaltyManager.ApplyPenalty(member, loan);
        }

        if (_listBooks.TryGetValue(loan.BookId, out Book book))
        {
            lock (book)
            {
                book.NumberOfCopiesAvailable++;
            }
        }

        return true;
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
        if (_listMembers.TryGetValue(memberId, out Member member))
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

    public bool RemoveMember(int memberId)
    {
        if (GetLoans(memberId).Count > 0)
        {
            return false;
        }

        if (_listMembers.TryGetValue(memberId, out Member member))
        {
            lock (member)
            {
                if (member.Balance > 0)
                {
                    return false;
                }
            }
        }

        return _listMembers.TryRemove(memberId, out _);
    }

    public Book AddBook(string title, string author, int copies)
    {
        int id = Interlocked.Increment(ref _nextBookId);
        Book book = new Book(id, title, author, copies);
        _listBooks.TryAdd(id, book);
        return book;
    }

    public bool RemoveBook(int bookId)
    {
        foreach (Loan loan in _listLoans.Values)
        {
            if (loan.BookId == bookId)
            {
                return false;
            }
        }

        return _listBooks.TryRemove(bookId, out _);
    }
}