// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class NameIsReasonableLength
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.NameIsReasonableLength();

        [TestMethod]
        public void TestNameIsReasonableLengthTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.Name = "Hello";
                Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestNameIsReasonableLengthFalse()
        {
            using (var e = new MockA11yElement())
            {
                StringBuilder s = new StringBuilder("*");
                for (var i = 0; i < 10; ++i)
                    s.Append(s.ToString() + s.ToString());

                e.Name = s.ToString();
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
                e.LocalizedControlType = "Button";
                Action action = () => { Rule.Evaluate(e); };
                Assert.ThrowsException<ArgumentException>(action);
            } // using
        }
    } // class
} // namespace
