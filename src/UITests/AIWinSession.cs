using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace UITests
{
    [TestClass]
    public class AIWinSession
    {
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";

        protected static WindowsDriver<WindowsElement> session;
        protected static int processId;

        public static void Setup(TestContext context)
        {
            // Launch a new instance of application
            if (session == null)
            {
                // Create a new session to launch application
                DesiredCapabilities appCapabilities = new DesiredCapabilities();

                var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var exeFiles = Directory.GetParent(currentDir).Parent.Parent.GetFiles("AccessibilityInsights.exe", SearchOption.AllDirectories);

                if (exeFiles.Length > 0)
                {
                    appCapabilities.SetCapability("app", exeFiles[0].FullName);
                }

                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.SessionId);

                // Set implicit timeout to 1.5 seconds to ensure element search retries every 500 ms for at most three times
                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);

                var processes = Process.GetProcessesByName("AccessibilityInsights");
                if (processes.Length > 0)
                {
                    processId = processes[0].Id;
                }

                StartCommand.Execute(null, string.Empty);
            }
        }

        public static void TearDown()
        {
            if (session != null)
            {
                session.Close();

                session.Quit();
                session = null;
            }

            StopCommand.Execute();
        }
    }
}
