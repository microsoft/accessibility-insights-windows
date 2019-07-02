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

        const string OpenFileToolBarAutomationID = "1001";
        const string OpenFileFolderTextBoxAutomationID = "41477";
        const string OpenFileFileTextBoxAutomationID = "1148";

        public LiveMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }

        public bool OpenFile(string folder, string fileName)
        {
            if (!Session.FindAndClickByAutomationID(AutomationIDs.MainWinLoadButton)) return false;

            if (!Session.FindAndClickByAutomationID(OpenFileToolBarAutomationID)) return false;

            try
            {
                var folderTextbox = Session.FindElementByAccessibilityId(OpenFileFolderTextBoxAutomationID);
                folderTextbox.SendKeys(folder);
                folderTextbox.SendKeys(OpenQA.Selenium.Keys.Enter);

                var fileTextbox = Session.FindElementByAccessibilityId(OpenFileFileTextBoxAutomationID);
                fileTextbox.SendKeys(fileName);
                fileTextbox.SendKeys(OpenQA.Selenium.Keys.Enter);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
