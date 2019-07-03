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
        private Process process;
        private WindowsDriver<WindowsElement> session;
        protected AIWinDriver driver;
        public TestContext TestContext { get; set; }

        protected IList<EventLogEntry> Events { get; } = new List<EventLogEntry>();

        public void Setup()
        {
            if (session != null)
            {
                return;
            }

            if (!IsWinAppDriverRunning())
            {
                Assert.Inconclusive("WinAppDriver.exe is not running");
            }

            SetupEventListening();

            LaunchApplicationAndAttach();

            Assert.IsNotNull(session);
            Assert.IsNotNull(session.SessionId);

            // Set implicit timeout to 1.5 seconds to ensure element search retries every 500 ms for at most three times
            session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
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

            process = Process.Start(exePath, configPathArgument);
            process.WaitForInputIdle();

            // small buffer between splash screen disappearing 
            // and main window initializing; otherwise in rare
            // cases splash screen can be picked up as main window
            Thread.Sleep(30000);

            DesiredCapabilities appCapabilities = new DesiredCapabilities();
            appCapabilities.SetCapability("appTopLevelWindow", process.MainWindowHandle.ToString("x"));
            session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);

            driver = new AIWinDriver(session, process.Id);
        }

        public void TearDown()
        {
            TestContext.AddResultFile(SerializeRecordedEvents());
            Events.Clear();

            // closing ai-win like this stops it from saving the config. Will have to change this
            // if we ever want to use the saved config.
            process.Kill();

            if (session == null)
            {
                return;
            }

            session.Quit();
            session = null;
        }

        private bool IsWinAppDriverRunning()
        {
            var processes = Process.GetProcessesByName("WinAppDriver");
            return processes.Length > 0;
        }

        private string SerializeRecordedEvents()
        {
            var path = Path.Combine(TestContext.TestResultsDirectory, TestContext.TestName, "events.txt");
            using (StreamWriter w = File.AppendText(path))
            {
                foreach (var e in Events)
                {
                    w.WriteLine($"{e.TimeGenerated},{e.Source},{e.Message}");
                }
            }
            return path;
        }
    }
}