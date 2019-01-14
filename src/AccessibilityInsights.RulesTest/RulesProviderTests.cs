// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AccessibilityInsights.RulesTest
{
    [TestClass]
    public class RulesProviderTests
    {
        [TestMethod]
        public void RulesAreCreatedOnlyOnce()
        {
            var rule = new Mock<IRule>(MockBehavior.Strict);
            var factory = new Mock<IRuleFactory>(MockBehavior.Strict);
            factory.Setup(o => o.CreateRule(RuleId.NameNotNull)).Returns(rule.Object);
            var provider = new RuleProvider(factory.Object);

            for (int i = 0; i < 5; ++i)
            {
                provider.GetRule(RuleId.NameNotNull);
                factory.Verify(o => o.CreateRule(RuleId.NameNotNull), Times.Once());
            }
        }

        [TestMethod]
        public void RulesAreCreatedOnlyOnceForAll()
        {
            var rule = new Mock<IRule>(MockBehavior.Strict);
            var factory = new Mock<IRuleFactory>(MockBehavior.Strict);
            factory.Setup(o => o.CreateRule(It.IsAny<RuleId>())).Returns(rule.Object);
            factory.Setup(o => o.CreateRule(RuleId.NameNotNull)).Returns(rule.Object);
            var provider = new RuleProvider(factory.Object);

            for (int i = 0; i < 5; ++i)
            {
                provider.All.GetEnumerator();
                factory.Verify(o => o.CreateRule(RuleId.NameNotNull), Times.Once());
            }
        }
    } // class
} // namespace
