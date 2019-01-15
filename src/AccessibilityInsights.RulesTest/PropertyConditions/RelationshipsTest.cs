// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Rules;
using AccessibilityInsights.Rules.PropertyConditions;
using static AccessibilityInsights.Rules.PropertyConditions.Relationships;
using static AccessibilityInsights.RulesTest.ControlType;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class RelationshipsTest
    {
        [TestMethod]
        public void TestSibblingsWithSameTypeTrue()
        {
            using (var e = new MockA11yElement())
            using (var sibbling = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.ControlTypeId = Hyperlink;
                sibbling.ControlTypeId = Hyperlink;
                e.Parent = parent;
                sibbling.Parent = parent;
                parent.Children.Add(e);
                parent.Children.Add(sibbling);

                Assert.IsTrue(SiblingsOfSameType.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestSibblingsWithSameTypeFalse()
        {
            using (var e = new MockA11yElement())
            using (var sibbling = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.ControlTypeId = Hyperlink;
                sibbling.ControlTypeId = ComboBox;
                e.Parent = parent;
                sibbling.Parent = parent;
                parent.Children.Add(e);
                parent.Children.Add(sibbling);

                Assert.IsFalse(SiblingsOfSameType.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoSibblingsWithSameTypeTrue()
        {
            using (var e = new MockA11yElement())
            using (var sibbling = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.ControlTypeId = Hyperlink;
                sibbling.ControlTypeId = ComboBox;
                e.Parent = parent;
                sibbling.Parent = parent;
                parent.Children.Add(e);
                parent.Children.Add(sibbling);

                Assert.IsTrue(NoSiblingsOfSameType.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoSibblingsWithSameTypeFalse()
        {
            using (var e = new MockA11yElement())
            using (var sibbling = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.ControlTypeId = Hyperlink;
                sibbling.ControlTypeId = Hyperlink;
                e.Parent = parent;
                sibbling.Parent = parent;
                parent.Children.Add(e);
                parent.Children.Add(sibbling);

                Assert.IsFalse(NoSiblingsOfSameType.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestParentExistsTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;
                Assert.IsTrue(ParentExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestParentExistsFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(ParentExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoParentExistsTrue()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsTrue(NoParentExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoParentExistsFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;
                Assert.IsFalse(NoParentExists.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestParentTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.ControlTypeId = Button;

                var condition = Parent(AccessibilityInsights.Rules.PropertyConditions.ControlType.Button);

                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestParentFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.ControlTypeId = Button;

                var condition = Parent(AccessibilityInsights.Rules.PropertyConditions.ControlType.CheckBox);

                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNullParentFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = Parent(Condition.True);

                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNotParentTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;

                var condition = NotParent(Condition.False);

                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNotParentFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;

                var condition = NotParent(Condition.True);

                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNullNotParentTrue()
        {
            using (var e = new MockA11yElement())
            {
                var condition = NotParent(Condition.True);

                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAncestorTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;

                var condition = Ancestor(2, Condition.True);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAncestorFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;

                var condition = Ancestor(2, Condition.True);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAncestorExistsTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;

                var condition = AncestorExists(2);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        public void TestAncestorExistsFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.Parent = parent;

                var condition = Ancestor(2, Condition.True);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyAncestorTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;
                grandparent.ControlTypeId = ControlType.Tree;

                var condition = AnyAncestor(AccessibilityInsights.Rules.PropertyConditions.ControlType.Tree);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyAncestorFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;
                grandparent.ControlTypeId = Pane;

                var condition = AnyAncestor(AccessibilityInsights.Rules.PropertyConditions.ControlType.Tree);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyAncestorNullFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = AnyAncestor(AccessibilityInsights.Rules.Condition.True);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyAncestorStopConditionTrue()
        {
            var e = new MockA11yElement();
            var parent = new MockA11yElement();
            e.Parent = parent;

            var condition = AnyAncestor(AccessibilityInsights.Rules.Condition.True, AccessibilityInsights.Rules.Condition.True);
            Assert.IsFalse(condition.Matches(e));
        }

        [TestMethod]
        public void TestAnyAncestorStopConditionFalse()
        {
            var e = new MockA11yElement();
            var parent = new MockA11yElement();
            e.Parent = parent;

            // as long as the parent exists, the first condition below should evaluate to true
            var condition = AnyAncestor(AccessibilityInsights.Rules.Condition.True, AccessibilityInsights.Rules.Condition.False);
            Assert.IsTrue(condition.Matches(e));
        }

        [TestMethod]
        public void TestNoAncestorTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;
                grandparent.ControlTypeId = Pane;

                var condition = NoAncestor(AccessibilityInsights.Rules.PropertyConditions.ControlType.Tree);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoAncestorFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;
                grandparent.ControlTypeId = ControlType.Tree;

                var condition = NoAncestor(AccessibilityInsights.Rules.PropertyConditions.ControlType.Tree);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoAncestorNullFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = NoAncestor(AccessibilityInsights.Rules.Condition.False);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllAncestorsTrue()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;
                e.ControlTypeId = TreeItem;
                parent.ControlTypeId = TreeItem;
                grandparent.ControlTypeId = TreeItem;

                var condition = AllAncestors(AccessibilityInsights.Rules.PropertyConditions.ControlType.TreeItem);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllAncestorsFalse()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            using (var grandparent = new MockA11yElement())
            {
                e.Parent = parent;
                parent.Parent = grandparent;
                e.ControlTypeId = TreeItem;
                parent.ControlTypeId = TreeItem;
                grandparent.ControlTypeId = Pane;

                var condition = AllAncestors(AccessibilityInsights.Rules.PropertyConditions.ControlType.TreeItem);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyChildTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                    e.Children.Add(new MockA11yElement());

                var child = e.Children[3] as MockA11yElement;
                child.ControlTypeId = ComboBox;

                var condition = AnyChild(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyChildFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                    e.Children.Add(new MockA11yElement());

                var condition = AnyChild(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyChildNoChildrenFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = AnyChild(Condition.True);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoChildTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);
                }

                var condition = NoChild(AccessibilityInsights.Rules.PropertyConditions.ControlType.Button);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoChildFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);
                }

                var c4 = e.Children[3] as MockA11yElement;
                c4.ControlTypeId = DataGrid;

                var condition = NoChild(AccessibilityInsights.Rules.PropertyConditions.ControlType.DataGrid);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllChildrenTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);
                }

                var condition = AllChildren(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllChildrenFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);
                }

                var c4 = e.Children[3] as MockA11yElement;
                c4.ControlTypeId = DataGrid;

                var condition = AllChildren(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllChildrenNoChildrenFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = AllChildren(Condition.True);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildrenExistTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.Children.Add(new MockA11yElement());
                var condition = ChildrenExist;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildrenExistFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = ChildrenExist;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoChildrenExistTrue()
        {
            using (var e = new MockA11yElement())
            {
                var condition = NoChildrenExist;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoChildrenExistFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.Children.Add(new MockA11yElement());
                var condition = NoChildrenExist;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);
                }

                var condition = ChildCount(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox) == 5;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountNumberFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);
                }

                var condition = ChildCount(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox) == 4;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountConditionFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);
                }

                var condition = ChildCount(AccessibilityInsights.Rules.PropertyConditions.ControlType.RadioButton) == 5;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountLessThanTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() < 6;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountLessThanFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() < 5;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountGreaterThanTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() > 4;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountGreaterThanFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() > 5;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountLessThanOrEqualToTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() <= 5;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountLessThanOrEqualToFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() <= 4;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountGreaterThanOrEqualToTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() >= 5;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildCountGreaterThanOrEqualToFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);
                }

                var condition = ChildCount() >= 6;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyDescendantTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            using (var grandchild = new MockA11yElement())
            {
                e.Children.Add(child);
                child.Children.Add(grandchild);
                grandchild.ControlTypeId = ComboBox;

                var condition = AnyDescendant(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyDescendantFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            using (var grandchild = new MockA11yElement())
            {
                e.Children.Add(child);
                child.Children.Add(grandchild);

                var condition = AnyDescendant(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAnyDescendantNoChildrenFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = AnyDescendant(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoDescendantTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            using (var grandchild = new MockA11yElement())
            {
                e.Children.Add(child);
                child.Children.Add(grandchild);

                var condition = NoDescendant(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoDescendantFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            using (var grandchild = new MockA11yElement())
            {
                e.Children.Add(child);
                child.Children.Add(grandchild);
                grandchild.ControlTypeId = ComboBox;

                var condition = NoDescendant(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNoDescendantNoChildrenTrue()
        {
            using (var e = new MockA11yElement())
            {
                var condition = NoDescendant(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllDescendantsTrue()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            using (var grandchild = new MockA11yElement())
            {
                e.Children.Add(child);
                child.Children.Add(grandchild);
                child.ControlTypeId = ComboBox;
                grandchild.ControlTypeId = ComboBox;

                var condition = AllDescendants(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllDescendantsFalse()
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            using (var grandchild = new MockA11yElement())
            {
                e.Children.Add(child);
                child.Children.Add(grandchild);
                child.ControlTypeId = ComboBox;

                var condition = AllDescendants(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestAllDescendantsNoChildrenFalse()
        {
            using (var e = new MockA11yElement())
            {
                var condition = AllDescendants(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        grandchild.ControlTypeId = ComboBox;
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount(AccessibilityInsights.Rules.PropertyConditions.ControlType.ComboBox) == 15;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountNumberFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() == 16;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountConditionFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    child.ControlTypeId = ComboBox;
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        grandchild.ControlTypeId = RadioButton;
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount(AccessibilityInsights.Rules.PropertyConditions.ControlType.RadioButton) == 15;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountLessThanTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() < 16;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountLessThanFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() < 15;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountGreaterThanTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() > 14;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountGreaterThanFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() > 15;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountLessThanOrEqualToTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() <= 15;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountLessThanOrEqualToFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() <= 14;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountGreaterThanOrEqualToTrue()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() >= 15;
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestDescendantCountGreaterThanOrEqualToFalse()
        {
            using (var e = new MockA11yElement())
            {
                for (var i = 0; i < 5; ++i)
                {
                    var child = new MockA11yElement();
                    e.Children.Add(child);

                    for (var j = 0; j < 2; ++j)
                    {
                        var grandchild = new MockA11yElement();
                        child.Children.Add(grandchild);
                    }
                }

                var condition = DescendantCount() >= 16;
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildOfSameTypeTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = ComboBox;

                for (var i = 0; i < 5; ++i)
                    e.Children.Add(new MockA11yElement());

                var child = e.Children[3] as MockA11yElement;
                child.ControlTypeId = ComboBox;

                var condition = AnyChild(HasSameType);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestChildOfSameTypeFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = ComboBox;

                for (var i = 0; i < 5; ++i)
                    e.Children.Add(new MockA11yElement());

                var condition = AnyChild(HasSameType);
                Assert.IsFalse(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestContextWithMultipleThreads()
        {
            // the condition AnyChild(HasSameType) uses the context mechanism
            // where the parent element is saved on the per-thread Context stack
            // and compared with its child in the HasSameType condition.
            // the idea of the test is that if the parents and children get out of sync, the assertion should fail.
            // if all the assertions in the loop are true, then the the thread local storage worked as expected.
            var tasks = new System.Collections.Generic.List<Task>();
            for (var i = 0; i < 20; ++i)
            {
                // we create a local variable because if we don't,
                // for some reason, the value passed into the TestContext function will be 20 most of the time.
                var n = i;
                tasks.Add(Task.Run(() => TestContext(n)));
            } // for

            Task.WaitAll(tasks.ToArray());
            foreach (var t in tasks)
                Assert.IsFalse(t.IsFaulted);
        }

        private void TestContext(int i)
        {
            using (var e = new MockA11yElement())
            using (var child = new MockA11yElement())
            {
                child.ControlTypeId = i;
                e.ControlTypeId = i;
                e.Children.Add(child);

                var condition = AnyChild(HasSameType);
                Assert.IsTrue(condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void ExactlyOne_True()
        {
            var condition = Relationships.ExactlyOne(Condition.False, Condition.True, Condition.False);
            Assert.IsTrue(condition.Matches(null));
        }

        [TestMethod]
        public void ExactlyOne_TwoFalse()
        {
            var condition = Relationships.ExactlyOne(Condition.True, Condition.True, Condition.False);
            Assert.IsFalse(condition.Matches(null));
        }

        [TestMethod]
        public void ExactlyOne_NoneFalse()
        {
            var condition = Relationships.ExactlyOne(Condition.False, Condition.False, Condition.False);
            Assert.IsFalse(condition.Matches(null));
        }

        [TestMethod]
        public void ExactlyOne_OneOptionTrue()
        {
            var condition = Relationships.ExactlyOne(Condition.True);
            Assert.IsTrue(condition.Matches(null));
        }

        [TestMethod]
        public void ExactlyOne_NoParamsFalse()
        {
            var condition = Relationships.ExactlyOne();
            Assert.IsFalse(condition.Matches(null));
        }

        [TestMethod]
        public void SiblingCount_Matches()
        {
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            var parent = new MockA11yElement();
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            child1.Parent = parent;
            child2.Parent = parent;

            var condition = Relationships.SiblingCount(Condition.True) == 2;

            Assert.IsTrue(condition.Matches(child1));
        }

        [TestMethod]
        public void SiblingCount_NoMatch_IncorrectNumber()
        {
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            var parent = new MockA11yElement();
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            child1.Parent = parent;
            child2.Parent = parent;

            var condition = Relationships.SiblingCount(Condition.True) == 3;

            Assert.IsFalse(condition.Matches(child1));
        }

        [TestMethod]
        public void SiblingCount_NoMatch_NoParent()
        {
            var child1 = new MockA11yElement();

            var condition = Relationships.SiblingCount(Condition.True) == -1;

            Assert.IsTrue(condition.Matches(child1));
        }

        [TestMethod]
        public void SiblingCount_NoMatch_ParentHasNoChildren()
        {
            var child = new MockA11yElement();
            var parent = new MockA11yElement();

            var condition = Relationships.SiblingCount(Condition.True) == -1;

            Assert.IsTrue(condition.Matches(child));
        }
    } // class
} // namespace
