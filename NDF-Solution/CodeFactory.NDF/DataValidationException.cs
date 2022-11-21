//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************

using System;

namespace CodeFactory.NDF
{
    /// <summary>
    /// Notifies when a property fails validation and captures the property name.
    /// </summary>
    public class DataValidationException : DataException
    {
        /// <summary>
        /// Backing field for the property <see cref="PropertyName"/>
        /// </summary>
        private readonly string _propertyName;

        /// <summary>
        /// Creates an instance of <see cref="DataValidationException"/> and returns the default exception message.
        /// </summary>
        /// <param name="propertyName">The name of the property the data validation occurred at.</param>
        public DataValidationException(string propertyName) : base(string.Format(StandardExceptionMessages.DataValidationException, propertyName))
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Creates an instance of <see cref="DataValidationException"/> and returns the default exception message.
        /// </summary>
        /// <param name="propertyName">The name of the property the data validation occurred at.</param>
        /// <param name="internalException">Additional exception information to include.</param>
        public DataValidationException(string propertyName,Exception internalException) : base(string.Format(StandardExceptionMessages.DataValidationException, propertyName),internalException)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Creates an instance of <see cref="DataValidationException"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property the data validation occurred at.</param>
        /// <param name="message">Custom exception message to include with the exception.</param>
        public DataValidationException(string propertyName, string message) : base(message)
        {
            _propertyName = propertyName;
        }

        /// <summary>
        /// Creates an instance of <see cref="DataValidationException"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property the data validation occurred at.</param>
        /// <param name="message">Custom exception message to include with the exception.</param>
        /// <param name="internalException">Additional exception information to include.</param>
        public DataValidationException(string propertyName, string message,Exception internalException) : base(message,internalException)
        {
            _propertyName = propertyName;
        }
        
        /// <summary>
        /// Name of the property that has an invalid value.
        /// </summary>
        public string PropertyName => _propertyName;
    }
}
