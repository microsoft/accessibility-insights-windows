// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Rules.PropertyConditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class ValuePatternTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void ValueProperty_ReturnsCorrectStringProperty()
        {
            var fakeValue = "hello";
            var patternMock = new Mock<IA11yPattern>();
            var element = new Mock<IA11yElement>();
            patternMock.Setup(m => m.GetValue<string>(ValuePattern.ValuePropertyString)).Returns(fakeValue);
            element.Setup(m => m.GetPattern(Core.Types.PatternType.UIA_ValuePatternId)).Returns(patternMock.Object);
            Assert.IsTrue(ValuePattern.ValueProperty.Is(fakeValue).Matches(element.Object));
        }
    }
}
