// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Enums;
using Axe.Windows.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    [TestCategory("Axe.Windows.Rules")]
    public class ControlShouldSupportSetInfoTest
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.ControlShouldSupportSetInfo();

        [TestMethod]
        public void TestControlShouldSupportSetInfoNoPositionInSetPropertyForListItemFail()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ListItemControlTypeId;
            e.SizeOfSet = 1;
            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoNoSizeOfSetPropertyForListItemFail()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ListItemControlTypeId;
            e.PositionInSet = 1;
            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoBothNonExistForListItemFail()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ListItemControlTypeId;
            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoBothExistForListItemPass()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ListItemControlTypeId;
            e.PositionInSet = 1;
            e.SizeOfSet = 1;
            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoNoPositionInSetPropertyForTreeItemFail()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
            e.SizeOfSet = 1;
            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoNoSizeOfSetPropertyForTreeItemFail()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
            e.PositionInSet = 1;
            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoBothNonExistForTreeItemFail()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoBothExistForTreeItemPass()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
            e.PositionInSet = 1;
            e.SizeOfSet = 1;
            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoWin32FrameworkFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ListItemControlTypeId;
                e.PositionInSet = 1;
                e.SizeOfSet = 1;
                e.Framework = Framework.Win32;
                Assert.IsFalse(Rule.Condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoWPFFrameworkTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
                e.PositionInSet = 1;
                e.SizeOfSet = 1;
                e.Framework = Framework.WPF;
                Assert.IsTrue(Rule.Condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoXAMLFrameworkTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;
                e.PositionInSet = 1;
                e.SizeOfSet = 1;
                e.Framework = Framework.XAML;
                Assert.IsTrue(Rule.Condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestControlShouldSupportSetInfoNotListOrTreeItemFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ImageControlTypeId;
                e.PositionInSet = 1;
                e.SizeOfSet = 1;
                e.Framework = Framework.WPF;
                Assert.IsFalse(Rule.Condition.Matches(e));
            } // using
        }
    }
}
