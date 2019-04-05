// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;
using static Axe.Windows.RulesTest.ControlType;
using System.Drawing;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    [TestCategory("Axe.Windows.Rules")]
    public class NameIsNotEmpty
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.NameIsNotEmpty();

        [TestMethod]
        public void TestNameEmpty()
        {
            var e = new MockA11yElement();
            e.Name = "";

            Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
        }

        [TestMethod]
        public void TestNameNotEmpty()
        {
            var e = new MockA11yElement();
            e.Name = " ";

            Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
        }

        [TestMethod]
        public void TestNameIsNotEmptyProgressBar()
        {
            var e = new MockA11yElement();
            e.IsKeyboardFocusable = false;
            e.ControlTypeId = CheckBox;
            e.Name = "";
            e.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            Assert.IsFalse(Rule.Condition.Matches(e));

            e.ControlTypeId = ProgressBar;
            Assert.IsTrue(Rule.Condition.Matches(e));
        }
    } // class
} // namespace
