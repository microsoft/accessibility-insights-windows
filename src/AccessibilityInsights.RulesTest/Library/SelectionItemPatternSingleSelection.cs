// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.PropertyConditions;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;
using static AccessibilityInsights.RulesTest.ControlType;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class SelectionItemPatternSingleSelectionTests
    {
        private readonly AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.SelectionItemPatternSingleSelection();

        private static Mock<IA11yElement> CreateMockElement(IA11yElement parent, int controlType, bool isSelected)
        {
            var pattern = new Mock<IA11yPattern>(MockBehavior.Strict);
            pattern.Setup(p => p.GetValue<bool>(SelectionItemPattern.IsSelectedProperty)).Returns(isSelected);

            var m = new Mock<IA11yElement>(MockBehavior.Strict);
            m.Setup(e => e.ControlTypeId).Returns(controlType);
            m.Setup(e => e.GetPattern(PatternType.UIA_SelectionItemPatternId)).Returns(pattern.Object);
            m.Setup(e => e.Parent).Returns(parent);

            return m;
        }

        private static Mock<IA11yElement> CreateMockTab(IA11yElement parent, bool isSelected)
        {
            return CreateMockElement(parent, TabItem, isSelected);
        }

        private static Mock<IA11yElement> CreateMockButton(IA11yElement parent, bool isSelected)
        {
            return CreateMockElement(parent, Button, isSelected);
        }

        [TestMethod]
        public void SelectionItemPatternSingleSelection_Pass()
        {
            var parent = new Mock<IA11yElement>(MockBehavior.Strict);

            var item1 = CreateMockTab(parent.Object, false);
            var item2 = CreateMockTab(parent.Object, true);
            var item3 = CreateMockTab(parent.Object, false);

            IA11yElement[] children = { item1.Object, item2.Object, item3.Object };

            parent.Setup(e => e.Children).Returns(children);

            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(item3.Object));
        }

        [TestMethod]
        public void SelectionItemPatternSingleSelection_Pass_DifferentTypes()
        {
            var parent = new Mock<IA11yElement>(MockBehavior.Strict);

            var item1 = CreateMockTab(parent.Object, false);
            var item2 = CreateMockTab(parent.Object, true);
            var item3 = CreateMockButton(parent.Object, true);

            IA11yElement[] children = { item1.Object, item2.Object, item3.Object };

            parent.Setup(e => e.Children).Returns(children);

            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(item3.Object));
        }

        [TestMethod]
        public void SelectionItemPatternSingleSelection_Error()
        {
            var parent = new Mock<IA11yElement>(MockBehavior.Strict);

            var item1 = CreateMockTab(parent.Object, false);
            var item2 = CreateMockTab(parent.Object, true);
            var item3 = CreateMockTab(parent.Object, true);

            IA11yElement[] children = { item1.Object, item2.Object, item3.Object };
            parent.Setup(e => e.Children).Returns(children);

            Assert.AreEqual(EvaluationCode.Error, this.Rule.Evaluate(item3.Object));
        }
    } // class
} // namespace
