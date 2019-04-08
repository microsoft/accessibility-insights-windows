// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.IO;
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if FAKES_SUPPORTED
using Microsoft.QualityTools.Testing.Fakes;
using System.IO.Fakes;
#endif

namespace Axe.Windows.AutomationTests
{
    [TestClass]
    public class LocationHelperUnitTests
    {
        private const string TestOutputPath = "C:\\Output\\Path";
        private const string TestOutputFileWithoutExtension = "MyOutputFile";
        private const string TestOutputFileWithExtension = TestOutputFileWithoutExtension + LocationHelper.TestExtension;
        private const string TestOutputFileFormat = "Sarif";
        private const string InvalidTestOutputFormat = "testing";
        private const string EnvOutputPath = "C:\\Env\\Output\\Path";
        private const string CWDOutputPath = "C:\\Current\\Directory\\Path";
        private const string FallbackResultsFolder = "ScanResults";

        private CommandParameters BuildParameters(string outputPath = TestOutputPath,
            string outputFile = TestOutputFileWithExtension, string outputFileFormat = TestOutputFileFormat)
        {
            Dictionary<string, string> inputs = new Dictionary<string, string>();

            if (outputPath != null)
                inputs[CommandConstStrings.OutputPath] = outputPath;

            if (outputFile != null)
                inputs[CommandConstStrings.OutputFile] = outputFile;

            if (outputFileFormat != null)
                inputs[CommandConstStrings.OutputFileFormat] = outputFileFormat;

            return new CommandParameters(inputs, string.Empty);
        }

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_OutputPathIsTrivial_ThrowsAutomationException_ErrorAutomation004()
        {
            CommandParameters parameters = BuildParameters(outputPath: string.Empty);
            try
            {
                new LocationHelper(parameters);
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation004:"));
                throw;
            }
        }

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_OutputFileIsOmitted_ThrowsAutomationException_ErrorAutomation005()
        {
            CommandParameters parameters = BuildParameters(outputFile: null);
            try
            {
                new LocationHelper(parameters);
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation005:"));
                throw;
            }
        }

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_OutputFileIsTrivial_ThrowsAutomationException_ErrorAutomation006()
        {
            CommandParameters parameters = BuildParameters(outputFile: string.Empty);
            try
            {
                new LocationHelper(parameters);
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation006:"));
                throw;
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout (1000)]
        public void Ctor_OutputPathDoesNotExist_CreatesOutputPath()
        {
            using (ShimsContext.Create())
            {
                int directoryCreates = 0;
                ShimDirectory.ExistsString = (_) => false;
                ShimDirectory.CreateDirectoryString = (_) => 
                {
                    directoryCreates++;
                    return new ShimDirectoryInfo();
                };

                CommandParameters parameters = BuildParameters();
                new LocationHelper(parameters);
                Assert.AreEqual(1, directoryCreates);
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_OutputPathExists_DoesNotCreatesOutputPath()
        {
            using (ShimsContext.Create())
            {
                int directoryCreates = 0;

                ShimDirectory.ExistsString = (_) => true;
                ShimDirectory.CreateDirectoryString = (_) =>
                {
                    directoryCreates++;
                    return new ShimDirectoryInfo();
                };

                CommandParameters parameters = BuildParameters();
                new LocationHelper(parameters);
                Assert.AreEqual(0, directoryCreates);
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_OutputFileWithoutExtension_EnsureProperties()
        {
            using (ShimsContext.Create())
            {
                ShimDirectory.ExistsString = (_) => true;

                CommandParameters parameters = BuildParameters(outputFile: TestOutputFileWithoutExtension);
                LocationHelper helper = new LocationHelper(parameters);
                Assert.AreEqual(TestOutputPath, helper.OutputPath);
                Assert.AreEqual(TestOutputFileWithoutExtension, helper.OutputFile);
                Assert.AreEqual(LocationHelper.SarifExtension.Substring(1), helper.OutputFileFormat, true);
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_OutputFileWithExtension_EnsureProperties()
        {
            using (ShimsContext.Create())
            {
                ShimDirectory.ExistsString = (_) => true;

                CommandParameters parameters = BuildParameters(outputFile: TestOutputFileWithExtension);
                LocationHelper helper = new LocationHelper(parameters);
                Assert.AreEqual(TestOutputPath, helper.OutputPath);
                Assert.AreEqual(TestOutputFileWithoutExtension, helper.OutputFile);
                Assert.AreEqual(LocationHelper.TestExtension.Substring(1), helper.OutputFileFormat, true);
            }
        }
#endif

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_OutputFileFormatTrivial_ThrowsAutomationException_ErrorAutomation015()
        {
            CommandParameters parameters = BuildParameters(outputFile: TestOutputFileWithoutExtension, outputFileFormat: " ");
            try
            {
                new LocationHelper(parameters);
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation015:"));
                throw;
            }
        }

        [TestMethod]
        [Timeout (1000)]
        [ExpectedException(typeof(A11yAutomationException))]
        public void Ctor_OutputFileFormatInvalid_ThrowsAutomationException_ErrorAutomation016()
        {
            CommandParameters parameters = BuildParameters(outputFile: TestOutputFileWithoutExtension, outputFileFormat: InvalidTestOutputFormat);
            try
            {
                new LocationHelper(parameters);
            }
            catch (A11yAutomationException e)
            {
                Assert.IsTrue(e.Message.Contains(" Automation016:"));
                throw;
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout (1000)]
        public void Ctor_NoOutputPathProvidedNullEnvValue_EnsureFetchesFallbackValue()
        {
            CommandParameters parameters = BuildParameters(outputPath: null);
            using (ShimsContext.Create())
            {
                System.Fakes.ShimEnvironment.GetEnvironmentVariableString = (_) => null;
                System.Fakes.ShimEnvironment.CurrentDirectoryGet = () => CWDOutputPath;

                LocationHelper helper = new LocationHelper(parameters);

                Assert.AreEqual(Path.Combine(CWDOutputPath, FallbackResultsFolder), helper.OutputPath);
                Assert.AreEqual(TestOutputFileWithoutExtension, helper.OutputFile);
                Assert.AreEqual(LocationHelper.TestExtension.Substring(1), helper.OutputFileFormat, true);
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_NoOutputPathProvidedEmptyEnvValue_EnsureFetchesFallbackValue()
        {
            CommandParameters parameters = BuildParameters(outputPath: null);
            using (ShimsContext.Create())
            {
                System.Fakes.ShimEnvironment.GetEnvironmentVariableString = (_) => string.Empty;
                System.Fakes.ShimEnvironment.CurrentDirectoryGet = () => CWDOutputPath;

                LocationHelper helper = new LocationHelper(parameters);

                Assert.AreEqual(Path.Combine(CWDOutputPath, FallbackResultsFolder), helper.OutputPath);
                Assert.AreEqual(TestOutputFileWithoutExtension, helper.OutputFile);
                Assert.AreEqual(LocationHelper.TestExtension.Substring(1), helper.OutputFileFormat, true);
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Ctor_NoOutputPathProvidedValidEnvValue_EnsureFetchesValidValueFromEnv()
        {
            CommandParameters parameters = BuildParameters(outputPath: null);
            using (ShimsContext.Create())
            {
                System.Fakes.ShimEnvironment.GetEnvironmentVariableString = (_) => EnvOutputPath;

                LocationHelper helper = new LocationHelper(parameters);

                Assert.AreEqual(EnvOutputPath, helper.OutputPath);
                Assert.AreEqual(TestOutputFileWithoutExtension, helper.OutputFile);
                Assert.AreEqual(LocationHelper.TestExtension.Substring(1), helper.OutputFileFormat, true);
            }
        }

        // In case the output path is provided + env variable is set and a fallback is provided that the user provided output path should be returned.
        // This is a guard against future changes that might break this hierarchy
        [TestMethod]
        [Timeout (1000)]
        public void Ctor_OutputPathProvided_EnsurePreservesOutputPathSelectionHierarchy()
        {
            CommandParameters parameters = BuildParameters();
            using (ShimsContext.Create())
            {
                System.Fakes.ShimEnvironment.GetEnvironmentVariableString = (_) => EnvOutputPath;
                System.Fakes.ShimEnvironment.CurrentDirectoryGet = () => CWDOutputPath;

                LocationHelper helper = new LocationHelper(parameters);

                Assert.AreEqual(TestOutputPath, helper.OutputPath);
                Assert.AreEqual(TestOutputFileWithoutExtension, helper.OutputFile);
                Assert.AreEqual(LocationHelper.TestExtension.Substring(1), helper.OutputFileFormat, true);
            }
        }
#endif
    }
}
