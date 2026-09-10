namespace Library
{
    public static class PenaltyManager
    {
        public const int StudentMaxDaysLoanDuration = 28;
        public const int StandardMaxDaysLoanDuration = 21;

        public const decimal PenaltyPerDay = 0.2m;
        public const decimal PenaltyMax = 10.0m;

        public static void ApplyPenalty (Member member, Loan loan)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member), "Member cannot be null when calculating penalties.");

            if (loan == null)
                throw new ArgumentNullException(nameof(loan), "Loan cannot be null when calculating penalties.");

            int loanDaysDuration = (int)(DateTime.Now - loan.LoanStartDate).TotalDays;
            int maxAllowedDays = member.isStudent
                ? StudentMaxDaysLoanDuration
                : StandardMaxDaysLoanDuration;
            int penaltyDays = loanDaysDuration - maxAllowedDays;

            if (penaltyDays <= 0) 
                return; 
            lock (member) 
            {
                member.Balance -= Math.Min(penaltyDays * PenaltyPerDay, PenaltyMax);
            }
        }
    }
}
