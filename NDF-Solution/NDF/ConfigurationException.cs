﻿//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using System;

namespace NDF
{
    /// <summary>
    /// Exception class that notifies when the applications configuration is missing, or has data that is not valid.
    /// </summary>
    public class ConfigurationException:ManagedException
    {
        /// <summary>
        /// Creates an instance of <see cref="ConfigurationException"/> and returns the default exception message.
        /// </summary>
        public ConfigurationException() : base(StandardExceptionMessages.ConfigurationException)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="ConfigurationException"/> that returns the default exception message and an embedded exception.
        /// </summary>
        /// <param name="internalException">Existing exception to be added to this exception.</param>
        public ConfigurationException(Exception internalException) : base(StandardExceptionMessages.ConfigurationException, internalException)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="ConfigurationException"/> exception class.
        /// </summary>
        /// <param name="message">Message to be returned as part of the exception.</param>
        public ConfigurationException(string message) : base(message)
        {
            //Intentionally blank
        }

        /// <summary>
        /// Creates an instance of <see cref="ConfigurationException"/> exception class.
        /// </summary>
        /// <param name="message">Message to be returned as part of the exception.</param>
        /// <param name="internalException">Existing exception to be added to this exception.</param>
        public ConfigurationException(string message, Exception internalException) : base(message, internalException)
        {
            //Intentionally blank
        }
    }

}
