// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ConsoleLoggerProcessor _queueProcessor;

        internal ConsoleLogger(string name, ConsoleLoggerProcessor loggerProcessor)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _name = name;
            _queueProcessor = loggerProcessor;
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        internal ConsoleLoggerOptions Options { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                var scopes = Options.IncludeScopes ? GetScopes() : Array.Empty<object>();
                var evt = new ConsoleLoggerEvent(logLevel, _name, eventId, exception, state, message, scopes, writeToStandardError: logLevel >= Options.LogToStandardErrorThreshold);
                _queueProcessor.EnqueueMessage(evt, Options.Formatter);
            }
        }

        private IEnumerable<object> GetScopes()
        {
            // PERF: Yikes...
            var scopes = new List<object>();
            ScopeProvider.ForEachScope((scope, state) => state.Add(scope), scopes);
            return scopes;
        }


        private DateTime GetCurrentDateTime()
        {
            return Options.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}
