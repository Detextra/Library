namespace Library
{
    public class Member
    {
        public int MemberId { get; set; }
        public bool isStudent { get; set; }
        public decimal Balance { get; set; }

        public Member (int MemberId, bool isStudent, decimal balance)
        {
            this.MemberId = MemberId;
            this.isStudent = isStudent;
            this.Balance = balance;
        }

    }
}