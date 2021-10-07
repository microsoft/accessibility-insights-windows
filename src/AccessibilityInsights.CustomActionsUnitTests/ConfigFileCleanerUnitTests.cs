// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CustomActions;
using AccessibilityInsights.SetupLibrary;
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
        public void RunAction_VersionSwitcherIsNotRunning_ConfigFilesExist_FilesAreDeleted()
        {
            List<string> deletedDirs = new List<string>();

            var configDirectory = FixedConfigSettingsProvider.CreateDefaultSettingsProvider().ConfigurationFolderPath;
            _systemShimMock.Setup(x => x.DirectoryExists(It.IsAny<string>()))
                .Returns<string>(fileName => fileName.Equals(configDirectory));
            _systemShimMock.Setup(x => x.DeleteDirectory(It.IsAny<string>()))
                .Callback<string>(fileName => deletedDirs.Add(fileName));

            Assert.AreEqual(ActionResult.Success, _cleaner.RunAction());
            Assert.IsTrue(deletedDirs.Count == 1 && deletedDirs.First().Equals(configDirectory));

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
