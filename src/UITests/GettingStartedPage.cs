// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Axe.Windows.Automation;

namespace UITests
{
    [TestClass]
    public class GettingStartedPage : AIWinSession
    {
        [TestMethod]
        public void VerifyGettingStartedTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - ", session.Title);
        }

        [TestMethod]
        public void VerifyAccessibility()
        {
            var result = SnapshotCommand.Execute(new Dictionary<string, string>
            {
                { CommandConstStrings.TargetProcessId, testAppProcessId.ToString() },
            });
            Assert.AreEqual(0, result.ScanResultsPassedCount);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) => Setup(context);

        [ClassCleanup]
        public static void ClassCleanup() => TearDown();
    }
}