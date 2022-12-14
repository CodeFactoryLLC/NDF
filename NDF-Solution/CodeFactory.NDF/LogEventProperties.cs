//*****************************************************************************
//* Net Delivery Framework
//* Copyright (c) 2022 CodeFactory, LLC
//*****************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CodeFactory.NDF
{
    /// <summary>
    /// Data class that holds properties that are passed into the <see cref="ILogger"/> implementation to be included in the logging output.
    /// </summary>
    public class LogEventProperties : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Field that holds the additional properties set in the log event.
        /// </summary>
        private readonly List<KeyValuePair<string, object>> _properties;

        /// <summary>
        /// Holds the message for the log event.
        /// </summary>
        private readonly string _message;

        /// <summary>
        /// Log event message 
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Creates a new instance of the <see cref="LogEventProperties"/>.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        public LogEventProperties(string message)
        {
            _message = message;
            _properties = new List<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// Gets the enumerators of the properties for this logging event.
        /// </summary>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }


        /// <summary>
        /// Gets the enumerators of the properties for this logging event.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        /// <summary>
        /// Adds a property to the log event.
        /// </summary>
        /// <param name="name">Name of the property for the log event.</param>
        /// <param name="value">Value of the log event.</param>
        public void AddProp(string name, object value)
        {
            _properties.Add(new KeyValuePair<string, object>(name, value));
        }

        /// <summary>
        /// Function for formatting log events.
        /// </summary>
        public static Func<LogEventProperties, Exception?, string> Formatter { get; } = (l, e) => l.Message;
    }
}
