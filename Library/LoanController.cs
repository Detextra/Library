namespace Library
{
    public class LoanController
    {
        private LibraryManager librairyManager { get; set; }

        public LoanController(LibraryManager librairyManager)
        {
            this.librairyManager = librairyManager;
        }

        public Loan MakeLoan (int bookId, int memberId)
        {
            return librairyManager.MakeLoan(bookId, memberId);
        }

        public List<Loan> GetLoans (int memberId)
        {
             return librairyManager.GetLoans(memberId);
        }

        public decimal GetBalance(int memberId)
        {
            return librairyManager.GetBalance(memberId);
        }
    }

}
