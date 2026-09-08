using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Member
    {
        public int MemberId;
        public bool IsMemberStudent;
        private LoanController loanController;

        public Member (int MemberId, bool IsMemberStudent)
        {
            this.MemberId = MemberId;
            this.IsMemberStudent = IsMemberStudent;
        }

        public List<Loan> GetLoans()
        {
            return loanController.GetLoans(this.MemberId);
        }

        public bool MakeLoan (int bookId)
        {
            Loan newLoan = loanController.MakeLoan(bookId, MemberId);
            if ( newLoan != null)
            {
                Loans.Add(newLoan.LoanId, newLoan);
                return true; 
            }
            return false;
        }
    }
}