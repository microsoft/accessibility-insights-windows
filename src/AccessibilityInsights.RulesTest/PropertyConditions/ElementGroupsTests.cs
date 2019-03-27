// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Rules.PropertyConditions;
using static AccessibilityInsights.RulesTest.ControlType;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class ElementGroupsTests
    {
        private readonly int[] AllowSameNameAndControlTypeTypes = null;
        private readonly IEnumerable<int> DisallowSameNameAndControlTypeTypes = null;

        public ElementGroupsTests()
        {
            this.AllowSameNameAndControlTypeTypes = new int[]{ AppBar, Custom, Header, MenuBar, SemanticZoom, StatusBar, TitleBar, Text };
            this.DisallowSameNameAndControlTypeTypes = ControlType.All.Difference(AllowSameNameAndControlTypeTypes);
        }

        [TestMethod]
        public void AllowSameNameAndControlType_True_BasedOnType()
        {
            var e = new MockA11yElement();

            foreach (var t in this.AllowSameNameAndControlTypeTypes)
            {
                e.ControlTypeId = t;
                Assert.IsTrue(ElementGroups.AllowSameNameAndControlType.Matches(e));
            } // for each type
        }

        [TestMethod]
        public void AllowSameNameAndControlType_False_ContainsDisallowedType()
        {
            var e = new MockA11yElement();

            foreach (var t in this.DisallowSameNameAndControlTypeTypes)
            {
                e.ControlTypeId = t;
                Assert.IsFalse(ElementGroups.AllowSameNameAndControlType.Matches(e));
            } // for each type
        }

        [TestMethod]
        public void AllowSameNameAndControlType_True_ExceedsMaxLength()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = this.DisallowSameNameAndControlTypeTypes.First();

            var tenChars = "1234567890";
            for (int i = 0; i < 5; ++i)
                e.Name += tenChars;

            Assert.IsFalse(ElementGroups.AllowSameNameAndControlType.Matches(e));

            e.Name += "1";

            Assert.IsTrue(ElementGroups.AllowSameNameAndControlType.Matches(e));
        }

        [TestMethod]
        public void WPFScrollbarPageUpButton_True()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                parent.ControlTypeId = ControlType.ScrollBar;
                e.IsOffScreen = false;
                e.ControlTypeId = ControlType.Button;
                e.Framework = "WPF";
                e.AutomationId = "PageUp";
                parent.Children.Add(e);
                e.Parent = parent;

                Assert.IsTrue(ElementGroups.WPFScrollBarPageUpOrPageDownButtons.Matches(e));
            } // using
        }

        [TestMethod]
        public void WPFScrollbarPageDownButton_True()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                parent.ControlTypeId = ControlType.ScrollBar;
                e.ControlTypeId = ControlType.Button;
                e.Framework = "WPF";
                e.AutomationId = "PageDown";
                parent.Children.Add(e);
                e.Parent = parent;

                Assert.IsTrue(ElementGroups.WPFScrollBarPageUpOrPageDownButtons.Matches(e));
            } // using
        }

        [TestMethod]
        public void WPFScrollbarPageUpOrPageDownButtons_NotWPF_False()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                parent.ControlTypeId = ControlType.ScrollBar;
                e.ControlTypeId = ControlType.Button;
                // e.Framework = "WPF";
                e.AutomationId = "PageUp";
                parent.Children.Add(e);
                e.Parent = parent;

                Assert.IsFalse(ElementGroups.WPFScrollBarPageUpOrPageDownButtons.Matches(e));
            } // using
        }

        [TestMethod]
        public void WPFScrollbarPageUpOrPageDownButtons_NotButton_False()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                parent.ControlTypeId = ControlType.ScrollBar;
                // e.ControlTypeId = ControlType.Button;
                e.Framework = "WPF";
                e.AutomationId = "PageUp";
                parent.Children.Add(e);
                e.Parent = parent;

                Assert.IsFalse(ElementGroups.WPFScrollBarPageUpOrPageDownButtons.Matches(e));
            } // using
        }

        [TestMethod]
        public void WPFScrollbarPageUpOrPageDownButtons_ParentNotScrollbar_False()
        {
            // This test essentially also covers the possibility of no parent.

            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                // parent.ControlTypeId = ControlType.ScrollBar;
                e.ControlTypeId = ControlType.Button;
                e.Framework = "WPF";
                e.AutomationId = "PageUp";
                parent.Children.Add(e);
                e.Parent = parent;

                Assert.IsFalse(ElementGroups.WPFScrollBarPageUpOrPageDownButtons.Matches(e));
            } // using
        }

        [TestMethod]
        public void WPFScrollbarPageUpOrPageDownButtons_NotPageUpOrPageDown_False()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                parent.ControlTypeId = ControlType.ScrollBar;
                e.ControlTypeId = ControlType.Button;
                e.Framework = "WPF";
                // e.AutomationId = "PageUp";
                parent.Children.Add(e);
                e.Parent = parent;

                Assert.IsFalse(ElementGroups.WPFScrollBarPageUpOrPageDownButtons.Matches(e));
            } // using
        }
    } // class
} // namespace
