namespace Library
{
    public class Member
    {
        public int MemberId { get; set; }
        public bool IsMemberStudent { get; set; }
        public double Balance { get; set; }

        public Member (int MemberId, bool IsMemberStudent, double balance)
        {
            this.MemberId = MemberId;
            this.IsMemberStudent = IsMemberStudent;
            this.Balance = balance;
        }

    }
}