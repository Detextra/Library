using Library;
using Library.Entities;
using Library.IRepository;
using System.Collections.Concurrent;
public class LibraryManager
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILoanRepository _loanRepository;

    public const int StudentMaxLoans = 5;
    public const int StandardMaxLoans = 3;

    public LibraryManager(
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        ILoanRepository loanRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
    }

    public Loan MakeLoan(int bookId, int memberId)
    {
        var member = _memberRepository.GetById(memberId)
            ?? throw new EntityNotFoundException($"Member with ID {memberId} was not found.");

        var book = _bookRepository.GetById(bookId)
            ?? throw new EntityNotFoundException($"Book with ID {bookId} was not found.");

        lock (member)
        {
            int maxAllowedLoans = member.isStudent ? StudentMaxLoans : StandardMaxLoans;
            if (_loanRepository.GetByMemberId(memberId).Count() >= maxAllowedLoans)
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
        }

        return _loanRepository.Add(DateTime.Now, memberId, bookId);
    }

    public void ReturnLoan(int loanId)
    {
        var loan = _loanRepository.GetById(loanId)
            ?? throw new EntityNotFoundException($"Loan with ID {loanId} was not found.");

        _loanRepository.Remove(loanId);

        var member = _memberRepository.GetById(loan.MemberId);
        if (member != null)
        {
            PenaltyManager.ApplyPenalty(member, loan);
        }

        var book = _bookRepository.GetById(loan.BookId);
        if (book != null)
        {
            lock (book)
            {
                book.NumberOfCopiesAvailable++;
            }
        }
    }

    public List<Loan> GetLoans(int memberId)
        => _loanRepository.GetByMemberId(memberId).ToList();

    public decimal GetBalance(int memberId)
    {
        var member = _memberRepository.GetById(memberId);
        if (member == null) return 0m;

        lock (member)
        {
            return member.Balance;
        }
    }

    public Member AddMember(bool isStudent, decimal initialBalance = 0.0m)
        => _memberRepository.Add(isStudent, initialBalance);

    public void RemoveMember(int memberId)
    {
        var member = _memberRepository.GetById(memberId)
            ?? throw new EntityNotFoundException($"Member with ID {memberId} was not found.");

        if (_loanRepository.GetByMemberId(memberId).Any())
        {
            throw new InvalidOperationExceptionCustom($"Cannot remove Member {memberId}: Member currently has active loans.");
        }

        lock (member)
        {
            if (member.Balance != 0)
            {
                throw new InvalidOperationExceptionCustom($"Cannot remove Member {memberId}: Member has outstanding penalties.");
            }
        }

        _memberRepository.Remove(memberId);
    }

    public Book AddBook(string title, string author, int copies)
        => _bookRepository.Add(title, author, copies);

    public void RemoveBook(int bookId)
    {
        var book = _bookRepository.GetById(bookId)
            ?? throw new EntityNotFoundException($"Book with ID {bookId} was not found.");

        lock (book)
        {
            if (_loanRepository.GetByBookId(bookId).Any())
            {
                throw new InvalidOperationExceptionCustom($"Cannot remove Book {bookId}: Book is currently on loan.");
            }

            _bookRepository.Remove(bookId);
        }
    }
}

