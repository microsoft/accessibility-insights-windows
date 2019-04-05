// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class NameExcludesSpecialCharacters
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.NameExcludesSpecialCharacters();

        [TestMethod]
        public void TestNameExcludesSpecialCharactersTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.Name = "Button";
                Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestNameExcludesSpecialCharactersFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.Name = "\xE946";
                Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestNullElement()
        {
            Action action = () => { Rule.Evaluate(null); };
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void TestNullName()
        {
            using (var e = new MockA11yElement())
            {
                Action action = () => { Rule.Evaluate(e); };
                Assert.ThrowsException<ArgumentException>(action);
            } // using
        }

    } // class
} // namespace
