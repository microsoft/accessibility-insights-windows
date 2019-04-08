// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;

namespace Axe.Windows.Automation
{
    /// <summary>
    /// Class to collect common location functionality into a single place
    /// </summary>
    internal class LocationHelper
    {
        private const string OutputPathEnvironmentVariableName = "A11YSCANRESULTSFOLDER";
        private const string FallbackResultsFolder = "ScanResults";
        public const string SarifExtension = "sarif";
        public const string TestExtension = "a11ytest";

        internal string OutputPath { get; }
        internal string OutputFile { get; }
        internal string OutputFileFormat { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parameters">Caller-specified command parameters. Will throw if the parameters
        /// are invalid.</param>
        internal LocationHelper(CommandParameters parameters)
        {
            if (!parameters.TryGetString(CommandConstStrings.OutputPath, out string outputPath))
            {
                outputPath = GetEnvironmentVariablePathValue() ?? GetFallbackPathValue();
            }

            if (string.IsNullOrWhiteSpace(outputPath))
                throw new A11yAutomationException(DisplayStrings.ErrorOutputPathIsTrivial);

            if (!parameters.TryGetString(CommandConstStrings.OutputFile, out string outputFile))
                throw new A11yAutomationException(DisplayStrings.ErrorOutputFileIsNotSpecified);

            if (string.IsNullOrWhiteSpace(outputFile))
                throw new A11yAutomationException(DisplayStrings.ErrorOutputFileIsTrivial);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            string outputFileFormat = null;
            string extension = Path.GetExtension(outputFile);

            // If extension is missing in the file name
            if (string.IsNullOrWhiteSpace(extension))
            {
                // If output file format is PRESENT check it is valid
                if (parameters.TryGetString(CommandConstStrings.OutputFileFormat, out outputFileFormat))
                {
                    if (string.IsNullOrWhiteSpace(outputFileFormat))
                    {
                        throw new A11yAutomationException(DisplayStrings.ErrorOutputFileFormatIsTrivial);
                    }
                    outputFileFormat = ValidateExtension(outputFileFormat);
                }
                else
                {
                    // If file format absent set default value
                    outputFileFormat = ValidateExtension("." + SarifExtension);
                }
            }
            else
            {
                // Validate the extension that was passed in the filename
                outputFileFormat = ValidateExtension(extension);
                outputFile = Path.GetFileNameWithoutExtension(outputFile);
            }

            // If extension is present in the filename and a fileformat is also being passed in, we will honor the extension in the filename.
            this.OutputPath = outputPath;
            this.OutputFileFormat = outputFileFormat;
            this.OutputFile = outputFile;
        }

        private static string ValidateExtension(string extension)
        {
            if (extension.IndexOf(".", StringComparison.Ordinal) == 0 && extension.Length > 1)
            {
                extension = extension.Substring(1);
            }

            // If extension is present in the file name make sure that it is a valid extension
            if (!Enum.TryParse(extension, true, out Actions.Enums.FileExtensionType enumExtension))
            {
                throw new A11yAutomationException(DisplayStrings.ErrorInvalidOutputFileFormat);
            }

            return enumExtension.ToString();
        }

        /// <summary>
        /// Fetches the fallback value for the output directory (in the absence of any user specified output path preferences). This is the last value in terms of preference.
        /// </summary>
        internal static string GetFallbackPathValue()
        {
            return Path.Combine(Environment.CurrentDirectory, FallbackResultsFolder);
        }

        /// <summary>
        /// Checks the environment to see whether the 'A11yScanResultsFolder' variable is set.This variable will be set by the prep task for the pipeline.
        /// </summary>
        /// <returns>The value of the variable if it exists. Null if whitespace</returns>
        internal static string GetEnvironmentVariablePathValue()
        {
            string outputPath = Environment.GetEnvironmentVariable(OutputPathEnvironmentVariableName);
            return String.IsNullOrWhiteSpace(outputPath) ? null : outputPath;
        }

        /// <summary>
        /// Checks whether a A11yTest file should be generated
        /// </summary>
        /// <returns> Whether the A11yTest file should be generated </returns>
        internal Boolean IsA11yTestExtension()
        {
            return IsAllOption() || Actions.Enums.FileExtensionType.A11yTest.ToString().Equals(this.OutputFileFormat, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether a SARIF file should be generated
        /// </summary>
        /// <returns> Whether a SARIF file should be generated </returns>
        internal Boolean IsSarifExtension()
        {
            return IsAllOption() || Actions.Enums.FileExtensionType.Sarif.ToString().Equals(this.OutputFileFormat, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether the all format was chosen
        /// </summary>
        /// <returns> true if the ALL option was chosen </returns>
        internal Boolean IsAllOption()
        {
            return Actions.Enums.FileExtensionType.All.ToString().Equals(this.OutputFileFormat, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Generates the path that the output files should be persisted at.
        /// </summary>
        /// <returns> Generated path to results file </returns>
        internal string GetOutputFilePath()
        {
            return Path.ChangeExtension(Path.Combine(this.OutputPath, this.OutputFile), Actions.Enums.FileExtensionType.A11yTest.ToString());
        }

        /// <summary>
        /// Geneartes the path that the SARIF files should be persisted at.
        /// </summary>
        /// <returns> Generated path to SARIF file </returns>
        internal string GetSarifFilePath()
        {
            return Path.ChangeExtension(Path.Combine(this.OutputPath, this.OutputFile), Actions.Enums.FileExtensionType.Sarif.ToString());
        }
    }
}
