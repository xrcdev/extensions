// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Extensions.Logging.Console
{
    internal class ConsoleLoggerProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private readonly BlockingCollection<(ConsoleLoggerEvent, IConsoleLoggerFormatter)> _messageQueue = new BlockingCollection<(ConsoleLoggerEvent, IConsoleLoggerFormatter)>(_maxQueuedMessages);
        private readonly Thread _outputThread;

        public IConsole Console;
        public IConsole ErrorConsole;

        public ConsoleLoggerProcessor()
        {
            // Start Console message queue processor
            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "Console logger queue processing thread"
            };
            _outputThread.Start();
        }

        public virtual void EnqueueMessage(ConsoleLoggerEvent message, IConsoleLoggerFormatter formatter)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add((message, formatter));
                    return;
                }
                catch (InvalidOperationException) { }
            }

            // Adding is completed so just log the message
            try
            {
                WriteMessage(message, formatter);
            }
            catch (Exception) { }
        }

        // for testing
        internal virtual void WriteMessage(ConsoleLoggerEvent message, IConsoleLoggerFormatter formatter)
        {
            var console = message.WriteToStandardError ? ErrorConsole : Console;

            formatter.Format(message, console);

            console.Flush();
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (var (message, formatter) in _messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(message, formatter);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _outputThread.Join(1500); // with timeout in-case Console is locked by user input
            }
            catch (ThreadStateException) { }
        }
    }
}
