// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Rules;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.RulesTest.Library.Structure.ControlView
{
    [TestClass]
    public class SpinnerTests
    {
        private readonly static IRule Rule = new Axe.Windows.Rules.Library.ControlViewSpinnerStructure();

        [TestMethod]
        public void Spinner_ZeroListItemChildren_Pass()
        {
            var spinner = new MockA11yElement();
            spinner.ControlTypeId = ControlType.Spinner;

            var button1 = new MockA11yElement();
            button1.ControlTypeId = ControlType.Button;

            var button2 = new MockA11yElement();
            button2.ControlTypeId = ControlType.Button;

            spinner.Children.Add(button1);
            spinner.Children.Add(button2);

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(spinner));
        }

        [TestMethod]
        public void Spinner_ListItemChildren_Pass()
        {
            var spinner = new MockA11yElement();
            spinner.ControlTypeId = ControlType.Spinner;

            var button1 = new MockA11yElement();
            button1.ControlTypeId = ControlType.Button;

            var button2 = new MockA11yElement();
            button2.ControlTypeId = ControlType.Button;

            var listItem = new MockA11yElement();
            listItem.ControlTypeId = ControlType.ListItem;

            spinner.Children.Add(button1);
            spinner.Children.Add(button2);
            spinner.Children.Add(listItem);

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(spinner));
        }
    } // class
} // namespace
