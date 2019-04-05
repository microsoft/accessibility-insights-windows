// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;
using Axe.Windows.Core.Types;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class ControlShouldSupportExpandCollapsePattern
    {
        private Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.ControlShouldSupportExpandCollapsePattern();

        /// <summary>
        /// Condition should not match since TreeItem doesn't have any childTreeItem
        /// </summary>
        [TestMethod]
        public void TestTreeItemWithoutChildTreeItem()
        {
            var e = new MockA11yElement();
            var ec = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
            ec.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TitleBarControlTypeId;

            e.Children.Add(ec);

            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        /// <summary>
        /// Condition should match since TreeItem has a treeitem child
        /// but ExpandCollapse is not supported. so scan fails.
        /// </summary>
        [TestMethod]
        public void TestTreeItemWithChildTreeItemWithoutExpandCollapsePattern()
        {
            var e = new MockA11yElement();
            var ec = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
            ec.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;

            e.Children.Add(ec);

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Error, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// Condition should match since TreeItem has a treeitem child
        /// and ExpandCollapse is supported. so scan succeeds
        /// </summary>
        [TestMethod]
        public void TestTreeItemWithChildTreeItemWithExpandCollapsePattern()
        {
            var e = new MockA11yElement();
            var ec = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
            ec.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;

            e.Children.Add(ec);
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_ExpandCollapsePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }
    }
} 
