// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;
using Axe.Windows.Core.Types;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class ControlShouldSupportTablePattern
    {
        private Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.ControlShouldSupportTablePattern();

        [TestMethod]
        public void HasTablePattern_Pass()
        {
            var e = new MockA11yElement();
            e.Patterns.Add(new A11yPattern(e, PatternType.UIA_TablePatternId));

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }

        [TestMethod]
        public void NoTablePattern_Error()
        {
            var e = new MockA11yElement();

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void NoTablePatternInEdgeFramework_Error()
        {
            var e = new MockA11yElement();
            e.Framework = Framework.Edge;

            Assert.AreEqual(EvaluationCode.Warning, Rule.Evaluate(e));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNull_Exception()
        {
            Rule.Evaluate(null);
        }
    } // class
} // namespace
