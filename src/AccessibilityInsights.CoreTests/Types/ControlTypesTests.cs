// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Axe.Windows.Core.Types.Tests
{
    [TestClass()]
    public class ControlTypesTest
    {
        [TestMethod()]
        public void CheckExists()
        {
            Assert.AreEqual(true, ControlType.GetInstance().Exists(ControlType.UIA_AppBarControlTypeId));
            Assert.AreEqual(false, ControlType.GetInstance().Exists(0));
        }

        [TestMethod()]
        public void CheckGetNameById()
        {
            Assert.AreEqual("AppBar(50040)", ControlType.GetInstance().GetNameById(ControlType.UIA_AppBarControlTypeId));
        }

        [TestMethod()]
        [Timeout(1000)]
        public void CheckHasExpectedValues()
        {
            var values = ControlType.GetInstance().Values;
            var minValue = ControlType.UIA_ButtonControlTypeId;
            var maxValue = ControlType.UIA_AppBarControlTypeId;

            // Adding 1 because the true count of objects is max + 1 - min.
            Assert.AreEqual(values.Count(), maxValue + 1 - minValue);

            for (int i = minValue; i <= maxValue; ++i)
                Assert.IsTrue(values.Contains(i), $"{nameof(ControlType)} does not contain the expected value {i}");
        }
    }
}
