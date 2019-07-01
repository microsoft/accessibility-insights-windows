// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Automation;
using OpenQA.Selenium.Appium.Windows;

namespace UITests.UILibrary
{
    public class AIWinDriver
    {
        WindowsDriver<WindowsElement> Session;
        public GettingStarted GettingStarted { get; }
        public EventsMode EventsMode { get; }
        public Settings Settings { get; }
        public LiveMode LiveMode { get; }
        public TestMode TestMode { get; }

        int PID;

        public AIWinDriver(WindowsDriver<WindowsElement> session, int pid)
        {
            EventsMode = new EventsMode(session);
            Settings = new Settings(session);
            LiveMode = new LiveMode(session);
            TestMode = new TestMode(session);
            GettingStarted = new GettingStarted(session);
            Session = session;
            PID = pid;
        }

        public (int errors, string file) ScanAIWin()
        {

            var config = Config.Builder.ForProcessId(PID).Build();
            var scanner = ScannerFactory.CreateScanner(config);

            var result = scanner.Scan();

            return (result.ErrorCount, result.OutputFile.A11yTest);
        }

        public WindowsElement FindElementByAccessibilityId(string accessibilityId) => Session.FindElementByAccessibilityId(accessibilityId);

        public string Title => Session.Title;
    }
}
