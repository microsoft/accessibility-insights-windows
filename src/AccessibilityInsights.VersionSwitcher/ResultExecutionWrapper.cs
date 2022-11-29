// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using System;

namespace AccessibilityInsights.VersionSwitcher
{
    internal static class ResultExecutionWrapper
    {
        /// <summary>
        /// Execute the specified function in a try/catch wrapper. Catch all Exceptions.
        /// If it's already a <see cref="ResultBearingException"/>, then just re-throw it. If it's
        /// anything else, wrap it in a <see cref="ResultBearingException"/> with the provided message
        /// and result.
        /// </summary>
        /// <typeparam name="T">The type returned by the function</typeparam>
        /// <param name="resultOnException">The <see cref="ExecutionResult"/> to specify if we throw a ResultBearingException</param>
        /// <param name="messageBuilder">The message builder if we throw a ResultBearingException</param>
        /// <param name="function">The code to execute within the try/catch block</param>
        /// <returns>The value returned by the function</returns>
        /// <exception cref="ResultBearingException"></exception>
        internal static T Execute<T>(ExecutionResult resultOnException, Func<string> messageBuilder, Func<T> function)
        {
            try
            {
                return function();
            }
            catch (ResultBearingException)
            {
                throw;
            }
            catch(Exception e)
            {
                throw new ResultBearingException(resultOnException, messageBuilder(), e);
            }
        }
    }
}
