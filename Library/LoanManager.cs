using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class LoanManager
    {
        public List<Member> ListMembers;
        public List<Book> ListBooks;

        public LoanManager()
        {
            ListMembers = new List<Member>();
            ListBooks = new List<Book>();
        }
    }
}
