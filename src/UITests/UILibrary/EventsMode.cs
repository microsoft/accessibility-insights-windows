// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class EventsMode
    {
        WindowsDriver<WindowsElement> Session;
        public EventsMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
    }
}
