// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace UITests
{
    [TestClass]
    public class GettingStartedPage : AIWinSession
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory("NoStrongName")]
        public void VerifyGettingStartedTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - ", session.Title);
        }

        [TestMethod]
        [TestCategory("NoStrongName")]
        public void VerifyAccessibility()
        {
            var result = SnapshotCommand.Execute(new Dictionary<string, string>
            {
                { CommandConstStrings.TargetProcessId, testAppProcessId.ToString() },
                { CommandConstStrings.OutputFile, "GettingStartedPage" },
                { CommandConstStrings.OutputFileFormat, "a11ytest" }
            });
            Assert.AreEqual(0, result.ScanResultsFailedCount);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);

            // Close telemetry dialog if open
            var elements = session.FindElementsByXPath("//Button[@Name=\"OK\"]");
            if (elements.Count > 0)
            {
                elements[0].Click();
            }
        }

        [ClassCleanup]
        public static void ClassCleanup() => TearDown();
    }
}