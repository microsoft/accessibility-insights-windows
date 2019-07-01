// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class TestMode
    {
        WindowsDriver<WindowsElement> Session;
        public AutomatedChecks AutomatedChecks { get; }
        public UIATree UIATree { get; }
        public TabStops TabStops { get; }
        public NoElementSelected NoElementSelected { get; }

        public TestMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
            AutomatedChecks = new AutomatedChecks(session);
            UIATree = new UIATree(session);
            TabStops = new TabStops(session);
            NoElementSelected = new NoElementSelected(session);
        }
    }

    public class AutomatedChecks
    {
        WindowsDriver<WindowsElement> Session;
        public AutomatedChecks(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
        public bool ToggleAllExpanders() => false;
        public bool ViewInUIATree() => false;
    }

    public class UIATree
    {
        WindowsDriver<WindowsElement> Session;
        public UIATree(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
        public bool SelectResultsTab() => false;
        public bool SelectDetailsTab() => false;

        public bool BackToAutomatedChecks() => false;
    }

    public class TabStops
    {
        WindowsDriver<WindowsElement> Session;
        public TabStops(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
    }

    public class NoElementSelected
    {
        WindowsDriver<WindowsElement> Session;
        public NoElementSelected(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
    }
}
