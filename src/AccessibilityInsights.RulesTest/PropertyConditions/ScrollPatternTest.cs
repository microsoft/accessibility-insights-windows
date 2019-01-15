// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class ScrollPatternTest
    {
        [TestMethod]
        public void TestScrollPatternNullTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(ScrollPattern.Null.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNullFalse()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                Assert.IsFalse(ScrollPattern.Null.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotNullTrue()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                Assert.IsTrue(ScrollPattern.NotNull.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotNullFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(ScrollPattern.NotNull.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternHorizontallyScrollableTrue()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontalScrollPercentProperty, Value = 0 });
                Assert.IsTrue(ScrollPattern.HorizontallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternHorizontallyScrollableValueFalse()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontallyScrollableProperty, Value = false });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontalScrollPercentProperty, Value = 0 });
                Assert.IsFalse(ScrollPattern.HorizontallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternHorizontallyScrollableInvalidScrollPercentFalse()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontalScrollPercentProperty, Value = (double)0/0 });
                Assert.IsFalse(ScrollPattern.HorizontallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotHorizontallyScrollableValueTrue()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontallyScrollableProperty, Value = false });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontalScrollPercentProperty, Value = 0 });
                Assert.IsTrue(ScrollPattern.NotHorizontallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotHorizontallyScrollableInvalidScrollPercentTrue()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontalScrollPercentProperty, Value = (double)0 / 0 });
                Assert.IsTrue(ScrollPattern.NotHorizontallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotHorizontallyScrollableFalse()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.HorizontalScrollPercentProperty, Value = 0 });
                Assert.IsFalse(ScrollPattern.NotHorizontallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternVerticallyScrollableTrue()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticalScrollPercentProperty, Value = 0 });
                Assert.IsTrue(ScrollPattern.VerticallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternVerticallyScrollableValueFalse()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticallyScrollableProperty, Value = false });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticalScrollPercentProperty, Value = 0 });
                Assert.IsFalse(ScrollPattern.VerticallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternVerticallyScrollableInvalidScrollPercentFalse()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticalScrollPercentProperty, Value = (double)0 / 0 });
                Assert.IsFalse(ScrollPattern.VerticallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotVerticallyScrollableValueTrue()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticallyScrollableProperty, Value = false });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticalScrollPercentProperty, Value = 0 });
                Assert.IsTrue(ScrollPattern.NotVerticallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotVerticallyScrollableInvalidScrollPercentFTrue()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticalScrollPercentProperty, Value = (double)0 / 0 });
                Assert.IsTrue(ScrollPattern.NotVerticallyScrollable.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestScrollPatternNotVerticallyScrollableFalse()
        {
            using (var e = new MockA11yElement())
            {
                var scrollPattern = new A11yPattern(e, PatternType.UIA_ScrollPatternId);
                e.Patterns.Add(scrollPattern);
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticallyScrollableProperty, Value = true });
                scrollPattern.Properties.Add(new A11yPatternProperty() { Name = ScrollPattern.VerticalScrollPercentProperty, Value = 0 });
                Assert.IsFalse(ScrollPattern.NotVerticallyScrollable.Matches(e));
            } // using
        }
    } // class
} // namespace
