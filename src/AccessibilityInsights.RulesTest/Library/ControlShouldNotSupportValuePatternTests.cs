using Axe.Windows.Core.Bases;
using Axe.Windows.Rules;
using Axe.Windows.Rules.PropertyConditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class ControlShouldNotSupportValuePatternTests
    {
        private Rules.IRule Rule = new Rules.Library.ControlShouldNotSupportValuePattern();

        [TestMethod]
        [Timeout(1000)]
        public void NoValuePattern_NotApplicable()
        {
            var element = new Mock<IA11yElement>();
            Assert.IsFalse(this.Rule.Condition.Matches(element.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void EmptyValue_NotApplicable()
        {
            var element = GetTextControlWithValueAndName(string.Empty, string.Empty);
            Assert.IsFalse(this.Rule.Condition.Matches(element.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void IdenticalValueName_Pass()
        {
            var element = GetTextControlWithValueAndName(value: "12 APPLES", name: "12 apples");
            var res = this.Rule.Condition.Matches(element.Object);
            Assert.IsTrue(res);
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(element.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ValueContainedInName_Pass()
        {
            var element = GetTextControlWithValueAndName(value: "APPLES", name: "12 apples");
            Assert.IsTrue(this.Rule.Condition.Matches(element.Object));
            Assert.AreEqual(EvaluationCode.Pass, this.Rule.Evaluate(element.Object));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ValueExceedsName_Fail()
        {
            var element = GetTextControlWithValueAndName(value: "12 apples", name: "APPLES");
            Assert.IsTrue(this.Rule.Condition.Matches(element.Object));
            Assert.AreEqual(EvaluationCode.Error, this.Rule.Evaluate(element.Object));
        }

        private Mock<IA11yElement> GetTextControlWithValueAndName(string value, string name = null)
        {
            var patternMock = new Mock<IA11yPattern>();
            var element = new Mock<IA11yElement>();
            patternMock.Setup(m => m.GetValue<string>(ValuePattern.ValuePropertyString)).Returns(value);
            element.Setup(m => m.GetPattern(Core.Types.PatternType.UIA_ValuePatternId)).Returns(patternMock.Object);
            element.Setup(m => m.Name).Returns(name);
            element.Setup(m => m.ControlTypeId).Returns(ControlType.Text);
            return element;
        }
    }
}
