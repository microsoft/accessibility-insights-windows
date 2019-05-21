// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UITests
{
    [TestClass]
    public class AIWinSession
    {
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";

        protected static WindowsDriver<WindowsElement> session;
        protected static int testAppProcessId;
        protected static int driverProcessId;

        public static void Setup(TestContext context)
        {
            if (session != null)
            {
                return;
            }

            if (!IsWinAppDriverRunning())
            {
                StartWinAppDriver();
            }

            LaunchApplicationAndAttach();

            Assert.IsNotNull(session);
            Assert.IsNotNull(session.SessionId);

            // Set implicit timeout to 1.5 seconds to ensure element search retries every 500 ms for at most three times
            session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);

            StartCommand.Execute(null, string.Empty);
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

                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("appTopLevelWindow", process.MainWindowHandle.ToString("x"));
                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
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

            try
            {
                Process.GetProcessById(driverProcessId).Close();
            }
            catch { }

            StopCommand.Execute();
        }

        private static bool IsWinAppDriverRunning()
        {
            var processes = Process.GetProcessesByName("WinAppDriver");
            return processes.Length > 0;
        }

        private static void StartWinAppDriver()
        {
            var driverExePath = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
            ProcessStartInfo info = new ProcessStartInfo(driverExePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            using (Process process = Process.Start(info))
            {
                driverProcessId = process.Id;
            }
        }
    }
}