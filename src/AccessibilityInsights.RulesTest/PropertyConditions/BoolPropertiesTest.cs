// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using static AccessibilityInsights.Rules.PropertyConditions.BoolProperties;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class BoolPropertiesTest
    {
        [TestMethod]
        public void TestIsEnabledTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.IsEnabled = true;
                Assert.IsTrue(IsEnabled.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsEnabledFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsEnabled.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotEnabledTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsNotEnabled.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotEnabledFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.IsEnabled = true;
                Assert.IsFalse(IsNotEnabled.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsKeyboardFocusableTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.IsKeyboardFocusable = true;
                Assert.IsTrue(IsKeyboardFocusable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsKeyboardFocusableFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsKeyboardFocusable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotKeyboardFocusableTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsNotKeyboardFocusable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotKeyboardFocusableFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.IsKeyboardFocusable = true;
                Assert.IsFalse(IsNotKeyboardFocusable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsOffScreenTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.IsOffScreen = true;
                Assert.IsTrue(IsOffScreen.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsOffScreenFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsOffScreen.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotOffScreenTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsNotOffScreen.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotOffScreenFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.IsOffScreen = true;
                Assert.IsFalse(IsNotOffScreen.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsDesktopTrue()
        {
            using (var e = new MockA11yElement())
            {
                var property = new A11yProperty(PropertyType.UIA_RuntimeIdPropertyId, new int[] { 0x2A, 0x10010 });
                e.Properties.Add(property.Id, property);
                Assert.IsTrue(IsDesktop.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsDesktopFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsDesktop.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotDesktopTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsNotDesktop.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotDesktopFalse()
        {
            using (var e = new MockA11yElement())
            {
                var property = new A11yProperty(PropertyType.UIA_RuntimeIdPropertyId, new int[] { 0x2A, 0x10010 });
                e.Properties.Add(property.Id, property);
                Assert.IsFalse(IsNotDesktop.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsContentElementTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.IsContentElement = true;
                Assert.IsTrue(IsContentElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsContentElementFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsContentElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotContentElementTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsNotContentElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotContentElementFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.IsContentElement = true;
                Assert.IsFalse(IsNotContentElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsControlElementElementTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.IsControlElement = true;
                Assert.IsTrue(IsControlElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsControlElementFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsControlElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotControlElementTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsNotControlElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsNotControlElementFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.IsControlElement = true;
                Assert.IsFalse(IsNotControlElement.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsContentElementExistsTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.IsContentElement = false;
                Assert.IsTrue(IsContentElementExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsContentElementExistsFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsContentElementExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsContentElementDoesNotExistTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsContentElementDoesNotExist.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsContentElementDoesNotExistFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.IsContentElement = false;
                Assert.IsFalse(IsContentElementDoesNotExist.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsControlElementExistsTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.IsControlElement = false;
                Assert.IsTrue(IsControlElementExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsControlElementExistsFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(IsControlElementExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsControlElementDoesNotExistTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(IsControlElementDoesNotExist.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestIsControlElementDoesNotExistFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.IsControlElement = false;
                Assert.IsFalse(IsControlElementDoesNotExist.Matches(e));
            } // using
        }
    } // class
} // namespace
