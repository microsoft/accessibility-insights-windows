// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Axe.Windows.CoreTests.Misc
{
    [TestClass]
    public class BoundedCounterUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_UpperBoundIsTooSmall_ThrowsArgumentException()
        {
            new BoundedCounter(0);
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_UpperBoundIsTooLarge_ThrowsArgumentException()
        {
            new BoundedCounter(int.MaxValue);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_UpperBoundIsSmallestValidValue_InitialValuesAreCorrect()
        {
            const int upperBound = 1;
            BoundedCounter counter = new BoundedCounter(upperBound);

            Assert.AreEqual(0, counter.Attempts);
            Assert.AreEqual(0, counter.Count);
            Assert.AreEqual(upperBound, counter.UpperBound);
            Assert.IsFalse(counter.UpperBoundExceeded);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_UpperBoundIsLargestValidValue_InitialValuesAreCorrect()
        {
            const int upperBound = int.MaxValue - 1;
            BoundedCounter counter = new BoundedCounter(upperBound);

            Assert.AreEqual(0, counter.Attempts);
            Assert.AreEqual(0, counter.Count);
            Assert.AreEqual(upperBound, counter.UpperBound);
            Assert.IsFalse(counter.UpperBoundExceeded);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryIncrement_UpperBoundNotExceeded_ReturnsTrue_UpdatesState()
        {
            const int upperBound = 5;
            BoundedCounter counter = new BoundedCounter(upperBound);

            for (int loop = 0; loop < upperBound; loop++)
            {
                Assert.AreEqual(upperBound, counter.UpperBound);
                Assert.AreEqual(loop, counter.Attempts);
                Assert.AreEqual(loop, counter.Count);
                Assert.IsFalse(counter.UpperBoundExceeded);
                Assert.IsTrue(counter.TryIncrement());
            }

            Assert.IsFalse(counter.UpperBoundExceeded);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryIncrement_UpperBoundExceeded_ReturnsFalse_UpdatesState()
        {
            const int upperBound = 5;
            BoundedCounter counter = new BoundedCounter(upperBound);

            Assert.IsTrue(counter.TryAdd(upperBound));
            Assert.IsFalse(counter.UpperBoundExceeded);

            // UpperBound has been reached, so all further adds should fail
            for (int loop = 0; loop < upperBound; loop++)
            {
                Assert.AreEqual(upperBound, counter.UpperBound);
                Assert.AreEqual(upperBound, counter.Count);
                Assert.AreEqual(upperBound + loop, counter.Attempts);
                Assert.IsFalse(counter.TryIncrement());
                Assert.IsTrue(counter.UpperBoundExceeded);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryIncrement_Overflows_ReturnsFalse_Updates_State()
        {
            const int upperBound = int.MaxValue - 1;
            BoundedCounter counter = new BoundedCounter(upperBound);
            Assert.IsTrue(counter.TryAdd(upperBound));

            for (int loop = 0; loop < 5; loop++)
            {
                Assert.IsFalse(counter.TryIncrement());
                Assert.AreEqual(int.MaxValue, counter.Attempts);
                Assert.AreEqual(upperBound, counter.Count);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentException))]
        public void TryAdd_ValueToAddIsNegative_ThrowsArgumentException()
        {
            const int upperBound = 5;

            BoundedCounter counter = new BoundedCounter(upperBound);
            counter.TryAdd(-1);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryAdd_ValueToAddIsZero_UpperBoundIsNotExceeded_ReturnsTrue()
        {
            const int upperBound = 5;

            BoundedCounter counter = new BoundedCounter(upperBound);
            Assert.IsTrue(counter.TryAdd(0));
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryAdd_ValueToAddIsZero_UpperBoundIsExceeded_ReturnsTrue()
        {
            const int upperBound = 5;

            BoundedCounter counter = new BoundedCounter(upperBound);
            Assert.IsFalse(counter.TryAdd(upperBound + 1));  // Exceed upper bound
            Assert.IsFalse(counter.TryAdd(0));
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryAdd_UpperBoundNotExceeded_ReturnsTrue_UpdatesState()
        {
            const int upperBound = 5;
            const int valueToAdd = 3;

            BoundedCounter counter = new BoundedCounter(upperBound);

            Assert.IsTrue(counter.TryAdd(valueToAdd));
            Assert.AreEqual(valueToAdd, counter.Attempts);
            Assert.AreEqual(valueToAdd, counter.Count);
            Assert.AreEqual(upperBound, counter.UpperBound);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryAdd_UpperBoundExceeded_ReturnsFalse_UpdatesState()
        {
            const int upperBound = 5;
            const int valueToAdd = 3;

            BoundedCounter counter = new BoundedCounter(upperBound);
            counter.TryAdd(valueToAdd);

            for (int loop = 0; loop < upperBound; loop++)
            {
                Assert.IsFalse(counter.TryAdd(valueToAdd));
                Assert.AreEqual(valueToAdd * (loop + 2), counter.Attempts);
                Assert.AreEqual(upperBound, counter.Count);
                Assert.AreEqual(upperBound, counter.UpperBound);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void TryAdd_Overflows_ReturnsFalse_Updates_State()
        {
            const int upperBound = int.MaxValue - 5;
            const int valueToAdd = 10;

            BoundedCounter counter = new BoundedCounter(upperBound);
            Assert.IsTrue(counter.TryAdd(upperBound - 2));

            for (int loop = 0; loop < 5; loop++)
            {
                Assert.IsFalse(counter.TryAdd(valueToAdd));
                Assert.AreEqual(int.MaxValue, counter.Attempts);
                Assert.AreEqual(upperBound, counter.Count);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void Reset_UpperBoundNotExceeded_SetsCorrectValues()
        {
            const int upperBound = 2;
            BoundedCounter counter = new BoundedCounter(upperBound);
            counter.TryIncrement();

            counter.Reset();
            Assert.AreEqual(0, counter.Attempts);
            Assert.AreEqual(0, counter.Count);
            Assert.AreEqual(upperBound, counter.UpperBound);
            Assert.IsFalse(counter.UpperBoundExceeded);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Reset_UpperBoundExceeded_SetsCorrectValues()
        {
            const int upperBound = 1;
            BoundedCounter counter = new BoundedCounter(upperBound);
            counter.TryIncrement();

            counter.Reset();
            Assert.AreEqual(0, counter.Attempts);
            Assert.AreEqual(0, counter.Count);
            Assert.AreEqual(upperBound, counter.UpperBound);
            Assert.IsFalse(counter.UpperBoundExceeded);
        }
    }
}
