// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using UITests.UILibrary;

namespace UITests
{
    [TestClass]
    public class AIWinSession
    {
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private Process _process;
        private WindowsDriver<WindowsElement> _session;
        protected AIWinDriver driver;
        public TestContext TestContext { get; set; }

        protected IList<EventLogEntry> Events { get; } = new List<EventLogEntry>();

        public void Setup()
        {
            if (!IsWinAppDriverRunning())
            {
                Assert.Inconclusive("WinAppDriver.exe is not running");
            }

            SetupEventListening();

            bool attached = LaunchApplicationAndAttach();

            Assert.IsTrue(attached);
            Assert.IsNotNull(_session);
            Assert.IsNotNull(_session.SessionId);

            // Set implicit timeout to 1.5 seconds to ensure element search retries every 500 ms for at most three times
            _session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
        }

        /// <summary>
        /// Save events written to Windows Event Log during this test
        /// </summary>
        private void SetupEventListening()
        {
            var log = new EventLog("Application");
            log.EntryWritten += AddEvent;
            log.EnableRaisingEvents = true;
        }

        private void ClearEventListening()
        {
            var log = new EventLog("Application");
            log.EntryWritten -= AddEvent;
        }

        private void AddEvent(object sender, EntryWrittenEventArgs args) => Events.Add(args.Entry);

        private bool LaunchApplicationAndAttach()
        {
            // AccessibilityInsights is referenced by this test project
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var exePath = Path.Combine(executingDirectory, "AccessibilityInsights.exe");
            var configPathArgument = $"--ConfigFolder \"{Path.Combine(TestContext.TestResultsDirectory, TestContext.TestName)}\"";

            _process = Process.Start(exePath, configPathArgument);

            const int attempts = 10; // this number should give us enough retries for the build to work
            bool attached = WaitFor(() => !string.IsNullOrEmpty(_session?.Title), new TimeSpan(0, 0, 3), attempts, StartNewSession);
            driver = new AIWinDriver(_session, _process.Id);

            return attached;
        }

        private void StartNewSession()
        {
            try
            {
                _process.Refresh(); // updates process.MainWindowHandle
                var options = new AppiumOptions();
                options.AddAdditionalCapability("deviceName", "WindowsPC");
                options.AddAdditionalCapability("appTopLevelWindow", _process.MainWindowHandle.ToString("x"));
                _session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), options);
            }
            catch { }
        }

        public void TearDown()
        {
            ClearEventListening();
            AttachEvents();

            // closing ai-win like this stops it from saving the config. Will have to change this
            // if we ever want to use the saved config.
            _process?.Kill();

            _session?.Quit();
        }

        private bool IsWinAppDriverRunning()
        {
            var processes = Process.GetProcessesByName("WinAppDriver");
            return processes.Length > 0;
        }

        /// <summary>
        /// Serialize recorded events and attach them to the test context
        /// </summary>
        private void AttachEvents()
        {
            var eventLog = SerializeRecordedEvents();
            if (eventLog != null)
            {
                TestContext.AddResultFile(eventLog);
            }
            Events.Clear();
        }

        private string SerializeRecordedEvents()
        {
            var testDir = Path.Combine(TestContext.TestResultsDirectory, TestContext.TestName);
            if (!Directory.Exists(testDir))
            {
                return null;
            }
            var logPath = Path.Combine(testDir, "events.txt");
            using (StreamWriter w = File.AppendText(logPath))
            {
                // We sometimes see a race condition here, so use an index instead of an iterator
                for (int index = 0; index < Events.Count; index++)
                {
                    var e = Events[index];
                    w.WriteLine($"{e.TimeGenerated},{e.Source},{e.Message}");
                }
            }
            return logPath;
        }

        protected bool WaitFor(Func<bool> checkSuccess, TimeSpan interval, int attempts, Action doUntilSuccess = null)
        {
            while (attempts > 0 && !checkSuccess())
            {
                attempts--;
                doUntilSuccess?.Invoke();
                Thread.Sleep(interval);
            }

            return attempts > 0;
        }
    }
}