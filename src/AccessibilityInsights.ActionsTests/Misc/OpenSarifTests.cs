// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.ActionsTests.Misc
{
    [TestClass]
    public class OpenSarifTests
    {
        /// <summary>
        /// Load known file & validate parsing / retreival of binary is correct
        /// Explicitly deserializing known JSON file to validate Sarif SDK shape
        /// </summary>
        [TestMethod()]
        public void ExtractA11yFile_StoredFile_CorrectBinary()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "sample.a11ytest");
            var binary = AccessibilityInsights.Actions.Sarif.OpenSarif.ExtractA11yTestFile(path);
            Assert.AreEqual(binary, "test-a11y-binary");
        }
    }
}
