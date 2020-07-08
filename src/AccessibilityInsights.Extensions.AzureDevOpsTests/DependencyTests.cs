// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AccessibilityInsights.Extensions.AzureDevOpsTests
{
    [TestClass]
    public class DependencyTests
    {
        private List<string> GetFilesInPath(string path, string pattern)
        {
            List<string> files = new List<string>();

            foreach (string file in Directory.EnumerateFiles(path, pattern))
            {
                files.Add(Path.GetFileName(file));
            }

            return files;
        }

        [TestMethod]
        public void ValidateDependenciesAreCopied()
        {
            // If this test fails, it means that we need to update AzureDevOps.ForcedDependencies
            const string pattern = "*.dll";

            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sourceLocation = testLocation.Replace(@".AzureDevOpsTests\", @".AzureDevOps\");

            List<string> sourceFiles = GetFilesInPath(sourceLocation, pattern);
            List<string> targetFiles = GetFilesInPath(testLocation, pattern);

            foreach (string file in sourceFiles)
            {
                Assert.IsTrue(targetFiles.Contains(file), file + " is missing");
            }
        }
    }
}
