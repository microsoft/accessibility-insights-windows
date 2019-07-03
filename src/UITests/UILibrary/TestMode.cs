// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class TestMode
    {
        WindowsDriver<WindowsElement> Session;
        public AutomatedChecks AutomatedChecks { get; }
        public ResultsInUIATree ResultsInUIATree { get; }
        public TestMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
            AutomatedChecks = new AutomatedChecks(session);
            ResultsInUIATree = new ResultsInUIATree(session);
        }
    }

    public class AutomatedChecks
    {
        WindowsDriver<WindowsElement> Session;
        public AutomatedChecks(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
        public void ViewInUIATree() => Session.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksUIATreeButton).Click();
    }

    public class ResultsInUIATree
    {
        WindowsDriver<WindowsElement> Session;
        public ResultsInUIATree(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
        public void BackToAutomatedChecks() => Session.FindElementByAccessibilityId(AutomationIDs.MainWinBreadCrumbTwoButton).Click();
    }
}
