using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Represents a log event to be written to the console.
    /// </summary>
    public struct ConsoleLoggerEvent
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConsoleLoggerEvent"/>
        /// </summary>
        /// <param name="level">A <see cref="LogLevel"/> indicating the level of the log event.</param>
        /// <param name="category">The name of the logger category that produced the event.</param>
        /// <param name="id">An <see cref="EventId"/> representing the unique ID of the event.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the event, or <see langword="null"/> if there is no <see cref="Exception"/>.</param>
        /// <param name="state">The state object associated with the event.</param>
        /// <param name="formattedMessage">The formatted message associated with the event.</param>
        /// <param name="scopes">The scope objects associated with the event, if any.</param>
        /// <param name="writeToStandardError">A boolean indicating if the event should be written to standard error.</param>
        public ConsoleLoggerEvent(LogLevel level, string category, EventId id, Exception exception, object state, string formattedMessage, IEnumerable<object> scopes, bool writeToStandardError)
        {
            Level = level;
            Category = category;
            Id = id;
            Exception = exception;
            State = state;
            FormattedMessage = formattedMessage;
            Scopes = scopes;
            WriteToStandardError = writeToStandardError;
        }

        /// <summary>
        /// Gets a boolean indicating if the event should be written to standard error.
        /// </summary>
        public bool WriteToStandardError { get; }

        /// <summary>
        /// Gets a <see cref="LogLevel"/> indicating the level of the log event.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Gets the name of the logger category that produced the event.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets an <see cref="EventId"/> representing the unique ID of the event.
        /// </summary>
        public EventId Id { get; }

        /// <summary>
        /// Gets the <see cref="Exception"/> associated with the event, or <see langword="null"/> if there is no <see cref="Exception"/>.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the state object associated with the event.
        /// </summary>
        public object State { get; }

        /// <summary>
        /// Gets the formatted message associated with the event.
        /// </summary>
        public string FormattedMessage { get; }

        /// <summary>
        /// Gets or sets the scope objects associated with the event, if any.
        /// </summary>
        public IEnumerable<object> Scopes { get; }
    }
}
