// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class EnumPropertyUnitTests
    {
        [TestMethod]
        public void TestEnumPropertyEqualsTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.Orientation = OrientationType.Vertical;

                var p = new EnumProperty<OrientationType>(PropertyType.UIA_OrientationPropertyId);
                var condition = p == OrientationType.Vertical;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestEnumPropertyEqualsFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.Orientation = OrientationType.Horizontal;

                var p = new EnumProperty<OrientationType>(PropertyType.UIA_OrientationPropertyId);
                var condition = p == OrientationType.Vertical;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [Timeout(2000)]
        public void Equals_OtherIsSameType_ThrowsNotSupportedException()
        {
            var p = new EnumProperty<OrientationType>(PropertyType.UIA_OrientationPropertyId);
            p.Equals(p);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [Timeout(2000)]
        public void Equals_OtherIsDifferentType_ThrowsNotSupportedException()
        {
            var p = new EnumProperty<OrientationType>(PropertyType.UIA_OrientationPropertyId);
            p.Equals("abc");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [Timeout(2000)]
        public void GetHashCode_ThrowsNotSupportedException()
        {
            var p = new EnumProperty<OrientationType>(PropertyType.UIA_OrientationPropertyId);
            p.GetHashCode();
        }
    }
}
