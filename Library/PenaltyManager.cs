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
            int loanDaysDuration = (DateTime.Now - loan.LoanStartDate).Days;
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
