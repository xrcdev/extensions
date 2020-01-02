using System;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    internal class SystemdConsoleLoggerFormatter : IConsoleLoggerFormatter
    {
        public SystemdConsoleLoggerFormatter(string timestampFormat, bool useUtcTimestamp)
        {
            TimestampFormat = timestampFormat;
            UseUtcTimestamp = useUtcTimestamp;
        }

        public string TimestampFormat { get; }
        public bool UseUtcTimestamp { get; }

        public void Format(ConsoleLoggerEvent logEvent, IConsole console)
        {
            // systemd reads messages from standard out line-by-line in a '<pri>message' format.
            // newline characters are treated as message delimiters, so we must replace them.
            // Messages longer than the journal LineMax setting (default: 48KB) are cropped.
            // Example:
            // <6>ConsoleApp.Program[10] Request received

            // TEMP: POOOOOOLING
            var logBuilder = new StringBuilder();

            // loglevel
            var logLevelString = GetSyslogSeverityString(logEvent.Level);
            logBuilder.Append(logLevelString);

            // timestamp
            var timestampFormat = TimestampFormat;
            if (timestampFormat != null)
            {
                var dateTime = UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
                logBuilder.Append(dateTime.ToString(timestampFormat));
            }

            // category and event id
            logBuilder.Append(logEvent.Category);
            logBuilder.Append("[");
            logBuilder.Append(logEvent.Id.Id);
            logBuilder.Append("]");

            // scope information
            foreach (var scope in logEvent.Scopes)
            {
                logBuilder.Append(" => ");
                logBuilder.Append(scope);
            }

            // message
            if (!string.IsNullOrEmpty(logEvent.FormattedMessage))
            {
                logBuilder.Append(' ');
                // message
                AppendAndReplaceNewLine(logBuilder, logEvent.FormattedMessage);
            }

            // exception
            // System.InvalidOperationException at Namespace.Class.Function() in File:line X
            if (logEvent.Exception != null)
            {
                logBuilder.Append(' ');
                AppendAndReplaceNewLine(logBuilder, logEvent.Exception.ToString());
            }

            // newline delimiter
            logBuilder.Append(Environment.NewLine);

            console.WriteLine(logBuilder.ToString(), null, null);

            static void AppendAndReplaceNewLine(StringBuilder sb, string message)
            {
                var len = sb.Length;
                sb.Append(message);
                sb.Replace(Environment.NewLine, " ", len, message.Length);
            }
        }

        private static string GetSyslogSeverityString(LogLevel logLevel)
        {
            // 'Syslog Message Severities' from https://tools.ietf.org/html/rfc5424.
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return "<7>"; // debug-level messages
                case LogLevel.Information:
                    return "<6>"; // informational messages
                case LogLevel.Warning:
                    return "<4>"; // warning conditions
                case LogLevel.Error:
                    return "<3>"; // error conditions
                case LogLevel.Critical:
                    return "<2>"; // critical conditions
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}
