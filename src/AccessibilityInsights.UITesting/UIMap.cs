// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.UITests
{
    using AccessibilityInsights.Automation;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;


    public partial class UIMap
    {
        private const string OutputFileKey = "OutputFile";
        const string OutputPathKey = "OutputPath";

        Dictionary<string, string> StartParams = new Dictionary<string, string>();


        /// <summary>
        /// Test the click once page
        /// </summary>
        public SnapshotCommandResult ClickOnceTest(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click once snapshot and click on later
            return ClickOnceSnapShot(testContext, testParams);
        }

        public SnapshotCommandResult FeedBackSettingsSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click 'Feedback' within settings
            WpfButton uIFeedbacksettingstab6Button = this.UIAccessibilityInsightsWindowsStartWindow.UIContentcontainerCustom.UIFeedbacksettingstab6Button;
            Mouse.Click(uIFeedbacksettingstab6Button);
            SnapshotCommandResult res = TakeSnapShot(testContext, testParams, "Feedback");
            AttachResult(testContext, testParams[OutputPathKey], "Feedback");
            return res;
        }

        public SnapshotCommandResult AboutSettingsSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click 'About tab' within settings
            WpfButton uIAbouttab5of6Button = this.UIAccessibilityInsightsWindowsStartWindow.UIContentcontainerCustom.UIAbouttab5of6Button;
            Mouse.Click(uIAbouttab5of6Button);
            SnapshotCommandResult res = TakeSnapShot(testContext, testParams, "AboutSettings");
            AttachResult(testContext, testParams[OutputPathKey], "AboutSettings");
            return res;
        }

        public SnapshotCommandResult WhatsNewSettingsSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click the 'What's new' tab within settings 
            WpfButton uIWhatsnewsettingstab4Button = this.UIAccessibilityInsightsWindowsStartWindow.UIContentcontainerCustom.UIWhatsnewsettingstab4Button;
            Mouse.Click(uIWhatsnewsettingstab4Button);
            SnapshotCommandResult res = TakeSnapShot(testContext, testParams, "WhatsNewSettings");
            AttachResult(testContext, testParams[OutputPathKey], "WhatsNewSettings");
            return res;
        }

        public SnapshotCommandResult TestSettingsSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click the test tab within settings 
            WpfButton uITestsettingstab3of6Button = this.UIAccessibilityInsightsWindowsStartWindow.UIContentcontainerCustom.UITestsettingstab3of6Button;
            Mouse.Click(uITestsettingstab3of6Button);
            SnapshotCommandResult res = TakeSnapShot(testContext, testParams, "TestSettings");
            AttachResult(testContext, testParams[OutputPathKey], "TestSettings");
            return res;
        }

        public SnapshotCommandResult ApplicationSettingsSnapshot(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click 'Settings'. Opens up on the Application settings tabd
            WpfButton uISettings1of2Button = this.UIAccessibilityInsightsWindowsStartWindow.UINavigationBarCustom.UISettings1of2Button;
            Mouse.Click(uISettings1of2Button);
            SnapshotCommandResult res = TakeSnapShot(testContext, testParams, "ApplicationSettings");
            AttachResult(testContext, testParams[OutputPathKey], "ApplicationSettings");
            return res;
        }

        public SnapshotCommandResult LiveInspectSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click 'Tests fastpass mode 2 of 2' button
            SnapshotCommandResult res = TakeSnapShot(testContext, testParams, "LiveInspect");
            AttachResult(testContext, testParams[OutputPathKey], "LiveInspect");
            return res;
        }

        public SnapshotCommandResult FastPassSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            SnapshotCommandResult res;
            WpfButton uITestsfastpassmode2ofButton = this.UIAccessibilityInsightsWindowsStartWindow.UINavigationBarCustom.UITestsfastpassmode2ofButton;
            Mouse.Click(uITestsfastpassmode2ofButton);
            res = TakeSnapShot(testContext, testParams, "FastPass");
            AttachResult(testContext, testParams[OutputPathKey], "FastPass");
            return res;
        }

        private SnapshotCommandResult ClickOnceSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            SnapshotCommandResult res = null;
            // Click 'Later' button
            WpfButton uILaterButton = this.UIAccessibilityInsightsWindowsStartWindow.UILaterButton;
            if (uILaterButton.TryFind())
            {
                res = TakeSnapShot(testContext, testParams, "ClickOnce");
                Mouse.Click(uILaterButton);
                AttachResult(testContext, testParams[OutputPathKey], "ClickOnce");
            }
            return res;
        }

        public SnapshotCommandResult SplashPageSnapShot(TestContext testContext, Dictionary<string, string> testParams)
        {
            // Click 'Continue' button
            WpfButton uIContinueButton = this.UIAccessibilityInsightsWindowsStartWindow.UIWelcomepageCustom.UIContinueButton;
            SnapshotCommandResult res = TakeSnapShot(testContext, testParams, "LandingPage");
            Mouse.Click(uIContinueButton);
            AttachResult(testContext, testParams[OutputPathKey], "LandingPage");
            return res;
        }

        /// <summary>
        /// Convenience method to attach the generated result artifact as a part of a particular test case.
        /// </summary>
        /// <param name="testContext"> The test context </param>
        /// <param name="path"> The path to the directory of the file </param>
        /// <param name="fileName"> The name of the file to be attached </param>
        private static void AttachResult(TestContext testContext, string path, String fileName)
        {
            string opPath = Path.Combine(path, fileName);
            testContext.AddResultFile(opPath);
        }

        /// <summary>
        /// Convenience method to updated the test paramenters and invoke the SnapShotCommand
        /// </summary>
        /// <param name="testContext"></param>
        /// <param name="testParams"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        private static SnapshotCommandResult TakeSnapShot(TestContext testContext, Dictionary<string, string> testParams, string outputFileName)
        {
            testParams[OutputPathKey] = testContext.TestResultsDirectory;
            testParams[OutputFileKey] = outputFileName;
            return SnapshotCommand.Execute(testParams);
        }

        /// <summary>
        /// Convenience method to assert the result (The snapshot command completed) and the expected number of passed, failed and inconclusive scans expected.
        /// </summary>
        /// <param name="result"> The actual result object </param>
        /// <param name="passedExpected"> No. of scans that are expected to pass </param>
        /// <param name="failedExpected"> No. of scans that are expected to fail </param>
        /// <param name="inconclusiveExpected"> No. of scans that are expected to be inconclusive </param>
        public void AssertScanResults(SnapshotCommandResult result, int passedExpected, int failedExpected, int inconclusiveExpected)
        {
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Completed, "The snapshot command failed to complete");
            Assert.AreEqual(passedExpected, result.ScanResultsPassed, "The number of scan results that passed is different from the number of results expected.");
            Assert.AreEqual(failedExpected, result.ScanResultsFailed, "The number of scan results that failed is different from the number of results expected.");
            Assert.AreEqual(inconclusiveExpected, result.ScanResultsInconclusive, "The number of scan results that are inconclusive is different from the number of results expected.");
        }
    }
}
