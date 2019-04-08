// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class SelectionPatternTests
    {
        [TestMethod]
        public void CanSelectMultiple_True()
        {
            var pattern = new Mock<IA11yPattern>();
            pattern.Setup(p => p.GetValue<bool>(SelectionPattern.CanSelectMultipleProperty)).Returns(true);

            var m = new Mock<IA11yElement>();
            m.Setup(e => e.GetPattern(PatternType.UIA_SelectionPatternId)).Returns(pattern.Object);

            Assert.IsTrue(SelectionPattern.CanSelectMultiple.Matches(m.Object));
        }

        [TestMethod]
        public void CanSelectMultiple_False_NoProperty()
        {
            var pattern = new Mock<IA11yPattern>();

            var m = new Mock<IA11yElement>();
            m.Setup(e => e.GetPattern(PatternType.UIA_SelectionPatternId)).Returns(pattern.Object);

            Assert.IsFalse(SelectionPattern.CanSelectMultiple.Matches(m.Object));
        }

        [TestMethod]
        public void CanSelectMultiple_False_NoPattern()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.GetPattern(PatternType.UIA_SelectionPatternId)).Returns<IA11yPattern>(null);

            Assert.IsFalse(SelectionPattern.CanSelectMultiple.Matches(m.Object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CanSelectMultiple_Exception_NoElement()
        {
            SelectionPattern.CanSelectMultiple.Matches(null);
        }
    } // class
} // namespace
