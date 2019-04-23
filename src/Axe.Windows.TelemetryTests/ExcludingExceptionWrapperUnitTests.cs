// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.TelemetryTests
{
    [TestClass]
    public class ExcludingExceptionWrapperUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void ExecuteWithExcludedExceptionConversion_NoExceptionThrown_RunsNormally()
        {
            int value = 0;
            Assert.AreEqual(0, value);
            ExcludingExceptionWrapper.ExecuteWithExcludedExceptionConversion(typeof(ArgumentException), () => value++);
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ExcludedException))]
        public void ExecuteWithExcludedExceptionConversion_ExceptionThrownButTypeDoesNotMatch_ExcludesException()
        {
            const string expectedMessage = "blah";

            try
            {
                ExcludingExceptionWrapper.ExecuteWithExcludedExceptionConversion(typeof(ArgumentException), () => throw new ArgumentException(expectedMessage));
            }
            catch (Exception e)
            {
                Assert.AreEqual(expectedMessage, e.Message);
                Assert.AreEqual(expectedMessage, e.InnerException.Message);
                Assert.IsTrue(e.InnerException is ArgumentException);
                ExcludedException castException = e as ExcludedException;
                Assert.AreEqual(typeof(ArgumentException), castException.ExcludedType);
                throw;
            }
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteWithExcludedExceptionConversion_ExceptionThrownAndTypeMatches_ExcludesException()
        {
            const string expectedMessage = "blah";

            ExcludingExceptionWrapper.ExecuteWithExcludedExceptionConversion(typeof(ArgumentException), () => throw new ArgumentNullException(expectedMessage));
        }
    }
}
