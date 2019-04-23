// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Telemetry;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.Fakes;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.TelemetryTests
{
    [TestClass]
    public class OSHelpersUnitTests
    {

        [TestMethod]
        public void GetCurrentWindowsVersionForTelemetry_CurrentVersionDoesNotExist_ReturnsUnknown()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.AreEqual("unknown", OSHelpers.GetVersion());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void GetCurrentWindowsVersionForTelemetry_CurrentBuildDoesNotExist_ReturnsCurrentVersion()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "99.88", "" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.AreEqual("99.88", OSHelpers.GetVersion());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void GetCurrentWindowsVersionForTelemetry_AllDataExists_ReturnsCurrentVersionDotCurrentBuild()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "99.88", "77" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.AreEqual("99.88.77", OSHelpers.GetVersion());
                Assert.AreEqual(2, index);
            }
        }
    }
}
