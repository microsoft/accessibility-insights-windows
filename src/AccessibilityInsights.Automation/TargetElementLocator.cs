// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Desktop.UIAutomation;
using System;
using System.Globalization;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Wraps up the logic of selecting a target element basd on the command parameters
    /// </summary>
    internal static class TargetElementLocator
    {
        /// <summary>
        /// Locate the target element in the UIA tree
        /// </summary>
        /// <param name="parameters">The caller-provided parameters</param>
        /// <returns>The ElementContext that matches the targeting parameters</returns>
        internal static ElementContext LocateElement(CommandParameters parameters)
        {
            if (parameters.TryGetLong(CommandConstStrings.TargetProcessId, out long parameterPid))
            {
                try
                {
                    var element = A11yAutomation.ElementFromProcessId((int)parameterPid);

                    return new ElementContext(element);
                }
                catch (Exception ex)
                {
                    throw new A11yAutomationException(string.Format(CultureInfo.InvariantCulture, DisplayStrings.ErrorFailToGetRootElementOfProcess, parameterPid, ex), ex);
                }
            }

            throw new A11yAutomationException(DisplayStrings.ErrorNoProcessIdSet);
        }
    }
}
