// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class LocalizedControlTypeIsNotCustom
    {
        private AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.LocalizedControlTypeIsNotCustom();

        [TestMethod]
        public void TestControlTypeIdIsNotCustomControl()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_AppBarControlTypeId;

            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestControlTypeIdIsCustomControl()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_CustomControlTypeId;
            e.LocalizedControlType = " ";
            e.IsKeyboardFocusable = true;

            Assert.IsTrue(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestControlTypeIdIsCustomControlButNotFocusable()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_CustomControlTypeId;
            e.LocalizedControlType = " ";

            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestControlTypeIdIsCustomControlAndLocalizedControlTypeIsCustom()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_CustomControlTypeId;
            e.LocalizedControlType = "custom";

            Assert.AreEqual(EvaluationCode.Error, this.Rule.Evaluate(e));
        }

        [TestMethod]
        public void TestControlTypeIdIsCustomControlAndLocalizedControlTypeIsNotCustom()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_CustomControlTypeId;
            e.LocalizedControlType = "not custom";

            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(e));
        }
    } // class
} // LocalizedControlTypespace
