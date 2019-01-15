// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    [TestCategory("AccessibilityInsights.Rules")]
    public class ControlShouldNotSupportInvokePattern
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.ControlShouldNotSupportInvokePattern();

        [TestMethod]
        public void TabitemNotEdgeFramework_Applicable()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_TabItemControlTypeId;
            e.Framework = Framework.Win32;

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TabitemEdgeFramework_NotApplicable()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_TabControlTypeId;
            e.Framework = Framework.Edge;

            Assert.IsFalse(Rule.Condition.Matches(e));
        }
    }
}
