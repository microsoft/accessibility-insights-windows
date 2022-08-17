// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class GettingStarted
    {
        readonly WindowsDriver<WindowsElement> Session;

        public GettingStarted(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }

        public void DismissTelemetry() => Session.FindElementByAccessibilityId(AutomationIDs.TelemetryDialogExitButton).Click();
        public void DismissStartupPage() => Session.FindElementByAccessibilityId(AutomationIDs.StartUpModeExitButton).Click();
    }
}
