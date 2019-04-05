// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Fingerprint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.CoreTests.Fingerprint
{
    [TestClass]
    public class FingerprintContributionUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout (2000)]
        public void Ctor_KeyIsTrivial_ThrowsArgumentException()
        {
            try
            {
                new FingerprintContribution(string.Empty, "someValue");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("key", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout (2000)]
        public void Ctor_ValueIsTrivial_ThrowsArgumentException()
        {
            try
            {
                new FingerprintContribution("someKey", string.Empty);
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("value", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Ctor_InputsAreNotTrivial_PropertiesAreCorrect()
        {
            const string key = "SuperCoolKey";
            const string value = "PhenomenalValue";

            FingerprintContribution contribution = new FingerprintContribution(key, value);

            Assert.AreEqual(key, contribution.Key);
            Assert.AreEqual(value, contribution.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void CompareTo_OtherIsNull_ThrowsArgumentNullException()
        {
            FingerprintContribution contribution = new FingerprintContribution("abc", "xyz");
            try
            {
                contribution.CompareTo(null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("other", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void CompareTo_OtherHasDifferentKey_SortsOnKey()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("abc", "MyValue");
            FingerprintContribution contribution2 = new FingerprintContribution("xyz", "MyValue");

            Assert.IsTrue(contribution1.CompareTo(contribution2) < 0);
            Assert.AreEqual(0, contribution1.CompareTo(contribution1));
            Assert.IsTrue(contribution2.CompareTo(contribution1) > 0);
        }

        [TestMethod]
        [Timeout (2000)]
        public void CompareTo_OtherHasSameKey_SortsOnValue()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("MyKey", "abc");
            FingerprintContribution contribution2 = new FingerprintContribution("MyKey", "xyz");

            Assert.IsTrue(contribution1.CompareTo(contribution2) < 0 );
            Assert.AreEqual(0, contribution1.CompareTo(contribution1));
            Assert.IsTrue(contribution2.CompareTo(contribution1) > 0);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherHasDifferentKey_ReturnsFalse()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("abc", "MyValue");
            FingerprintContribution contribution2 = new FingerprintContribution("xyz", "MyValue");

            Assert.AreNotEqual(contribution1, contribution2);
            Assert.AreNotEqual(contribution2, contribution1);
            Assert.AreNotEqual(contribution1, (object)contribution2);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherHasDifferentValue_ReturnsFalse()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("MyKey", "abc");
            FingerprintContribution contribution2 = new FingerprintContribution("MyKey", "xyz");

            Assert.AreNotEqual(contribution1, contribution2);
            Assert.AreNotEqual(contribution2, contribution1);
            Assert.AreNotEqual(contribution1, (object)contribution2);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherIsNull_ReturnsFalse()
        {
            FingerprintContribution contribution = new FingerprintContribution("MyKey", "MyValue");
            FingerprintContribution other = null;

            // Expliicitly call the .Equals method here, since Assert.AreNotEqual
            // includes special handling that bypasses calling the .Equals method
            Assert.IsFalse(contribution.Equals(other));
            Assert.IsFalse(contribution.Equals((object)other));
        }

        [TestMethod]
        [Timeout(2000)]
        public void Equals_OtherHasSameKeyAndValue_ReturnsTrue()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("MyKey", "MyValue");
            FingerprintContribution contribution2 = new FingerprintContribution("MyKey", "MyValue");

            Assert.AreEqual(contribution1, contribution2);
            Assert.AreEqual(contribution2, contribution1);
            Assert.AreEqual(contribution1, (object)contribution2);
        }

        [TestMethod]
        [Timeout (2000)]
        public void GetHashCode_TwoObjectsSameKeySameValue_SameHashReturned()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("MyKey", "MyValue");
            FingerprintContribution contribution2 = new FingerprintContribution("MyKey", "MyValue");

            Assert.AreEqual(contribution1.GetHashCode(), contribution2.GetHashCode());
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCode_TwoObjectsSameKeyDifferentValue_DifferentHashesReturned()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("MyKey", "abc");
            FingerprintContribution contribution2 = new FingerprintContribution("MyKey", "xyz");

            Assert.AreNotEqual(contribution1.GetHashCode(), contribution2.GetHashCode());
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCode_TwoObjectsDifferentKeySameValue_DifferentHashesReturned()
        {
            FingerprintContribution contribution1 = new FingerprintContribution("abc", "MyValue");
            FingerprintContribution contribution2 = new FingerprintContribution("xyz", "MyValue");

            Assert.AreNotEqual(contribution1.GetHashCode(), contribution2.GetHashCode());
        }
    }
}
