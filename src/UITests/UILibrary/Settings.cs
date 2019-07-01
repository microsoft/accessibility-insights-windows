// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class Settings
    {
        WindowsDriver<WindowsElement> Session;
        ApplicationTab ApplicationTab { get; }
        AboutTab AboutTab { get; }

        public Settings(WindowsDriver<WindowsElement> session)
        {
            Session = session;
            AboutTab = new AboutTab(session);
            ApplicationTab = new ApplicationTab(session);
        }
        public bool SaveAndClose() => false;
        public bool Back() => false;
    }

    public class ApplicationTab
    {
        WindowsDriver<WindowsElement> Session;
        public ApplicationTab(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
    }

    public class AboutTab
    {
        WindowsDriver<WindowsElement> Session;
        public AboutTab(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
    }
}
