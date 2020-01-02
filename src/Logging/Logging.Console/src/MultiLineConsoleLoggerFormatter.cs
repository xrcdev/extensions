using System;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    internal class MultiLineConsoleLoggerFormatter : IConsoleLoggerFormatter
    {
        // ConsoleColor does not have a value to specify the 'Default' color
        private readonly ConsoleColor? DefaultConsoleColor = null;
        private static readonly string _loglevelPadding = ": ";
        private static readonly string _messagePadding;
        private static readonly string _newLineWithMessagePadding;

        static MultiLineConsoleLoggerFormatter()
        {
            var logLevelString = GetLogLevelString(LogLevel.Information);
            _messagePadding = new string(' ', logLevelString.Length + _loglevelPadding.Length);
            _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        }


        public MultiLineConsoleLoggerFormatter(string timestampFormat, bool useUtcTimestamp, bool disableColors)
        {
            TimestampFormat = timestampFormat;
            UseUtcTimestamp = useUtcTimestamp;
            DisableColors = disableColors;
        }

        public string TimestampFormat { get; }
        public bool UseUtcTimestamp { get; }
        public bool DisableColors { get; }

        public void Format(ConsoleLoggerEvent logEvent, IConsole console)
        {
            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received

            // TEMP: POOOOOOLING
            var logBuilder = new StringBuilder();

            // It seems wrong that we don't put a space after the timestamp, but that's the current behavior.
            // We could fix it but that would break people who put a space in their TimestampFormat!
            var timestampFormat = TimestampFormat;
            if (timestampFormat != null)
            {
                var dateTime = UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
                console.Write(dateTime.ToString(timestampFormat), null, null);
            }

            var logLevelColors = GetLogLevelConsoleColors(logEvent.Level);
            var logLevelString = GetLogLevelString(logEvent.Level);
            console.Write(logLevelString, logLevelColors.Background, logLevelColors.Foreground);

            // category and event id
            logBuilder.Append(_loglevelPadding);
            logBuilder.Append(logEvent.Category);
            logBuilder.Append("[");
            logBuilder.Append(logEvent.Id.Id);
            logBuilder.Append("]");
            console.WriteLine(logBuilder.ToString(), null, null);
            logBuilder.Length = 0;

            // scope information
            foreach (var scope in logEvent.Scopes)
            {
                if (logBuilder.Length == 0)
                {
                    logBuilder.Append(_messagePadding);
                    logBuilder.Append("=> ");
                }
                else
                {
                    logBuilder.Append(" => ");
                }
                logBuilder.Append(scope);
            }

            if(logBuilder.Length > 0)
            {
                console.WriteLine(logBuilder.ToString(), null, null);
                logBuilder.Length = 0;
            }

            if (!string.IsNullOrEmpty(logEvent.FormattedMessage))
            {
                // message
                logBuilder.Append(_messagePadding);

                var len = logBuilder.Length;
                logBuilder.Append(logEvent.FormattedMessage);
                logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, logEvent.FormattedMessage.Length);
                console.WriteLine(logBuilder.ToString(), null, null);
                logBuilder.Length = 0;
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (logEvent.Exception != null)
            {
                // exception message
                console.WriteLine(logEvent.Exception.ToString(), null, null);
            }
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
        {
            if (DisableColors)
            {
                return new ConsoleColors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return new ConsoleColors(ConsoleColor.White, ConsoleColor.Red);
                case LogLevel.Error:
                    return new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red);
                case LogLevel.Warning:
                    return new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black);
                case LogLevel.Information:
                    return new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black);
                case LogLevel.Debug:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                case LogLevel.Trace:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                default:
                    return new ConsoleColors(DefaultConsoleColor, DefaultConsoleColor);
            }
        }

        private readonly struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }
    }
}
