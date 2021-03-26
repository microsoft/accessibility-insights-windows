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
        const string OpenFileFolderTextBoxAutomationID = "41477";
        const string OpenFileFileTextBoxAutomationID = "1148";
        const string OpenFileAllLocationsElementName = "All locations";

        public string SelectedElementText => Session.FindElementByAccessibilityId(AutomationIDs.InspectTabsElementTextBlock).Text;

        public LiveMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }

        public void OpenFile(string folder, string fileName)
        {
            Session.FindElementByAccessibilityId(AutomationIDs.MainWinLoadButton).Click();
            Session.FindElementByName(OpenFileAllLocationsElementName).Click();

            var folderTextbox = Session.FindElementByAccessibilityId(OpenFileFolderTextBoxAutomationID);
            folderTextbox.SendKeys(folder + Keys.Enter);

            var fileTextbox = Session.FindElementByAccessibilityId(OpenFileFileTextBoxAutomationID);
            fileTextbox.SendKeys(fileName + Keys.Enter);
        }

        public void TogglePause() => Session.FindElementByAccessibilityId(AutomationIDs.MainWindow).SendKeys(Keys.Shift + Keys.F5);

        public void RunTests() => Session.FindElementByAccessibilityId(AutomationIDs.HierarchyControlTestElementButton).Click();
    }
}
