using System;

namespace GoferEx.Core
{
    public class ErrorMessage
    {
        public string UserMessage { get; }
        public Exception ExceptionDetails { get; }

        public ErrorMessage(string userMessage, Exception exceptionDetails)
        {
            UserMessage = userMessage;
            ExceptionDetails = exceptionDetails;
        }
    }
}