// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class LiveMode
    {
        WindowsDriver<WindowsElement> Session;

        // These AutomationIDs came from inspecting the open file dialog with ai-win.
        // The assumption is that they are the same accross machines--if they aren't,
        // we'll need a more robust way to navigate the dialog.
        const string OpenFileToolBarAutomationID = "1001";
        const string OpenFileFolderTextBoxAutomationID = "41477";
        const string OpenFileFileTextBoxAutomationID = "1148";
        const string OpenFileSearchBoxAutomationID = "SearchEditBox";

        public string SelectedElementText => Session.FindElementByAccessibilityId(AutomationIDs.InspectTabsElementTextBlock).Text;

        public LiveMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }

        public void OpenFile(string folder, string fileName)
        {
            Session.FindElementByAccessibilityId(AutomationIDs.MainWinLoadButton).Click();

            // this puts focus on the toolbar, which we need for the next command to work
            Session.FindElementByAccessibilityId(OpenFileSearchBoxAutomationID).SendKeys(Keys.Shift + Keys.Tab);
            // this turns the toolbar into a text box
            Session.FindElementByAccessibilityId(OpenFileToolBarAutomationID).SendKeys(Keys.Enter);

            var folderTextbox = Session.FindElementByAccessibilityId(OpenFileFolderTextBoxAutomationID);
            folderTextbox.SendKeys(folder);
            folderTextbox.SendKeys(Keys.Enter);

            var fileTextbox = Session.FindElementByAccessibilityId(OpenFileFileTextBoxAutomationID);
            fileTextbox.SendKeys(fileName);
            fileTextbox.SendKeys(Keys.Enter);
        }

        public void TogglePause() => Session.FindElementByAccessibilityId(AutomationIDs.MainWindow).SendKeys(Keys.Shift + Keys.F5);

        public void RunTests() => Session.FindElementByAccessibilityId(AutomationIDs.HierarchyControlTestElementButton).Click();
    }
}
