// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Rules.PropertyConditions;
using static Axe.Windows.RulesTest.ControlType;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class ControlViewTest
    {
        [TestMethod]
        public void TestValidControlViewTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = List;
                child.ControlTypeId = ListItem;
                child.IsControlElement = true;
                e.Children.Add(child);

                Assert.IsTrue(ControlView.ListStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoControlElementsInControlViewTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = List;
                child.ControlTypeId = Tree;
                child.IsControlElement = false;
                e.Children.Add(child);

                Assert.IsTrue(ControlView.ListStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoChildrenAllowedControlViewFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = Edit;
                child.ControlTypeId = Text;
                child.IsControlElement = true;
                e.Children.Add(child);

                Assert.IsFalse(ControlView.EditStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestRequiresChildrenControlViewTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = Tab;
                child.ControlTypeId = TabItem;
                child.IsControlElement = true;
                e.Children.Add(child);

                Assert.IsTrue(ControlView.TabStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestRequiresChildrenControlViewFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = Tab;
                child.ControlTypeId = TabItem;
                child.IsControlElement = false;
                e.Children.Add(child);

                Assert.IsFalse(ControlView.TabStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestInvalidControlViewFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = ToolTip;
                child.ControlTypeId = Separator;
                child.IsControlElement = true;
                e.Children.Add(child);

                Assert.IsFalse(ControlView.ToolTipStructure.Matches(e));
            } // using
        }
    }
}
