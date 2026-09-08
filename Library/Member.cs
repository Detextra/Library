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
        public Dictionary<int, Loan> Loans;
        private LoanController loanController;

        Member(int MemberId, bool IsMemberStudent, LoanController loanController)
        {
            this.MemberId = MemberId;
            this.IsMemberStudent = IsMemberStudent;
            this.loanController = loanController;
        }

        public Dictionary<int, Loan> GetLoans()
        {
            return Loans;
        }

        public bool MakeLoan (int bookId)
        {
            Loan newLoan = loanController.MakeLoan(bookId, MemberId);
            if ( newLoan != null)
            {
                Loans.Add(newLoan.LoanId, newLoan);
                return true; 
            }
            return false;}
        }
    }
}
