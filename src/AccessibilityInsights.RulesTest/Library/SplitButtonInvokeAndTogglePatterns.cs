// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;
using Axe.Windows.Core.Types;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class SplitButtonInvokeAndTogglePatterns
    {
        private Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.SplitButtonInvokeAndTogglePatterns();

        /// <summary>
        /// A splitbutton with both of Invoke and Toggle Patterns. error.
        /// </summary>
        [TestMethod]
        public void SplitButtonWithInvokeAndToggle()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_InvokePatternId));
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_TogglePatternId));

            Assert.AreEqual(EvaluationCode.Error, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// A splitbutton with both of Invoke Pattern. pass.
        /// </summary>
        [TestMethod]
        public void SplitButtonWithInvoke()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_InvokePatternId));

            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// A splitbutton with both of Toggle Pattern. pass.
        /// </summary>
        [TestMethod]
        public void SplitButtonWithToggle()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_TogglePatternId));

            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// false condition. the rule is only for splitbutton
        /// </summary>
        [TestMethod]
        public void TestConditionFalseCase()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId;

            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        /// <summary>
        /// true condition. the rule is only for splitbutton
        /// </summary>
        [TestMethod]
        public void TestConditionTrueCase()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_SplitButtonControlTypeId;

            Assert.IsTrue(this.Rule.Condition.Matches(e));
        }
    }
} 
