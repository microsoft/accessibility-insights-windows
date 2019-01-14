// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;
using AccessibilityInsights.Core.Types;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class ButtonInvokeAndExpandCollapsePatterns
    {
        private AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.ButtonInvokeAndExpandCollapsePatterns();

        /// <summary>
        /// Rule not applicable
        /// A button supports only ExpandCollapse pattern
        /// </summary>
        [TestMethod]
        public void ControlOtherThanButton()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_AppBarControlTypeId;

            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        /// <summary>
        /// Rule not applicable
        /// A button supports only ExpandCollapse pattern
        /// </summary>
        [TestMethod]
        public void ButtonWithExpandCollapsePattern()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_ExpandCollapsePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// Rule not applicable
        /// A button supports Invoke pattern only
        /// </summary>
        [TestMethod]
        public void ButtonWithInvoke()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_InvokePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// Rule not applicable
        /// A button supports Toggle pattern only
        /// </summary>
        [TestMethod]
        public void ButtonWithToggle()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_TogglePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }

        /// <summary>
        /// Rule applicable and Result should be warning
        /// both of Invoke and ExpandCollapse may be supported by a button. 
        /// </summary>
        [TestMethod]
        public void ButtonWithInvokeAndExpandCollapse()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;

            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_ExpandCollapsePatternId));
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_InvokePatternId));

            Assert.IsTrue(this.Rule.Condition.Matches(e));
            Assert.AreEqual(EvaluationCode.Warning, this.Rule.Evaluate(e));
        }
    }
} 
