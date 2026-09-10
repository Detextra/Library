namespace Library.Entities
{
    public class Loan
    {
        public int LoanId { get; set; }
        public DateTime LoanStartDate { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }

        public Loan(int loanId, DateTime loanStartDate, int memberId, int bookId)
        {
            LoanId = loanId;
            LoanStartDate = loanStartDate;
            MemberId = memberId;
            BookId = bookId;
        }
    }
}
