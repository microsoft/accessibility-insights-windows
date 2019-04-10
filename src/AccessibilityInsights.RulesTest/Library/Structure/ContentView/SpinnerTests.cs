// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Rules;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.RulesTest.Library.Structure.ContentView
{
    [TestClass]
    public class SpinnerTests
    {
        private readonly static IRule Rule = new Axe.Windows.Rules.Library.ContentViewSpinnerStructure();

        [TestMethod]
        public void Spinner_ZeroListItemChildren_Pass()
        {
            var spinner = new MockA11yElement();
            spinner.ControlTypeId = ControlType.Spinner;

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(spinner));
        }

        [TestMethod]
        public void Spinner_ListItemChildren_Pass()
        {
            var spinner = new MockA11yElement();
            spinner.ControlTypeId = ControlType.Spinner;

            var listItem = new MockA11yElement();
            listItem.ControlTypeId = ControlType.ListItem;

            spinner.Children.Add(listItem);

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(spinner));
        }
    } // class
} // namespace
