using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    internal class PenaltyManager
    {
        public const int StudentMaxDaysLoanDuration = 28;
        public const int StandardMaxDaysLoanDuration = 21;

        public const double PenaltyPerDay = 0.2;
        public const double PenaltyMax = 10;

        public PenaltyManager ()
        {
            
        }

        public static void ApplyPenalty (Member member, Loan loan)
        {
            int loanDaysDuration = (DateTime.Now - loan.LoanStartDate).Days;
            int maxAllowedDays = member.IsMemberStudent
                ? StudentMaxDaysLoanDuration
                : StandardMaxDaysLoanDuration;
            int penaltyDays = loanDaysDuration - maxAllowedDays;

            if (penaltyDays <= 0) 
                return; 
            lock (member) 
            { 
                member.Balance = Math.Max(10m, member.Balance + penaltyDays * 0.20m);
            }
        }
    }
}
