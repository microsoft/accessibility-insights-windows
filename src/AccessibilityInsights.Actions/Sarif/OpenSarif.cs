// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis.Sarif;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Axe.Windows.Actions.Sarif
{
    public static class OpenSarif
    {
        /// <summary>
        /// Opens the Sarif file at the given path and extracts the 
        /// first a11ytest file found as a base64-encoded string
        /// </summary>
        /// <param name="filePath">path to sarif file</param>
        /// <returns>a11ytest file data</returns>
        public static string ExtractA11yTestFile(string filePath)
        {
            string sarifData = File.ReadAllText(filePath);
            List<SarifLog> baselineLogs = new List<SarifLog>();
            try
            {
                baselineLogs.AddRange(JsonConvert.DeserializeObject<List<SarifLog>>(sarifData));
            }
            catch
            {
                baselineLogs.Add(JsonConvert.DeserializeObject<SarifLog>(sarifData));
            }

            // Find first a11y file referenced by an attachment in any run result
            return baselineLogs.FirstOrDefault().Runs
                .Where(run => run.Results
                    .Any(res => res.Attachments
                        .Any(att => att.FileLocation.Uri != null && string.Equals(att.Description.Text, "toolOutput", StringComparison.OrdinalIgnoreCase)
                        )
                    )
                )
                .SelectMany(run => run.Files.Values)
                .Where(fd => string.Equals(fd.MimeType, "application/a11y", StringComparison.Ordinal)).FirstOrDefault()
                .Contents.Binary;
        }
    }
}
