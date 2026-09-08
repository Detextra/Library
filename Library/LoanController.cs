using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class LoanController
    {
        private LibraryManager librairyManager { get; set; }

        public LoanController(LibraryManager librairyManager)
        {
            this.librairyManager = librairyManager;
        }

        public Loan MakeLoan (int loanId, int memberId)
        {
            return librairyManager.MakeLoan(loanId, memberId);
        }

        public List<Loan> GetLoans (int memberId)
        {
             return librairyManager.GetLoans(memberId);
        }
    }

}
