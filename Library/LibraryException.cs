using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message) { }
    }

    public class BookUnavailableException : Exception
    {
        public BookUnavailableException(string message) : base(message) { }
    }

    public class LimitExceededException : Exception
    {
        public LimitExceededException(string message) : base(message) { }
    }

    public class InvalidOperationExceptionCustom : Exception
    {
        public InvalidOperationExceptionCustom(string message) : base(message) { }
    }
}
