// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.VersionSwitcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace AccessibilityInsights.VersionSwitcherUnitTests
{
    [TestClass]
    public class InstallationEngineUnitTests
    {
        private const string CommandIgnored = "ignored";
        private const string CommandMsiPath = "https://azure.com/this/is/where/the/msi/is/found";
        private const string CommandMsiSize = "79";
        private const string CommandNewChannel = "TestChannel";
        private readonly string TestFile =  Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\..\..\TestFiles\SampleFile.txt"));
        private const int TestFileSize = 79;
        private const int BadTestFileSize = TestFileSize + 1;
        private const string TestFileSha512 = "B53D5F87E96D5D69F22EA229E4205B474D51CF068D04C03C223F87759E28DE35DC8118C8BB0AD70732BB0DCD60A60A0F4B5C5DE9B5147FB0B01E9091A79155AD";
        private static readonly string TestFileSha512MixedCase = TestFileSha512.Replace('D', 'd');
        private static readonly string BadTestFileSha512 = TestFileSha512.Replace('D', 'C');

        private InstallationEngine _engine;
        private ExecutionHistory _history;
        private string[] _commandLineArgs;

        [TestInitialize]
        public void BeforeEach()
        {
            _commandLineArgs = new string[0];
            _history = new ExecutionHistory();
            _engine = new InstallationEngine("TestProductName", "TestAppToLaunchAfterInstall", _history, TestCommandLineProvider);
        }

        private string[] TestCommandLineProvider()
        {
            return _commandLineArgs;
        }

        [TestMethod]
        public void GetInstallationOptions_ZeroParameters_ThrowsResultBearingException()
        {
            var e = Assert.ThrowsException<ResultBearingException>(() => _engine.GetInstallationOptions());
            Assert.AreEqual(ExecutionResult.ErrorBadCommandLine, e.Result);
        }

        [TestMethod]
        public void GetInstallationOptions_OneParameter_ThrowsResultBearingException()
        {
            _commandLineArgs = new string[] { CommandIgnored };
            var e = Assert.ThrowsException<ResultBearingException>(() => _engine.GetInstallationOptions());
            Assert.AreEqual(ExecutionResult.ErrorBadCommandLine, e.Result);
        }

        [TestMethod]
        public void GetInstallationOptions_TwoParameters_ThrowsResultBearingException()
        {
            _commandLineArgs = new string[] { CommandIgnored, CommandMsiPath };
            var e = Assert.ThrowsException<ResultBearingException>(() => _engine.GetInstallationOptions());
            Assert.AreEqual(ExecutionResult.ErrorBadCommandLine, e.Result);
        }

        [TestMethod]
        public void GetInstallationOptions_ThreeParameters_ThrowsResultBearingException()
        {
            _commandLineArgs = new string[] { CommandIgnored, CommandMsiPath, CommandMsiSize };
            var e = Assert.ThrowsException<ResultBearingException>(() => _engine.GetInstallationOptions());
            Assert.AreEqual(ExecutionResult.ErrorBadCommandLine, e.Result);
        }

        [TestMethod]
        public void GetInstallationOptions_SizeIsNotAnInteger_ThrowsResultBearingException()
        {
            _commandLineArgs = new string[] { CommandIgnored, CommandMsiPath, "not an integer", TestFileSha512};
            var e = Assert.ThrowsException<ResultBearingException>(() => _engine.GetInstallationOptions());
            Assert.AreEqual(ExecutionResult.ErrorBadCommandLine, e.Result);
        }

        [TestMethod]
        public void GetInstallationOptions_PathIsNotValid_ThrowsResultBearingException()
        {
            _commandLineArgs = new string[] { CommandIgnored, "Not a valid url", CommandMsiSize, TestFileSha512 };
            var e = Assert.ThrowsException<ResultBearingException>(() => _engine.GetInstallationOptions());
            Assert.AreEqual(ExecutionResult.ErrorBadCommandLine, e.Result);
        }

        [TestMethod]
        public void GetInstallationOptions_SizeIsZero_OptionsAreCorrect()
        {
            _commandLineArgs = new string[] { CommandIgnored, CommandMsiPath, "0", TestFileSha512 };
            var options = _engine.GetInstallationOptions();

            Assert.AreEqual(CommandMsiPath, options.MsiPath.ToString());
            Assert.AreEqual(0, options.MsiSizeInBytes);
            Assert.AreEqual(TestFileSha512, options.MsiSha512);
            Assert.IsNull(options.NewChannel);

            Assert.AreEqual(ExecutionResult.Unknown, _history.TypedExecutionResult);
        }

        [TestMethod]
        public void GetInstallationOptions_ChannelIsSet_OptionsAreCorrect()
        {
            _commandLineArgs = new string[] { CommandIgnored, CommandMsiPath, CommandMsiSize, TestFileSha512, CommandNewChannel };
            var options = _engine.GetInstallationOptions();
            _engine.SetInitialTelemetryValues(options);

            Assert.AreEqual(CommandMsiPath, options.MsiPath.ToString());
            Assert.AreEqual(TestFileSize, options.MsiSizeInBytes);
            Assert.AreEqual(TestFileSha512, options.MsiSha512);
            Assert.AreEqual(CommandNewChannel, options.NewChannel);

            Assert.AreEqual(ExecutionResult.Unknown, _history.TypedExecutionResult);
        }

        [TestMethod]
        public void SetInitialTelemetryValues_NoChannelIsProvided_SetsCorrectValues()
        {
            InstallationOptions options = new InstallationOptions(new Uri(CommandMsiPath), TestFileSize, TestFileSha512, null, false);
            _engine.SetInitialTelemetryValues(options);
            Assert.IsNotNull(_history.StartingVersion);
            Assert.AreEqual(ExecutionResult.Unknown, _history.TypedExecutionResult);
            Assert.AreEqual(CommandMsiPath, _history.RequestedMsi);
            Assert.AreEqual(TestFileSize, _history.ExpectedMsiSizeInBytes);
            Assert.AreEqual(TestFileSha512, _history.ExpectedMsiSha512);
            Assert.IsNull(_history.NewChannel);
        }

        [TestMethod]
        public void SetInitialTelemetryValues_ChannelIsProvided_SetsCorrectValues()
        {
            InstallationOptions options = new InstallationOptions(new Uri(CommandMsiPath), TestFileSize, TestFileSha512, CommandNewChannel, false);
            _engine.SetInitialTelemetryValues(options);
            Assert.IsNotNull(_history.StartingVersion);
            Assert.AreEqual(ExecutionResult.Unknown, _history.TypedExecutionResult);
            Assert.AreEqual(CommandMsiPath, _history.RequestedMsi);
            Assert.AreEqual(TestFileSize, _history.ExpectedMsiSizeInBytes);
            Assert.AreEqual(TestFileSha512, _history.ExpectedMsiSha512);
            Assert.AreEqual(CommandNewChannel, _history.NewChannel);
        }

        private void AssertCorrectTelemetryValuesForValidateFileProperties()
        {
            // These are always set by ValidateFileProperties
            Assert.AreEqual(TestFileSha512, _history.ActualMsiSha512);
            Assert.AreEqual(TestFileSize, _history.ActualMsiSizeInBytes);

            // These are never set by ValidateFileProperties so validate the default
            Assert.AreEqual(0, _history.ExpectedMsiSizeInBytes);
            Assert.IsNull(_history.ExpectedMsiSha512);
        }

        [TestMethod]
        public void EnsureValidInputsForValidateFileProperties()
        {
            Assert.AreNotEqual(TestFileSha512, TestFileSha512MixedCase);
            Assert.AreNotEqual(TestFileSha512, BadTestFileSha512);
        }

        [TestMethod]
        public void ValidateFileProperties_SizeIsOmitted_ShaIsBad_ThrowsResultBearingException()
        {
            ResultBearingException e = Assert.ThrowsException<ResultBearingException>(() => _engine.ValidateFileProperties(TestFile, TestFileSize, BadTestFileSha512));
            Assert.AreEqual(ExecutionResult.ErrorMsiSha512Mismatch, e.Result);
            AssertCorrectTelemetryValuesForValidateFileProperties();
        }

        [TestMethod]
        public void ValidateFileProperties_SizeIsCorrect_ShaIsCorrect_Passes()
        {
            _engine.ValidateFileProperties(TestFile, TestFileSize, TestFileSha512);
            Assert.AreEqual(ExecutionResult.Unknown, _history.TypedExecutionResult);
            AssertCorrectTelemetryValuesForValidateFileProperties();
        }

        [TestMethod]
        public void ValidateFileProperties_SizeIsCorrect_ShaIsCorrectWithCaseInsensitiveMatch_Passes()
        {
            _engine.ValidateFileProperties(TestFile, TestFileSize, TestFileSha512MixedCase);
            Assert.AreEqual(ExecutionResult.Unknown, _history.TypedExecutionResult);
            AssertCorrectTelemetryValuesForValidateFileProperties();
        }

        [TestMethod]
        public void ValidateFileProperties_SizeIsCorrect_ShaIsBad_ThrowsResultBearingException()
        {
            ResultBearingException e = Assert.ThrowsException<ResultBearingException>(() => _engine.ValidateFileProperties(TestFile, TestFileSize, BadTestFileSha512));
            Assert.AreEqual(ExecutionResult.ErrorMsiSha512Mismatch, e.Result);
            AssertCorrectTelemetryValuesForValidateFileProperties();
        }

        [TestMethod]
        public void ValidateFileProperties_SizeIsBad_ShaIsOmitted_ThrowsResultBearingException()
        {
            ResultBearingException e = Assert.ThrowsException<ResultBearingException>(() => _engine.ValidateFileProperties(TestFile, BadTestFileSize, null));
            Assert.AreEqual(ExecutionResult.ErrorMsiSizeMismatch, e.Result);
            AssertCorrectTelemetryValuesForValidateFileProperties();
        }

        [TestMethod]
        public void ValidateFileProperties_SizeIsBad_ShaIsCorrect_ThrowsResultBearingException()
        {
            ResultBearingException e = Assert.ThrowsException<ResultBearingException>(() => _engine.ValidateFileProperties(TestFile, BadTestFileSize, TestFileSha512));
            Assert.AreEqual(ExecutionResult.ErrorMsiSizeMismatch, e.Result);
            AssertCorrectTelemetryValuesForValidateFileProperties();
        }

        [TestMethod]
        public void ValidateFileProperties_SizeIsBad_ShaIsBad_ThrowsResultBearingException()
        {
            ResultBearingException e = Assert.ThrowsException<ResultBearingException>(() => _engine.ValidateFileProperties(TestFile, BadTestFileSize, BadTestFileSha512));
            Assert.AreEqual(ExecutionResult.ErrorMsiSizeMismatch, e.Result);
            AssertCorrectTelemetryValuesForValidateFileProperties();
        }
    }
}
