using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class LibraryManager
    {
        private Dictionary<int, Member> _listMembers;
        private int _nextMemberId = 1;
        private Dictionary<int, Book> _listBooks;
        private int _nextBookId = 1;
        private Dictionary<int, Loan> _listLoans;
        private int _nextLoanId = 1;

        public LibraryManager()
        {
            _listMembers = new Dictionary<int, Member>();
            _listBooks = new Dictionary<int, Book>();
        }

        public bool AddMember (bool IsMemberStudent)
        {
            lock (_listMembers)
            {
                int uniqueId = _nextMemberId++;
                Member newMember = new Member(uniqueId, IsMemberStudent);
                _listMembers.Add(uniqueId, newMember);
                return true;
            }
            return false;
        }

        public bool RemoveMember (int MemberId)
        {
            lock (_listMembers)
            {
                _listMembers.Remove(MemberId);
                return true;
            }
            return false;
        }

        public bool ChangeMemberType(int MemberId)
        {
            lock (_listMembers)
            {
                _listMembers.TryGetValue(MemberId, out Member m);
                m.IsMemberStudent = !m.IsMemberStudent;
                return true;
            }
            return false;
        }

        public bool AddBook(string title, string author, int numberOfCopiesAvailable)
        {
            lock (_listBooks)
            {
                int uniqueId = _nextBookId++;
                Book newBook = new Book(uniqueId, title, author, numberOfCopiesAvailable);
                _listBooks.Add(uniqueId, newBook);
                return true;
            }
            return false;
        }

        public bool RemoveBook (int BookId)
        {
            lock (_listBooks)
            {
                _listBooks.Remove(BookId);
                return true;
            }
            return false;
        }

        public bool AdjustBookCopies (int bookId, int increase)
        {
            lock (_listBooks)
            {
                _listBooks.TryGetValue(bookId, out Book b);
                b.NumberOfCopiesAvailable += increase;
                return true;
            }
            return bool;
        }

        public Loan MakeLoan(int bookId, int memberId)
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

        // does member make and return a loan on its page?
        public bool ReturnLoan (int loanId)
        {
            lock (_listBooks)
            {
                _listBooks.Remove(loanId);
                return true;
            }
            return false;
        }

        public List<Loan> GetLoans (int memberId)
        {
            List<Loan> loans = new List<Loan>();
            foreach( Loan l in _listLoans.Values)
            {
                if (l.MemberId == memberId)
                    loans.Add(l);
            }
            return loans;
        }
    }
}
