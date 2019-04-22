// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Axe.Windows.Telemetry
{
    /// <summary>
    /// Code to allow code to run but ensure that and Exceptions will be excluded
    /// from the telemetry pipeline
    /// </summary>
    public static class ExceptionExcluder
    {
        /// <summary>
        /// Execute a block of code within a try/catch block. If it throws, catch the Exception
        /// and wrap it in a TelemetryExcludedException, which should be excluded from any
        /// upstream telemetry pipelines.
        /// </summary>
        /// <param name="action">The code that might generate an Exception</param>
        public static void ExcludeThrownExceptions(Action action)
        {
            try
            {
                action();
            }
#pragma warning disable CA1031
            catch (Exception e)
#pragma warning restore CA1031
            {
                throw new ExcludedException("Excluded Exception", e);
            }
        }

    }
}
