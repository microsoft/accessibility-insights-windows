// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Types;
using BoundingRectangle = AccessibilityInsights.Rules.PropertyConditions.BoundingRectangle;
using AccessibilityInsights.Core.Bases;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class BoundingRectangleTest
    {
        [TestMethod]
        public void TestBoundingRectangleNullTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(BoundingRectangle.Null.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNullFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty; ;
                Assert.IsFalse(BoundingRectangle.Null.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotNullTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty; ;
                Assert.IsTrue(BoundingRectangle.NotNull.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotNullFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(BoundingRectangle.NotNull.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleEmptyTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty;
                Assert.IsTrue(BoundingRectangle.Empty.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleEmptyFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 1, 1);
                Assert.IsFalse(BoundingRectangle.Empty.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotEmptyTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 1, 1);
                Assert.IsTrue(BoundingRectangle.NotEmpty.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotEmptyFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty;
                Assert.IsFalse(BoundingRectangle.NotEmpty.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleValidTrue()
        {
            Rectangle rect = new Rectangle(0, 0, 5, 5);

            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = rect;
                Assert.IsTrue(BoundingRectangle.Valid.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleEmptyValidFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty;
                Assert.IsFalse(BoundingRectangle.Valid.Matches(e));
            } // using
        }

        public void TestBoundingRectangleNotMinWidthValidFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 2, 13);
                Assert.IsFalse(BoundingRectangle.Valid.Matches(e));
            } // using
        }

        public void TestBoundingRectangleNotMinHeightValidFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 13, 2);
                Assert.IsFalse(BoundingRectangle.Valid.Matches(e));
            } // using
        }

        public void TestBoundingRectangleNotMinAreaValidFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 4, 5);
                Assert.IsFalse(BoundingRectangle.Valid.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleEmptyNotValidTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty;
                Assert.IsTrue(BoundingRectangle.NotValid.Matches(e));
            } // using
        }

        public void TestBoundingRectangleNotMinWidthNotValidTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 2, 13);
                Assert.IsTrue(BoundingRectangle.NotValid.Matches(e));
            } // using
        }

        public void TestBoundingRectangleNotMinHeightNotValidTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 13, 2);
                Assert.IsTrue(BoundingRectangle.NotValid.Matches(e));
            } // using
        }

        public void TestBoundingRectangleNotMinAreaNotValidTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 4, 5);
                Assert.IsTrue(BoundingRectangle.NotValid.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotValidFalse()
        {
            Rectangle rect = new Rectangle(0, 0, 5, 5);

            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = rect;
                Assert.IsFalse(BoundingRectangle.NotValid.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleCorrectDataFormatTrue()
        {
            using (var e = new MockA11yElement())
            {
                var property = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, new double[] { 1, 2, 3, 4 });
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, property);
                Assert.IsTrue(BoundingRectangle.CorrectDataFormat.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleCorrectDataFormatLengthFalse()
        {
            using (var e = new MockA11yElement())
            {
                var property = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, new double[] { 1, 2, 3 });
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, property);
                Assert.IsFalse(BoundingRectangle.CorrectDataFormat.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleCorrectDataFormatTypeFalse()
        {
            using (var e = new MockA11yElement())
            {
                var property = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, true);
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, property);
                Assert.IsFalse(BoundingRectangle.CorrectDataFormat.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotCorrectDataFormatLengthTrue()
        {
            using (var e = new MockA11yElement())
            {
                var property = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, new double[] { 1, 2, 3 });
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, property);
                Assert.IsTrue(BoundingRectangle.NotCorrectDataFormat.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotCorrectDataFormatTypeTrue()
        {
            using (var e = new MockA11yElement())
            {
                var property = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, true);
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, property);
                Assert.IsTrue(BoundingRectangle.NotCorrectDataFormat.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotCorrectDataFormatFalse()
        {
            using (var e = new MockA11yElement())
            {
                var proeprty = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, new double[] { 1, 2, 3, 4 });
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, proeprty);
                Assert.IsFalse(BoundingRectangle.NotCorrectDataFormat.Matches(e));
            } // using
        }
    } // class
} // namespace
