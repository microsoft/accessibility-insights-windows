// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Automation;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace AccessibilityInsights.UITests
{
    /// <summary>
    /// This is a set of **ordered tests** that run through the major pages in the AccessibilityInsights app. These are meant to be proof of concept and are in no way comprehensive enough.
    /// </summary>
    [CodedUITest]
    public class CodedUITest
    {
        static Dictionary<string, string> TestParams = new Dictionary<string, string>();
        public CodedUITest()
        {
        }

        [ClassInitialize]
        public static void CodedUITestInit(TestContext testContext)
        {
            string restOfThePath = @"AccessibilityInsights\bin\Debug\AccessibilityInsights.exe";
            const string pathSeparator = "TestResults";
            const string targetProcessIdKey = "TargetProcessId";
            const string outputPathKey = "OutputPath";

            // Build path separator to executable
            object buildConfig = testContext.Properties["BuildConfiguration"];
            string buildConfiguration = buildConfig != null ? buildConfig.ToString() : "Debug";
            restOfThePath = restOfThePath.Replace("Debug", buildConfiguration);

            // Get path up to the path separator
            string currentDirectory = testContext.TestDir;
            currentDirectory = currentDirectory.Substring(0, currentDirectory.IndexOf(pathSeparator));

            // Start AccessibilityInsights
            string exePath = Path.Combine(currentDirectory, restOfThePath);
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(exePath);
            int processId = process.Id;

            // Set common test parameters
            string testDirectory = testContext.TestResultsDirectory;
            string opPath = testDirectory != null ? testDirectory : Path.Combine(currentDirectory, pathSeparator);
            TestParams.Add(targetProcessIdKey, processId.ToString());
            TestParams.Add(outputPathKey, opPath);

            // Start AccessibilityInsights
            StartCommandResult startResult = StartCommand.Execute(TestParams, "");
            Assert.IsTrue(startResult.Completed, "The start command failed to complete");
            Assert.IsTrue(startResult.Succeeded, "The start command completed but AccessibilityInsights failed to start up");
        }

        /// <summary>
        /// Tests the click once page if present.
        /// </summary>
        [TestMethod]
        public void ClickOnce()
        {
            SnapshotCommandResult result = this.UIMap.ClickOnceTest(testContext, TestParams);
            if (null != result)
            {
                this.UIMap.AssertScanResults(result, 0, 0, 0);
            }
        }

        /// <summary>
        /// Tests the Splash page.
        /// </summary>
        [TestMethod]
        public void Splash()
        {
            SnapshotCommandResult result = this.UIMap.SplashPageSnapShot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 458, 0, 12);
        }

        /// <summary>
        /// Tests the Live Inspect page.
        /// </summary>
        [TestMethod]
        public void LiveInspect()
        {
            SnapshotCommandResult result = this.UIMap.LiveInspectSnapShot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 1100, 0, 10);
        }

        /// <summary>
        /// Tests the Fast Pass page.
        /// </summary>
        [TestMethod]
        public void FastPass()
        {
            SnapshotCommandResult result = this.UIMap.FastPassSnapShot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 1262, 0, 7);
        }

        // Click 'Settings'. Opens up on the Application settings tab
        [TestMethod]
        public void ApplicationSettings()
        {
            SnapshotCommandResult result = this.UIMap.ApplicationSettingsSnapshot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 2186, 4, 12);
        }

        // Click the test tab within settings 
        [TestMethod]
        public void TestSettings()
        {
            SnapshotCommandResult result = this.UIMap.TestSettingsSnapShot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 2421, 4, 17);
        }

        // Click the 'What's new' tab within settings 
        [TestMethod]
        public void WhatsNewSettings()
        {
            SnapshotCommandResult result = this.UIMap.WhatsNewSettingsSnapShot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 2774, 12, 37);
        }

        // Click 'About tab' within settings
        [TestMethod]
        public void AboutSettings()
        {
            SnapshotCommandResult result = this.UIMap.AboutSettingsSnapShot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 2930, 4, 19);
        }

        // Click 'Feedback' within settings
        [TestMethod]
        public void FeedbackSettings()
        {
            SnapshotCommandResult result = this.UIMap.FeedBackSettingsSnapShot(testContext, TestParams);
            this.UIMap.AssertScanResults(result, 3064, 6, 23);
        }

        /// <summary>
        /// Cleans up by calling the stop command
        /// </summary>
        [ClassCleanup]
        public static void CleanUp()
        {
            StopCommand.Execute();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContext;
            }
            set
            {
                testContext = value;
            }
        }
        private TestContext testContext;

        public UIMap UIMap
        {
            get
            {
                if (this.map == null)
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
