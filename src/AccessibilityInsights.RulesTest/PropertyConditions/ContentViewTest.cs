// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Rules.PropertyConditions;
using static Axe.Windows.RulesTest.ControlType;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class ContentViewTest
    {
        [TestMethod]
        public void TestValidContentViewTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = TreeItem;
                child.ControlTypeId = TreeItem;
                child.IsContentElement = true;
                e.Children.Add(child);

                Assert.IsTrue(ContentView.TreeItemStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoContentElementsInContentViewTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = TreeItem;
                child.ControlTypeId = Separator;
                child.IsContentElement = false;
                e.Children.Add(child);

                Assert.IsTrue(ContentView.TreeItemStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoChildrenAllowedContentViewFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = Edit;
                child.ControlTypeId = Text;
                child.IsContentElement = true;
                e.Children.Add(child);

                Assert.IsFalse(ContentView.EditStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestRequiresChildrenContentViewTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = Menu;
                child.ControlTypeId = MenuItem;
                child.IsContentElement = true;
                e.Children.Add(child);

                Assert.IsTrue(ContentView.MenuStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestRequiresChildrenContentViewFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = Menu;
                child.ControlTypeId = MenuItem;
                child.IsContentElement = false;
                e.Children.Add(child);

                Assert.IsFalse(ContentView.MenuStructure.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestInvalidContentViewFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                e.ControlTypeId = TreeItem;
                child.ControlTypeId = Separator;
                child.IsContentElement = true;
                e.Children.Add(child);

                Assert.IsFalse(ContentView.TreeItemStructure.Matches(e));
            } // using
        }
    }
}
