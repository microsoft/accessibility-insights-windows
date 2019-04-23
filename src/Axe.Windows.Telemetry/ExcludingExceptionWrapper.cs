// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Axe.Windows.Telemetry
{
    /// <summary>
    /// Methods to allow code to be executed as a lambda, but identify specific
    /// types of Exceptions that should be excluded from the telemetry pipeline.
    /// Exclusion is handled by converting the specific Exception type to an
    /// ExcludedException, which telemetry will then ignore. If you have catch
    /// handlers that distinguish on type, be sure to allow 
    /// </summary>
    public static class ExcludingExceptionWrapper
    {
        /// <summary>
        /// Execute a block of code within a try/catch block. If it throws the specified
        /// Exception type, catch it, wrap it in a TelemetryExcludedException, and re-throw
        /// the wrapped Exception. This allows the existing Exception handling to be used,
        /// but also allows us to exclude that specific Exception from telemetry.
        /// </summary>
        /// <param name="typeToExclude">The specific Exception type that we want to exclude from telemetry</param>
        /// <param name="action">The code that might generate an Exception</param>
        /// <exception cref="ExcludedException">If the targeted exception type occurs, otherwise whatever the Action does</exception>
        public static void ExecuteWithExcludedExceptionConversion(Type typeToExclude, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (e.GetType() != typeToExclude)
                {
                    throw;
                }

                throw new ExcludedException(e);
            }
        }
    }
}
