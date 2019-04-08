// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class IntPropertiesTests
    {
        [TestMethod]
        public void TestPositionInSetPropertyExistsTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.PositionInSet = 1;
                var p = new IntProperty(PropertyType.UIA_PositionInSetPropertyId);
                Assert.IsTrue(p.Exists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestPositionInSetPropertyExistsFalse()
        {
            using (var e = new MockA11yElement())
            {
                var p = new IntProperty(PropertyType.UIA_PositionInSetPropertyId);
                Assert.IsFalse(p.Exists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestSizeOfSetPropertyExistsTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.SizeOfSet = 1;
                var p = new IntProperty(PropertyType.UIA_SizeOfSetPropertyId);
                Assert.IsTrue(p.Exists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestSizeOfSetPropertyExistsFalse()
        {
            using (var e = new MockA11yElement())
            {
                var p = new IntProperty(PropertyType.UIA_SizeOfSetPropertyId);
                Assert.IsFalse(p.Exists.Matches(e));
            } // using
        }
    }
}
