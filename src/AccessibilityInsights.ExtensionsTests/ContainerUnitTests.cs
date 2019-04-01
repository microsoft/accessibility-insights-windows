// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if FAKES_SUPPORTED
using Microsoft.QualityTools.Testing.Fakes;
using System.IO.Fakes;
#endif

namespace AccessibilityInsights.ExtensionsTests
{
    [TestClass]
    public class ContainerUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void GetExtensionPaths_FindsThreeFolders_ReturnsCorrectValues()
        {
            using (ShimsContext.Create())
            {
                List<string> directories = new List<string> { "a", "b", "c" };
                ShimDirectory.EnumerateDirectoriesString = (_) => directories;
                ShimDirectory.ExistsString = (_) => true;
                List<string> outputDirs = Container.GetExtensionPaths().ToList();
                string currentDir = Path.GetDirectoryName(typeof(Container).Assembly.Location);

                Assert.AreEqual(directories.Count, outputDirs.Count);
                for (int loop = 0; loop < directories.Count; loop++)
                {
                    Assert.AreEqual(Path.Combine(currentDir, directories[loop]), Path.Combine(currentDir, outputDirs[loop]));
                }
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetExtensionPaths_NoFolders_ReturnsCorrectValues()
        {
            using (ShimsContext.Create())
            {
                List<string> directories = new List<string> { };
                ShimDirectory.EnumerateDirectoriesString = (_) => directories;
                List<string> outputDirs = Container.GetExtensionPaths().ToList();
                string currentDir = Path.GetDirectoryName(typeof(Container).Assembly.Location);
                Assert.AreEqual(directories.Count, outputDirs.Count);
            }
        }
    }
}
