// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using OpenQA.Selenium.Appium.Windows;
using UITests.Utilities;

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

        public LiveMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }

        public void OpenFile(string folder, string fileName)
        {
            Session.FindAndClickByAutomationID(AutomationIDs.MainWinLoadButton);
            Session.FindAndClickByAutomationID(OpenFileToolBarAutomationID);

            var folderTextbox = Session.FindElementByAccessibilityId(OpenFileFolderTextBoxAutomationID);
            folderTextbox.SendKeys(folder);
            folderTextbox.SendKeys(OpenQA.Selenium.Keys.Enter);

            var fileTextbox = Session.FindElementByAccessibilityId(OpenFileFileTextBoxAutomationID);
            fileTextbox.SendKeys(fileName);
            fileTextbox.SendKeys(OpenQA.Selenium.Keys.Enter);
        }
    }
}
