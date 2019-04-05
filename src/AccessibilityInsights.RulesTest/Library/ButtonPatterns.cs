// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;
using Axe.Windows.Core.Types;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class ButtonPatterns
    {
        private Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.ButtonShouldHavePatterns();

        /// <summary>
        /// Error
        /// A button supports only ExpandCollapse pattern
        /// </summary>
        [TestMethod]
        public void ButtonWithExpandCollapsePattern()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_ExpandCollapsePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// Pass
        /// A button supports Invoke pattern only
        /// </summary>
        [TestMethod]
        public void ButtonWithInvoke()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_InvokePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// Pass
        /// A button supports Toggle pattern only
        /// </summary>
        [TestMethod]
        public void ButtonWithToggle()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_TogglePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// Pass
        /// This rule doesn't care about having both of Invoke and Toggle. 
        /// </summary>
        [TestMethod]
        public void ButtonWithToggleAndInvoke()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_TogglePatternId));
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_InvokePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }
    }
} 
