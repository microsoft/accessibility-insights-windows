// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
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

            LaunchApplicationAndAttach();

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
            log.EntryWritten += new EntryWrittenEventHandler((_, args) => Events.Add(args.Entry));
            log.EnableRaisingEvents = true;
        }

        private void LaunchApplicationAndAttach()
        {
            // AccessibilityInsights is referenced by this test project
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var exePath = Path.Combine(executingDirectory, "AccessibilityInsights.exe");
            var configPathArgument = $"--ConfigFolder \"{Path.Combine(TestContext.TestResultsDirectory, TestContext.TestName)}\"";

            _process = Process.Start(exePath, configPathArgument);

            const int attempts = 10; // this number should give us enough retries for the build to work
            StartNewSessionWithRetry(attempts);

            driver = new AIWinDriver(_session, _process.Id);
        }

        /// <summary>
        /// We can't start a WinAppDriver session until ai-win is past its splash screen and fully loaded.
        /// This takes a variable amount of time depending on the executing machine. It is particularly slow
        /// in our build pipeline. Rather than set a long delay for the worst case scenario, we instead attempt
        /// to start a new session repeatedly until ai-win is ready.
        /// </summary>
        /// <param name="attempts">Number of times to retry starting a new session</param>
        private void StartNewSessionWithRetry(int attempts)
        {
            // if the session and its title are present, ai-win is ready for testing.
            while (attempts > 0 && string.IsNullOrEmpty(_session?.Title))
            {
                attempts--;

                StartNewSession();
                Thread.Sleep(3000);
            }
        }

        private void StartNewSession()
        {
            try
            {
                _process.Refresh(); // updates process.MainWindowHandle
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("appTopLevelWindow", _process.MainWindowHandle.ToString("x"));
                _session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
            }
            catch { }
        }

        public void TearDown()
        {
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
                foreach (var e in Events)
                {
                    w.WriteLine($"{e.TimeGenerated},{e.Source},{e.Message}");
                }
            }
            return logPath;
        }
    }
}