// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    internal class ConfigureFormatter : IPostConfigureOptions<ConsoleLoggerOptions>
    {
        public void PostConfigure(string name, ConsoleLoggerOptions options)
        {
            if(options.Formatter == null)
            {
                // Configure the formatter
                options.Formatter = options.Format switch
                {
                    ConsoleLoggerFormat.Default => new MultiLineConsoleLoggerFormatter(options.TimestampFormat, options.UseUtcTimestamp, options.DisableColors),
                    ConsoleLoggerFormat.Systemd => new SystemdConsoleLoggerFormatter(options.TimestampFormat, options.UseUtcTimestamp),
                    _ => throw new ArgumentException($"Unknown console logger format: {options.Format}."),
                };
            }
        }
    }
}
