using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class LoanController
    {
        private LoanManager loanManager { get; set; }

        public LoanController(LoanManager loanManager)
        {
            this.loanManager = loanManager;
        }

        public Loan MakeLoan (int loanId, int memberId)
        {
            return loanManager.MakeLoan(loanId, memberId);
        }
    }

}
