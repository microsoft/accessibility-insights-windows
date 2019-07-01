// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
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

        private static WindowsDriver<WindowsElement> session;
        protected static AIWinDriver driver;
        protected static int testAppProcessId;

        public static void Setup(TestContext context)
        {
            if (session != null)
            {
                return;
            }

            if (!IsWinAppDriverRunning())
            {
                Assert.Inconclusive("WinAppDriver.exe is not running");
            }

            LaunchApplicationAndAttach();

            Assert.IsNotNull(session);
            Assert.IsNotNull(session.SessionId);

            // Set implicit timeout to 1.5 seconds to ensure element search retries every 500 ms for at most three times
            session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
        }

        private static void LaunchApplicationAndAttach()
        {
            // AccessibilityInsights is referenced by this test project
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var exePath = Path.Combine(executingDirectory, "AccessibilityInsights.exe");

            using (Process process = Process.Start(exePath))
            {
                process.WaitForInputIdle();
                testAppProcessId = process.Id;
                // small buffer between splash screen disappearing 
                // and main window initializing; otherwise in rare
                // cases splash screen can be picked up as main window
                Thread.Sleep(2000);

                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("appTopLevelWindow", process.MainWindowHandle.ToString("x"));
                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);

                driver = new AIWinDriver(session, process.Id);
            }
        }

        public static void TearDown()
        {
            if (session == null)
            {
                return;
            }
            session.Close();
            session.Quit();
            session = null;
        }

        private static bool IsWinAppDriverRunning()
        {
            var processes = Process.GetProcessesByName("WinAppDriver");
            return processes.Length > 0;
        }
    }
}