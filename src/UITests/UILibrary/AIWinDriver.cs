// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using System.IO;
using System.Linq;
using static System.FormattableString;

namespace UITests.UILibrary
{
    public class AIWinDriver
    {
        readonly WindowsDriver<WindowsElement> Session;
        public GettingStarted GettingStarted { get; }
        public EventsMode EventsMode { get; }
        public Settings Settings { get; }
        public LiveMode LiveMode { get; }
        public TestMode TestMode { get; }

        readonly int PID;

        public AIWinDriver(WindowsDriver<WindowsElement> session, int pid)
        {
            EventsMode = new EventsMode();
            Settings = new Settings();
            LiveMode = new LiveMode(session);
            TestMode = new TestMode(session);
            GettingStarted = new GettingStarted(session);
            Session = session;
            PID = pid;
        }

        /// <summary>
        /// Run an accessibility scan on Accessibility Insights for Windows
        /// and add an a11ytest file to the given context's test results
        /// if there are any errors
        /// </summary>
        /// <param name="context"></param>
        /// <returns>number of accessibility issues</returns>
        private int ScanAIWin(TestContext context, string fileName)
        {
            var outputPath = Path.Combine(context.TestResultsDirectory, context.TestName);
            var config = Config.Builder.ForProcessId(PID)
                .WithOutputFileFormat(OutputFileFormat.A11yTest)
                .WithOutputDirectory(outputPath)
                .Build();

            var scanner = ScannerFactory.CreateScanner(config);

            var result = scanner.Scan(null).WindowScanOutputs.First();
            if (result.ErrorCount > 0)
            {
                var newPath = Path.Combine(outputPath, Invariant($"{fileName}.a11ytest"));
                File.Move(result.OutputFile.A11yTest, newPath);
                context.AddResultFile(newPath);
            }

            return result.ErrorCount;
        }

        public void VerifyAccessibility(TestContext context, string fileName, int expectedIssueCount)
        {
            var issueCount = this.ScanAIWin(context, fileName);
            Assert.AreEqual(expectedIssueCount, issueCount, $"axe.windows found accessibility issues, check {fileName}.a11ytest file in test artifacts");
        }

        public WindowsElement FindElementByAccessibilityId(string accessibilityId) => Session.FindElementByAccessibilityId(accessibilityId);

        public string Title => Session.Title;

        public void ToggleHighlighter() => Session.FindElementByAccessibilityId(AutomationIDs.MainWinHighlightButton).Click();

        public void GoToSettings() => Session.FindElementByAccessibilityId(AutomationIDs.MainWinSettingsButton).Click();

        public void Maximize() => Session.Manage().Window.Maximize();

        public void FocusWindowByResizing() => Session.Manage().Window.Size = Session.Manage().Window.Size;
    }
}
