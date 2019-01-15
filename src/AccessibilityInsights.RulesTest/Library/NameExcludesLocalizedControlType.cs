// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AccessibilityInsights.RulesTest.ControlType;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class NameExcludesLocalizedControlType
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.NameExcludesLocalizedControlType();

        [TestMethod]
        public void NameExcludesLocalizedControlType_Pass()
        {
            var e = new MockA11yElement();
            e.Name = "This is a button";
            e.LocalizedControlType = "Checkbox";

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }

        [TestMethod]
        public void NameExcludesLocalizedControlType_PassSimilar()
        {
            var e = new MockA11yElement();
            e.Name = "Comfortable";
            e.LocalizedControlType = "Tab";

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }

        [TestMethod]
        public void NameExcludesLocalizedControlType_Fail()
        {
            var e = new MockA11yElement();
            e.Name = "This is a button, yep";
            e.LocalizedControlType = "Button";

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void NameExcludesLocalizedControlType_FailIdentical()
        {
            var e = new MockA11yElement();
            e.Name = "Custom";
            e.LocalizedControlType = "Custom";

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NameExcludesLocalizedControlType_ExceptionNullName()
        {
            var e = new MockA11yElement();
            e.Name = null;
            e.LocalizedControlType = "Button";

            Rule.Evaluate(e);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NameExcludesLocalizedControlType_ExceptionNullLocalizedControlType()
        {
            var e = new MockA11yElement();
            e.Name = "name";
            e.LocalizedControlType = null;

            Rule.Evaluate(e);
        }

        [TestMethod]
        public void NameExcludesLocalizedControlType_NotApplicableType()
        {
            var e = new MockA11yElement();
            e.Name = "same";
            e.LocalizedControlType = "same";

            int[] types = { AppBar, Header, MenuBar, SemanticZoom, StatusBar, TitleBar };

            foreach (var t in types)
            {
                e.ControlTypeId = t;
                Assert.IsFalse(Rule.Condition.Matches(e));
            } // for each type
        }
    } // class
} // namespace
