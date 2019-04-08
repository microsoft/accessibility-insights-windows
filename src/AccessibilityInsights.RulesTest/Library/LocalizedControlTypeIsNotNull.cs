// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    [TestCategory("Axe.Windows.Rules")]
    public class LocalizedControlTypeIsNotNull
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.LocalizedControlTypeIsNotNull();

        [TestMethod]
        public void IsKeyboardFocusableTrue_LocalizedControlTypeNull_ScanError()
        {
            var e = new MockA11yElement();
            e.LocalizedControlType = null;
            e.IsKeyboardFocusable = true;

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void ConditionMismatch_IsKeyboardFocusableFalse_ReturnFalse()
        {
            var e = new MockA11yElement();
            e.IsKeyboardFocusable = false;

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void LocalizedControlTypeNotNull_ScanPass()
        {
            var e = new MockA11yElement();
            e.IsKeyboardFocusable = true;
            e.LocalizedControlType = "abc";

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }
    } // class
} // namespace
