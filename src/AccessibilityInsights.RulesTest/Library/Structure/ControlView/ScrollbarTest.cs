// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class ControlViewScrollbarStructureTest
    {
        private static Rules.IRule Rule = new Rules.Library.ControlViewScrollbarStructure();

        [TestMethod]
        [Timeout(1000)]
        public void ScrollbarNoChildren_Pass()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.ControlTypeId).Returns(ControlType.ScrollBar);
            
            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(m.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScrollbarOneButton_Note()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.ControlTypeId).Returns(Core.Types.ControlType.UIA_ScrollBarControlTypeId);
            m.Setup(e => e.Children).Returns(() => GenerateElementsWithControlTypes(new Dictionary<int, int>() {
                { Core.Types.ControlType.UIA_ButtonControlTypeId, 1 },
            }));

            Assert.AreEqual(EvaluationCode.Note, Rule.Evaluate(m.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScrollbarTwoButtons_Pass()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.ControlTypeId).Returns(Core.Types.ControlType.UIA_ScrollBarControlTypeId);
            m.Setup(e => e.Children).Returns(() => GenerateElementsWithControlTypes(new Dictionary<int, int>() {
                { Core.Types.ControlType.UIA_ButtonControlTypeId, 2 },
            }));

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(m.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScrollbarFourButtons_Pass()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.ControlTypeId).Returns(Core.Types.ControlType.UIA_ScrollBarControlTypeId);
            m.Setup(e => e.Children).Returns(() => GenerateElementsWithControlTypes(new Dictionary<int, int>() {
                { Core.Types.ControlType.UIA_ButtonControlTypeId, 4 },
            }));

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(m.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScrollbarFourButtonsOneThumb_Pass()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.ControlTypeId).Returns(Core.Types.ControlType.UIA_ScrollBarControlTypeId);
            m.Setup(e => e.Children).Returns(() => GenerateElementsWithControlTypes(new Dictionary<int, int>() {
                { Core.Types.ControlType.UIA_ButtonControlTypeId, 4 },
                { Core.Types.ControlType.UIA_ThumbControlTypeId, 1 },
            }));

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(m.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ScrollbarFourButtonsTwoThumbs_Note()
        {
            var m = new Mock<IA11yElement>();
            m.Setup(e => e.ControlTypeId).Returns(Core.Types.ControlType.UIA_ScrollBarControlTypeId);
            m.Setup(e => e.Children).Returns(() => GenerateElementsWithControlTypes(new Dictionary<int, int>() {
                { Core.Types.ControlType.UIA_ButtonControlTypeId, 4 },
                { Core.Types.ControlType.UIA_ThumbControlTypeId, 2 },
            }));

            Assert.AreEqual(EvaluationCode.Note, Rule.Evaluate(m.Object));
        }

        /// <summary>
        /// Returns a sequence of elements with the given distribution of control types,
        /// </summary>
        /// <param name="controlTypeCounts">mapping from control type to number of elements for that type</param>
        /// <returns>sequence of elements with given control type counts</returns>
        private IEnumerable<IA11yElement> GenerateElementsWithControlTypes(Dictionary<int, int> controlTypeCounts)
        {
            return controlTypeCounts.SelectMany(controlTypeToCount => 
                GenerateElementsWithControlType(controlTypeToCount.Key, controlTypeToCount.Value));
        }

        /// <summary>
        /// Returns a sequence of {count} elements with the {controlType} control type
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private IEnumerable<IA11yElement> GenerateElementsWithControlType(int controlType, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var m = new Mock<IA11yElement>();
                m.Setup(e => e.ControlTypeId).Returns(controlType);
                yield return m.Object;
            }
        }
    } // class
} // namespace
