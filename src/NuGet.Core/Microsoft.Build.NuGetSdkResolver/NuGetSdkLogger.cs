// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using NuGet.Common;
using INuGetLogger = NuGet.Common.ILogger;

namespace Microsoft.Build.NuGetSdkResolver
{
    /// <summary>
    /// An implementation of <see cref="T:NuGet.Common.ILogger" /> that logs messages to an <see cref="T:Microsoft.Build.Framework.SdkLogger" />.
    /// </summary>
    /// <inheritdoc />
    internal class NuGetSdkLogger : INuGetLogger
    {
        /// <summary>
        /// A collection of errors that have been logged.
        /// </summary>
        private readonly ICollection<string> _errors;

        /// <summary>
        /// A <see cref="SdkLogger"/> to forward events to.
        /// </summary>
        private readonly SdkLogger _sdkLogger;

        /// <summary>
        /// A collection of warnings that have been logged.
        /// </summary>
        private readonly ICollection<string> _warnings;

        /// <summary>
        /// Initializes a new instance of the NuGetLogger class.
        /// </summary>
        /// <param name="sdkLogger">A <see cref="SdkLogger"/> to forward events to.</param>
        /// <param name="warnings">A <see cref="ICollection{String}"/> to add logged warnings to.</param>
        /// <param name="errors">An <see cref="ICollection{String}"/> to add logged errors to.</param>
        public NuGetSdkLogger(SdkLogger sdkLogger, ICollection<string> warnings, ICollection<string> errors)
        {
            _sdkLogger = sdkLogger ?? throw new ArgumentNullException(nameof(sdkLogger));
            _warnings = warnings ?? throw new ArgumentNullException(nameof(warnings));
            _errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Verbose:
                    // Detailed and Diagnostic verbosity in MSBuild shows high, normal, and low importance messages
                    _sdkLogger.LogMessage(data, MessageImportance.Low);
                    break;

                case LogLevel.Information:
                    // Normal verbosity in MSBuild shows only high and normal importance messages
                    _sdkLogger.LogMessage(data, MessageImportance.Normal);
                    break;

                case LogLevel.Minimal:
                    // Minimal verbosity in MSBuild shows only high importance messages
                    _sdkLogger.LogMessage(data, MessageImportance.High);
                    break;

                case LogLevel.Warning:
                    _warnings.Add(data);
                    break;

                case LogLevel.Error:
                    _errors.Add(data);
                    break;
            }
        }

        public void Log(ILogMessage message) => Log(message.Level, message.Message);

        public Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);

            return Task.CompletedTask;
        }

        public Task LogAsync(ILogMessage message)
        {
            Log(message);

            return Task.CompletedTask;
        }

        public void LogDebug(string data) => Log(LogLevel.Debug, data);

        public void LogError(string data) => Log(LogLevel.Error, data);

        public void LogInformation(string data) => Log(LogLevel.Information, data);

        public void LogInformationSummary(string data) => Log(LogLevel.Information, data);

        public void LogMinimal(string data) => Log(LogLevel.Minimal, data);

        public void LogVerbose(string data) => Log(LogLevel.Verbose, data);

        public void LogWarning(string data) => Log(LogLevel.Warning, data);
    }
}
