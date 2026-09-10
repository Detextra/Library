using Library;
using Library.Entities;

namespace LibraryTests
{
    [TestFixture]
    public class PenaltyManagerTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ApplyPenalty_StandardMember_OnExactDueDate_HasZeroPenalty()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-PenaltyService.StandardMaxDaysLoanDuration), member.MemberId, 100);

            PenaltyService.ApplyPenalty(member, loan);

            // loan duration 21 days, 0 overdue
            Assert.That(member.Balance, Is.EqualTo(0.0));

        }

        [Test]
        public void ApplyPenalty_StandardMember_OneDayOverdue_AppliesSingleDayPenalty()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-(PenaltyService.StandardMaxDaysLoanDuration + 1)), member.MemberId, 100);

            PenaltyService.ApplyPenalty(member, loan);

            // loan duration 21 days, 1 overdue
            Assert.That(member.Balance, Is.EqualTo(-PenaltyService.PenaltyPerDay).Within(0.001m));
        }

        [Test]
        public void ApplyPenalty_StudentMember_OnExactDueDate_HasZeroPenalty()
        {
            var member = new Member(1, isStudent: true, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-PenaltyService.StudentMaxDaysLoanDuration), member.MemberId, 100);

            PenaltyService.ApplyPenalty(member, loan);

            // loan duration 28 days, 0 overdue
            Assert.That(member.Balance, Is.EqualTo(0.0));
        }

        [Test]
        public void ApplyPenalty_AtExactMaxCapThreshold_ReachesCap()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            int exactDaysToCap = PenaltyService.StandardMaxDaysLoanDuration + 50;
            var loan = new Loan(1, DateTime.Now.AddDays(-exactDaysToCap), member.MemberId, 100);

            PenaltyService.ApplyPenalty(member, loan);

            // loan duration 50 days, 39 overdue
            Assert.That(member.Balance, Is.EqualTo(-PenaltyService.PenaltyMax).Within(0.001m));
        }

        [Test]
        public void ApplyPenalty_ExceedingMaxCapThreshold_DoesNotExceedCap()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-500), member.MemberId, 100);

            PenaltyService.ApplyPenalty(member, loan);

            // 500 days overdue = max penalty
            Assert.That(member.Balance, Is.EqualTo(-PenaltyService.PenaltyMax));
        }

        [Test]
        public void PenaltyManager_ApplyPenalty_ThrowsArgumentNullException_WhenMemberOrLoanIsNull()
        {
            var member = new Member(1, isStudent: false, balance: 0.0m);
            var loan = new Loan(1, DateTime.Now.AddDays(-30), memberId: 1, bookId: 10);

            Assert.Throws<ArgumentNullException>(new Action(() => PenaltyService.ApplyPenalty(null, loan)));
            Assert.Throws<ArgumentNullException>(new Action(() => PenaltyService.ApplyPenalty(member, null)));
        }

    }
}