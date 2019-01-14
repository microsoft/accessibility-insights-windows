// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class SelectionItemPatternTests
    {
        [TestMethod]
        public void IsSelected_True()
        {
            var pattern = new Mock<IA11yPattern>();
            pattern.Setup(p => p.GetValue<bool>(SelectionItemPattern.IsSelectedProperty)).Returns(true);

            var m = new Mock<IA11yElement>();
            m.Setup(e => e.GetPattern(PatternType.UIA_SelectionItemPatternId)).Returns(pattern.Object);

            Assert.IsTrue(SelectionItemPattern.IsSelected.Matches(m.Object));
        }

        [TestMethod]
        public void IsSelected_False_NoProperty()
        {
            var pattern = new Mock<IA11yPattern>();

            var m = new Mock<IA11yElement>();
            m.Setup(e => e.GetPattern(PatternType.UIA_SelectionItemPatternId)).Returns(pattern.Object);

            Assert.IsFalse(SelectionItemPattern.IsSelected.Matches(m.Object));
        }

        [TestMethod]
        public void IsSelected_False_NoPattern()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.GetPattern(PatternType.UIA_SelectionItemPatternId)).Returns<IA11yPattern>(null);

            Assert.IsFalse(SelectionItemPattern.IsSelected.Matches(m.Object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsSelected_Exception_NoElement()
        {
            SelectionItemPattern.IsSelected.Matches(null);
        }
    } // class
} // namespace
