//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using System;

namespace CodeFactory.NDF
{
    /// <summary>
    /// Notifies that an exception has occurred within the applications executing logic that was not handled and a application safe message has been provided with the exception.
    /// </summary>
    public class UnhandledException:ManagedException
    {
        /// <summary>
        /// Creates an instance of <see cref="UnhandledException"/> and returns the default exception message.
        /// </summary>
        public UnhandledException() : base(StandardExceptionMessages.UnhandledException)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="UnhandledException"/> that returns the default exception message and an embedded exception.
        /// </summary>
        /// <param name="internalException">Existing exception to be added to this exception.</param>
        public UnhandledException(Exception internalException) : base(StandardExceptionMessages.UnhandledException, internalException)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="UnhandledException"/> exception class.
        /// </summary>
        /// <param name="message">Message to be returned as part of the exception.</param>
        public UnhandledException(string message) : base(message)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="UnhandledException"/> exception class.
        /// </summary>
        /// <param name="message">Message to be returned as part of the exception.</param>
        /// <param name="internalException">Existing exception to be added to this exception.</param>
        public UnhandledException(string message, Exception internalException) : base(message, internalException)
        {
            //Intentionally blank
        }
    }
}
