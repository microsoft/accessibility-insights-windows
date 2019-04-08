// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    [TestCategory("Axe.Windows.Rules")]
    public class SelectionPatternSelectionRequired
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.SelectionPatternSelectionRequired();

        [TestMethod]
        public void TabControlWithSelectionPatternButNotEdgeFramework_Applicable()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TabControlTypeId;
            e.Patterns.Add(new A11yPattern(e, PatternType.UIA_SelectionPatternId));
            e.Framework = Framework.Win32;

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TabControlWithSelectionPatternEdgeFramework_NotApplicable()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Axe.Windows.Core.Types.ControlType.UIA_TabControlTypeId;
            e.Patterns.Add(new A11yPattern(e, PatternType.UIA_SelectionPatternId));
            e.Framework = Framework.Edge;

            Assert.IsFalse(Rule.Condition.Matches(e));
        }
    } // class
} // namespace
