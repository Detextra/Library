using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Loan
    {
        public int LoanId { get; set; }
        public DateTime LoanStartDate { get; set; }
        public int Memberid { get; set; }
        public int BookId { get; set; }

        public Loan (int LoanId, DateTime loanStartDate, int memberid, int bookId)
        {
            LoanId = LoanId;
            LoanStartDate = loanStartDate;
            Memberid = memberid;
            BookId = bookId;
        }
    }
}
