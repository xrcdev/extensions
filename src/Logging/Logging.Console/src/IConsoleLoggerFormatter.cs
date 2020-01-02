namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Provides an abstraction over types that format log messages to be written to the console.
    /// </summary>
    public interface IConsoleLoggerFormatter
    {
        /// <summary>
        /// Formats the provided log event to the console.
        /// </summary>
        /// <param name="logEvent">A <see cref="ConsoleLoggerEvent"/> structure containing all the information about the log event.</param>
        /// <param name="console">A <see cref="IConsole"/> representing the console to write the message to.</param>
        void Format(ConsoleLoggerEvent logEvent, IConsole console);
    }
}
