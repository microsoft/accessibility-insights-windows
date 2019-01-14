// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccessibilityInsights.CoreTests.Misc
{
    [TestClass]
    public class PreconditionsUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void IsNotNull_ValueIsNull_ThrowsCorrectException()
        {
            object someVariable = null;

            try
            {
                someVariable.ArgumentIsNotNull(nameof(someVariable));
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("someVariable", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void IsNotNull_ValueIsNotNoll_DoesNotThrow()
        {
            object someVariable = new object();
            someVariable.ArgumentIsNotNull(nameof(someVariable));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout (2000)]
        public void IsNotTrivialString_IsTrivial_ThrowsCorrectException()
        {
            string someString = "";
            try
            {
                someString.ArgumentIsNotTrivialString(nameof(someString));
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("someString", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void IsNotTrivialString_IsNotTrivial_DoesNotThrow()
        {
            string someString = "hello";
            someString.ArgumentIsNotTrivialString(nameof(someString));
        }
    }
}
