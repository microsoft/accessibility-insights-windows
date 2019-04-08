// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class NameIsInformative
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.NameIsInformative();

        [TestMethod]
        public void NameMatchesDotNetType()
        {
            using (var e = new MockA11yElement())
            {
                string[] stringsToTry =
                    {
                    "Microsoft.xxx.xxx",
                    "Microsoft.x",
                    "Windows.xxx.xxx",
                    "Windows.abc"
                };

                foreach (var s in stringsToTry)
                    {
                    e.Name = s;
                    Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
                }
            } // using
        }

        [TestMethod]
        public void NameDoesNotMatchDotNetType()
        {
            using (var e = new MockA11yElement())
            {
                string[] stringsToTry =
                    {
                    " Microsoft",
                    "Microsoft. ",
                    "Microsoft.*",
                    " Windows",
                    "Windows. ",
                    "Windows.*",
                };

                foreach (var s in stringsToTry)
                {
                    e.Name = s;
                    Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
                }
            } // using
        }
    } // class
} // namespace
