// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using AccessibilityInsights.Win32;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Pose;

namespace AccessibilityInsights.Win32Tests
{
    [TestClass]
    public class Win32ApisUnitTests
    {
        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsMissing_ReturnsFalse()
        {
            List<string> returnValues = new List<string>();
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsBadFormat_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.3 alpha" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsLessThanWindows10_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.2" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_OsValueIsGreaterThanWindows10_ReturnsTrue()
        {
            List<string> returnValues = new List<string> { "6.4" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsTrue(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsMissing_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.3" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsBadFormat_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.3", "16227-rc1" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsLessThanTarget_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.3", "16227" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsEqualToTarget_ReturnsTrue()
        {
            List<string> returnValues = new List<string> { "6.3", "16228" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsTrue(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS3OrLater_BuildValueIsGreaterThanTarget_ReturnsTrue()
        {
            List<string> returnValues = new List<string> { "6.3", "16229" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS3OrLater();
            }, registryShim);

            Assert.IsTrue(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsMissingReturnsFalse()
        {
            List<string> returnValues = new List<string>();
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsBadFormat_ReturnsFalse()
        {
            List<string> returnValues = new List<string>() { "6.3 alpha" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsLessThanWindows10_ReturnsFalse()
        {
            List<string> returnValues = new List<string>() { "6.2" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_OsValueIsGreaterThanWindows10_ReturnsTrue()
        {
            List<string> returnValues = new List<string> { "6.4" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsTrue(result);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsMissing_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.3" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsBadFormat_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.3", "17713-rc1" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsLessThanTarget_ReturnsFalse()
        {
            List<string> returnValues = new List<string> { "6.3", "17712" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsFalse(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsEqualToTarget_ReturnsTrue()
        {
            List<string> returnValues = new List<string> { "6.3", "17713" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsTrue(result);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void IsWindowsRS5OrLater_BuildValueIsGreaterThanTarget_ReturnsTrue()
        {
            List<string> returnValues = new List<string> { "6.3", "17714" };
            int index = 0;
            Shim registryShim = Shim.Replace(() => Registry.GetValue(Is.A<string>(), Is.A<string>(), Is.A<object>())).With(
                delegate (string _, string __, object def) { return ++index > returnValues.Count ? def : returnValues[index - 1]; });

            bool result = true;
            PoseContext.Isolate(() =>
            {
                result = NativeMethods.IsWindowsRS5OrLater();
            }, registryShim);

            Assert.IsTrue(result);
            Assert.AreEqual(2, index);
        }
    }
}
