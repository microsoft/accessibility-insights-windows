// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CustomActions;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.CustomActionsUnitTests
{
    [TestClass]
    public class ConfigFileCleanerUnitTests
    {
        private Mock<ISystemShim> _systemShimMock;
        private ConfigFileCleaner _cleaner;

        [TestInitialize]
        public void TestInit()
        {
            _systemShimMock = new Mock<ISystemShim>(MockBehavior.Strict);
            _systemShimMock.Setup(x => x.GetRunningProcessNames())
                .Returns(new List<string> { "abc", "def" });
            _systemShimMock.Setup(x => x.LogToSession(It.IsAny<string>()));

            _cleaner = new ConfigFileCleaner(_systemShimMock.Object);
        }

        [TestMethod]
        public void RunAction_VersionSwitcherIsRunning_ConfigFilesAreIgnored()
        {
            _systemShimMock.Setup(x => x.GetRunningProcessNames())
                .Returns(new List<string> { "abc", "AccessibilityInsights.VersionSwitcher", "def" });

            Assert.AreEqual(ActionResult.Success, _cleaner.RunAction());

            _systemShimMock.VerifyAll();
        }

        [TestMethod]
        public void RunAction_VersionSwitcherIsNotRunning_NoConfigFilesExist_NothingIsDeleted()
        {
            _systemShimMock.Setup(x => x.GetConfigFiles())
                .Returns(Enumerable.Empty<string>);

            Assert.AreEqual(ActionResult.Success, _cleaner.RunAction());

            _systemShimMock.VerifyAll();
        }

        [TestMethod]
        public void RunAction_VersionSwitcherIsNotRunning_ConfigFilesExist_FilesAreDeleted()
        {
            List<string> configFiles = new List<string>{ "a.json", "b.json", "c.json" };
            List<string> deletedFiles = new List<string>();

            _systemShimMock.Setup(x => x.GetConfigFiles())
                .Returns(configFiles);
            _systemShimMock.Setup(x => x.DeleteFile(It.IsAny<string>()))
                .Callback<string>(fileName => deletedFiles.Add(fileName));

            Assert.AreEqual(ActionResult.Success, _cleaner.RunAction());
            Assert.IsTrue(configFiles.SequenceEqual(deletedFiles));

            _systemShimMock.VerifyAll();
        }

        [TestMethod]
        public void RunAction_ExceptionThrownDuringExecution_EatsAndLogsException()
        {
            List<string> logMessages = new List<string>();

            _systemShimMock.Setup(x => x.GetRunningProcessNames())
                .Throws<AccessViolationException>();
            _systemShimMock.Setup(x => x.LogToSession(It.IsAny<string>()))
                .Callback<string>(message => logMessages.Add(message));

            Assert.AreEqual(ActionResult.Success, _cleaner.RunAction());
            Assert.AreEqual(1, logMessages.Count);
            Assert.IsTrue(logMessages[0].Contains("AccessViolationException"));

            _systemShimMock.VerifyAll();
        }
    }
}
