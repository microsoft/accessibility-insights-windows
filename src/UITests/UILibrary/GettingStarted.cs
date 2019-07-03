// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using OpenQA.Selenium.Appium.Windows;
using UITests.Utilities;

namespace UITests.UILibrary
{
    public class GettingStarted
    {
        WindowsDriver<WindowsElement> Session;

        public GettingStarted(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }

        public void DismissTelemetry() => Session.FindAndClickByAutomationID(AutomationIDs.TelemetryDialogExitButton);
        public void DismissStartupPage() => Session.FindAndClickByAutomationID(AutomationIDs.StartUpModeExitButton);
    }
}
