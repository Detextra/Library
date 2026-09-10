using Library.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IRepository
{
    public interface ILoanRepository
    {
        Loan Add(DateTime loanDate, int memberId, int bookId);
        Loan? GetById(int id);
        IEnumerable<Loan> GetByMemberId(int memberId);
        IEnumerable<Loan> GetByBookId(int bookId);
        bool Remove(int id);
    }

    public class InMemoryLoanRepository : ILoanRepository
    {
        private readonly ConcurrentDictionary<int, Loan> _loans = new();
        private int _nextId = 1;

        public Loan Add(DateTime loanDate, int memberId, int bookId)
        {
            int id = Interlocked.Increment(ref _nextId);
            Loan loan = new Loan(id, memberId, bookId, loanDate);
            _loans.TryAdd(id, loan);
            return loan;
        }

        public Loan? GetById(int id) => _loans.TryGetValue(id, out var loan) ? loan : null;

        public IEnumerable<Loan> GetByMemberId(int memberId)
            => _loans.Values.Where(l => l.MemberId == memberId);

        public IEnumerable<Loan> GetByBookId(int bookId)
            => _loans.Values.Where(l => l.BookId == bookId);

        public bool Remove(int id) => _loans.TryRemove(id, out _);
    }
}
