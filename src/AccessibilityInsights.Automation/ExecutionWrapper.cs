// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;

namespace AccessibilityInsights.Automation
{
    internal static class ExecutionWrapper
    {
        private static readonly object lockObject = new object();

        /// <summary>
        /// Synchronously (and blocking) execute the passed-in command, handling errors appropriately
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="errorFactory">The factory to create the appropriate error object</param>
        /// <returns>An object of Type T that describes the command result</returns>
        internal static T ExecuteCommand<T>(Func<T> command, Func<string, T> errorFactory)
        {
            lock (lockObject)
            {
                try
                {
                    return command();
                }
                catch (Exception ex)
                {
                    string errorDetail;

                    A11yAutomationException automationException = ex as A11yAutomationException;

                    if (automationException == null)
                    {
                        errorDetail = string.Format(CultureInfo.InvariantCulture, DisplayStrings.ErrorUnhandledExceptionFormat, ex.ToString());
                    }
                    else
                    {
                        errorDetail = automationException.Message;
                    }

                    return errorFactory(errorDetail);
                }
            }
        }
    }
}
