// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Exceptions;
using Axe.Windows.UnitTestSharedLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Axe.Windows.CoreTests.Bases
{
    /// <summary>
    /// Tests A11yPattern class
    /// </summary>
    [TestClass()]
    public class A11yPatternTests
    {
        /// <summary>
        /// Test ToString and constructor for A11yPattern
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            A11yElement ke = Utility.LoadA11yElementsFromJSON("Resources/A11yPatternTest.hier");
       
            Assert.AreEqual("SelectionPattern: False", ke.Patterns[0].ToString());
            Assert.AreEqual("ScrollPattern: False", ke.Patterns[1].ToString());
            Assert.AreEqual("ExpandCollapsePattern: 0", ke.Patterns[2].ToString());
            Assert.AreEqual("ItemContainerPattern: ", ke.Patterns[3].ToString());
            Assert.AreEqual("SynchronizedInputPattern: ", ke.Patterns[4].ToString());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ExcludeExceptionsFromTelemetry_NoExceptionThrown_RunsNormally()
        {
            int value = 0;
            Assert.AreEqual(0, value);
            A11yPattern.ExcludeExceptionsFromTelemetry(() => value++);
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(TelemetryExcludedException))]
        public void ExcludeExceptionsFromTelemetry_ExceptionThrown_WrapsException()
        {
            const string expectedMessage = "blah";

            try
            {
                A11yPattern.ExcludeExceptionsFromTelemetry(() => throw new ArgumentException(expectedMessage));
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
