using System.Data;

namespace Library.Entities
{
    public class Loan
    {
        public int LoanId { get; set; }
        public DateTime LoanStartDate { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }

        public Loan(int loanId, int memberId, int bookId, DateTime loanStartDate = default)
        {
            LoanId = loanId;
            LoanStartDate = loanStartDate == default ? DateTime.Now : loanStartDate; ;
            MemberId = memberId;
            BookId = bookId;
        }
    }
}
