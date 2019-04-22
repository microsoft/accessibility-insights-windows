// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Axe.Windows.Win32;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.Fakes;

namespace Axe.Windows.Win32Tests
{
    [TestClass]
    public class Win32ApisUnitTests
    {
        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsMissing_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {

                List<string> returnValues = new List<string>();
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsBadFormat_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3 alpha" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsLessThanWindows10_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.2" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsGreaterThanWindows10_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.4" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsTrue(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsMissing_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsBadFormat_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "16227-rc1" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsLessThanTarget_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "16227" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsEqualToTarget_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "16228" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsTrue(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsGreaterThanTarget_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "16229" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsTrue(NativeMethods.IsWindowsRS3OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsMissingReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string>();
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsBadFormat_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3 alpha" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsLessThanWindows10_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.2" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsGreaterThanWindows10_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.4" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsTrue(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(1, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsMissing_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsBadFormat_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "17713-rc1" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsLessThanTarget_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "17712" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsFalse(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsEqualToTarget_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "17713" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsTrue(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsGreaterThanTarget_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "6.3", "17714" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsTrue(NativeMethods.IsWindowsRS5OrLater());
                Assert.AreEqual(2, index);
            }
        }

        [TestMethod]
        public void GetCurrentWindowsVersionForTelemetry_CurrentVersionDoesNotExist_ReturnsNull()
        {
            using (ShimsContext.Create())
            {
                List<string> returnValues = new List<string> { "" };
                int index = 0;
                ShimRegistry.GetValueStringStringObject = (_, __, def) => (++index > returnValues.Count ? def : returnValues[index - 1]);

                Assert.IsNull(NativeMethods.GetCurrentWindowsVersionForTelemetry());
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

                Assert.AreEqual("99.88", NativeMethods.GetCurrentWindowsVersionForTelemetry());
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

                Assert.AreEqual("99.88.77", NativeMethods.GetCurrentWindowsVersionForTelemetry());
                Assert.AreEqual(2, index);
            }
        }
    }
}
