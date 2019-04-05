// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using Condition = Axe.Windows.Rules.Condition;

namespace Axe.Windows.RulesTest.Conditions
{
    [TestClass]
    public class TreeDescentConditionTest
    {
        [TestMethod]
        public void TestDescendTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = ControlType.Hyperlink;
                child.ControlTypeId = ControlType.Text;
                e.Children.Add(child);

                var condition = Hyperlink / Text;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendWithChildFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = ControlType.Hyperlink;
                child.ControlTypeId = ControlType.Text;
                e.Children.Add(child);

                var condition = Hyperlink / Button;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendWithParentFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = ControlType.Hyperlink;
                child.ControlTypeId = ControlType.Text;
                e.Children.Add(child);

                var condition = Button / Text;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendWithNullElementsTrue()
        {
            var condition = Condition.True / Condition.True;
            Assert.IsTrue(condition.Matches(null));
        }

        [TestMethod]
        public void TestDescendWithParentNullFalse()
        {
            var condition = Condition.False / Condition.True;
            Assert.IsFalse(condition.Matches(null));
        }

        [TestMethod]
        public void TestDescendWithChildNullFalse()
        {
            var condition = Condition.True / Condition.False;
            Assert.IsFalse(condition.Matches(null));
        }
    } // class
} // namespace
