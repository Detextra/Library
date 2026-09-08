using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Member
    {
        public int MemberId;
        public bool IsMemberStudent;
        public double Balance;

        public Member (int MemberId, bool IsMemberStudent, double balance)
        {
            this.MemberId = MemberId;
            this.IsMemberStudent = IsMemberStudent;
            this.Balance = balance;
        }

    }
}