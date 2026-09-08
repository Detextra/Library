using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class LoanManager
    {
        private Dictionary<int, Member> _listMembers;
        private int _nextMemberId = 1;
        private Dictionary<int, Book> _listBooks;
        private int _nextBookId = 1;
        private Dictionary<int, Loan> _listLoans;
        private int _nextLoanId = 1;

        private readonly object _loanLock = new object();

        public LoanManager()
        {
            _listMembers = new Dictionary<int, Member>();
            _listBooks = new Dictionary<int, Book>();
        }



        public Loan MakeLoan (int bookId, int memberId)
        {
            if (_listBooks.TryGetValue(bookId, out Book book) && book.NumberOfCopiesAvailable > 0)
            {
                lock (_listBooks)
                {
                    int uniqueId = _nextLoanId++;
                    Loan newLoan = new Loan(uniqueId, DateTime.Now, memberId, bookId);
                    _listLoans.Add(uniqueId, newLoan);
                }
            }
            return null;
    }
}
