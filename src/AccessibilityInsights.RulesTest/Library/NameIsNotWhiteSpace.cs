// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class NameIsNotWhiteSpace
    {
        private Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.NameIsNotWhiteSpace();

        [TestMethod]
        public void TestNameNull()
        {
            var e = new MockA11yElement();
            e.Name = null;
            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestNameEmpty()
        {
            var e = new MockA11yElement();
            e.Name = "";
            Assert.IsFalse(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestNameNotEmpty()
        {
            var e = new MockA11yElement();
            e.Name = " ";
            Assert.IsTrue(this.Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TestNameWithOnlyWhiteSpace()
        {
            var e = new MockA11yElement();
            e.Name = "   ";
            Assert.AreNotEqual(this.Rule.Evaluate(e), EvaluationCode.Pass);
        }

        [TestMethod]
        public void TestNameWithVisibleCharacters()
        {
            var e = new MockA11yElement();
            e.Name = "hello world!";
            Assert.AreEqual(this.Rule.Evaluate(e), EvaluationCode.Pass);
        }
    } // class
} // namespace
