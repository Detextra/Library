using Library;
using System;
using System.Collections.Concurrent;
using System.Threading;

public class LibraryManager
{
    private readonly ConcurrentDictionary<int, Book> _listBooks = new();
    private readonly ConcurrentDictionary<int, Loan> _listLoans = new();
    private readonly ConcurrentDictionary<int, Member> _listMembers = new();

    private int _nextLoanId = 0;

    public int StudentMaxDaysLoanDuration { get; set; } = 14;
    public int StandardMaxDaysLoanDuration { get; set; } = 7;

    public Loan MakeLoan(int bookId, int memberId)
    {
        if (!_listBooks.TryGetValue(bookId, out Book book) ||
            !_listMembers.ContainsKey(memberId))
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
            int loanDaysDuration = (DateTime.Now - loan.LoanStartDate).Days;
            int maxAllowedDays = member.IsMemberStudent
                ? StudentMaxDaysLoanDuration
                : StandardMaxDaysLoanDuration;

            int overdueDays = loanDaysDuration - maxAllowedDays;

            if (overdueDays > 0)
            {
                double penalty = overdueDays * 0.10;

                lock (member)
                {
                    member.Balance += penalty;
                }
            }
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
}