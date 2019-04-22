// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.TelemetryTests
{
    [TestClass]
    public class ExceptionExcluderUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void ExcludeThrownExceptions_NoExceptionThrown_RunsNormally()
        {
            int value = 0;
            Assert.AreEqual(0, value);
            ExceptionExcluder.ExcludeThrownExceptions(() => value++);
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ExcludedException))]
        public void ExcludeThrownExceptions_ExceptionThrown_ExcludesException()
        {
            const string expectedMessage = "blah";

            try
            {
                ExceptionExcluder.ExcludeThrownExceptions(() => throw new ArgumentException(expectedMessage));
            }
            catch (Exception e)
            {
                Assert.AreEqual("Excluded Exception", e.Message);
                Assert.AreEqual(expectedMessage, e.InnerException.Message);
                Assert.IsTrue(e.InnerException is ArgumentException);
                throw;
            }
        }
    }
}
