// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Provides an abstraction over a console
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Writes the specified message, changing the background and/or foreground color from the default if specified.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="background">The background color to use. If <see langword="null"/>, the current default background color is used.</param>
        /// <param name="foreground">The foreground color to use. If <see langword="null"/>, the current default foreground color is used.</param>
        void Write(string message, ConsoleColor? background, ConsoleColor? foreground);

        /// <summary>
        /// Writes the specified message, followed by a line-break, changing the background and/or foreground color from the default if specified.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="background">The background color to use. If <see langword="null"/>, the current default background color is used.</param>
        /// <param name="foreground">The foreground color to use. If <see langword="null"/>, the current default foreground color is used.</param>
        void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground);

        /// <summary>
        /// Flushes all writes to the console.
        /// </summary>
        void Flush();
    }
}
