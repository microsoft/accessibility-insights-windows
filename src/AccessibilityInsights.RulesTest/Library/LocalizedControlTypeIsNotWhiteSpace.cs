// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class LocalizedControlTypeIsNotWhiteSpace
    {
        private AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.LocalizedControlTypeIsNotWhiteSpace();

        [TestMethod]
        public void TestLocalizedControlTypeNull()
        {
            var e = new MockA11yElement();
            e.LocalizedControlType = null;
            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestLocalizedControlTypeEmpty()
        {
            var e = new MockA11yElement();
            e.LocalizedControlType = "";
            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestLocalizedControlTypeNotEmpty()
        {
            var e = new MockA11yElement();
            e.LocalizedControlType = " ";
            Assert.IsTrue(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestLocalizedControlTypeWithOnlyWhiteSpace()
        {
            var e = new MockA11yElement();
            e.LocalizedControlType = "   ";
            Assert.AreEqual(EvaluationCode.Error, this.Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestLocalizedControlTypeWithVisibleCharacters()
        {
            var e = new MockA11yElement();
            e.LocalizedControlType = "hello world!";
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }
    } // class
} // LocalizedControlTypespace
