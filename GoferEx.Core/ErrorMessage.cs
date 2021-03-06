﻿using System;

namespace GoferEx.Core
{
    /// <summary>
    /// This is a wrapping for an error occured at the server
    /// </summary>
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