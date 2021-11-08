using System;

namespace EmailService.Exceptions
{
    public class FailedToSendEmailException : Exception
    {
        public FailedToSendEmailException()
        {
        }

        public FailedToSendEmailException(string message) : base(message)
        {
        }

        public FailedToSendEmailException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
