// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Rules;

namespace Axe.Windows.RulesTest.Conditions
{
    [TestClass]
    public class ControlTypeConditionTest
    {
        [TestMethod]
        public void TestMatchingControlTypes()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = ControlType.Button;
                var test = new ControlTypeCondition(ControlType.Button);
                Assert.IsTrue(test.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNonMatchingControlTypes()
        {
            using (var e = new MockA11yElement())
            {
                e.ControlTypeId = ControlType.CheckBox;
                var test = new ControlTypeCondition(ControlType.Button);
                Assert.IsFalse(test.Matches(e));
            } // using
        }
    } // class
} // namespace
