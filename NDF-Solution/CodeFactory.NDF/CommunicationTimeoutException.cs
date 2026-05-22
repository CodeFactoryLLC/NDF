//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2026 CodeFactory, LLC
//*****************************************************************************
using System;

namespace CodeFactory.NDF
{
    /// <summary>
    /// Notifies that a communications timeout exception has occurred and application safe message has been provided with the exception.
    /// </summary>
    public class CommunicationTimeoutException:CommunicationException
    {
        /// <summary>
        /// Creates an instance of <see cref="CommunicationTimeoutException"/> and returns the default exception message.
        /// </summary>
        public CommunicationTimeoutException() : base(StandardExceptionMessages.CommunicationTimeoutException)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="CommunicationTimeoutException"/> that returns the default exception message and an embedded exception.
        /// </summary>
        /// <param name="internalException">Existing exception to be added to this exception.</param>
        public CommunicationTimeoutException(Exception internalException) : base(StandardExceptionMessages.CommunicationTimeoutException, internalException)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="CommunicationTimeoutException"/> exception class.
        /// </summary>
        /// <param name="message">Message to be returned as part of the exception.</param>
        public CommunicationTimeoutException(string message) : base(message)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="CommunicationException"/> exception class.
        /// </summary>
        /// <param name="message">Message to be returned as part of the exception.</param>
        /// <param name="internalException">Existing exception to be added to this exception.</param>
        public CommunicationTimeoutException(string message, Exception internalException) : base(message, internalException)
        {
            //Intentionally blank
        }
    }

}
