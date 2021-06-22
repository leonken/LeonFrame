using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions
{
    /// <summary>
    /// 领域异常
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException()
        { }

        public DomainException(string message)
            : base(message)
        { }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
