// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Misc;

namespace Axe.Windows.CoreTests.Misc
{
    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void ProcessNameAsExpected()
        {
            var process = Process.GetCurrentProcess();
            if (process == null) throw new ArgumentNullException(nameof(process));

            var name = Utility.GetProcessName(process.Id);

            Assert.AreEqual(process.ProcessName, name);
        }

        [TestMethod]
        public void ProcessNameAsExpected_ReturnsNull_InvalidId()
        {
            var process = Process.GetCurrentProcess();
            if (process == null) throw new ArgumentNullException(nameof(process));

            var name = Utility.GetProcessName(1);

            Assert.IsNull(name);
        }

        [TestMethod]
        public void ProcessNameAsExpected_ReturnsNull_IdIsZero()
        {
            var process = Process.GetCurrentProcess();
            if (process == null) throw new ArgumentNullException(nameof(process));

            var name = Utility.GetProcessName(0);

            Assert.IsNull(name);
        }
    } // class
} // namespace
