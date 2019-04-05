// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class IntPropertyTest
    {
        [TestMethod]
        public void TestIntPropertyExistsTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.Orientation = OrientationType.None;

                var p = new IntProperty(PropertyType.UIA_OrientationPropertyId);
                Assert.IsTrue(p.Exists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIntPropertyExistsFalse()
        {
            using (var e = new MockA11yElement())
            {
                var p = new IntProperty(PropertyType.UIA_OrientationPropertyId);
                Assert.IsFalse(p.Exists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIntPropertyEqualsTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel1;

                var p = new IntProperty(PropertyType.UIA_HeadingLevelPropertyId);
                var condition = p == HeadingLevelType.HeadingLevel1;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIntPropertyEqualsFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel1;

                var p = new IntProperty(PropertyType.UIA_HeadingLevelPropertyId);
                var condition = p == HeadingLevelType.HeadingLevel2;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }
    } // class
} // namespace
