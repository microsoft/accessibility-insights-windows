// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.AutomationTests
{
    [TestClass]
    public class ExecutionWrapperUnitTests
    {
        private class TestResult
        {
            public string Detail { get; }

            public bool IsError { get; }

            public TestResult(string detail, bool isError = false)
            {
                Detail = detail;
                IsError = isError;
            }
        }

        const string TestString = "He's dead, Jim!";

        [TestMethod]
        [Timeout (5000)]
        public void ExecuteCommand_CommandIsNull_CallsErrorFactory_Automation003InDetail()
        {
            TestResult result = ExecutionWrapper.ExecuteCommand<TestResult>(
                null,
                (errorDetail) =>
                {
                    return new TestResult(errorDetail, true);
                });

            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Detail.Contains(" Automation003:"));
            Assert.IsTrue(result.Detail.Contains("System.NullReferenceException"));
        }

        [TestMethod]
        [Timeout (1000)]
        public void ExecuteCommand_CommandThrowsNonAutomationException_CallsErrorFactory_Automation003InDetail()
        {
            TestResult result = ExecutionWrapper.ExecuteCommand<TestResult>(
                () =>
                {
                    throw new ArgumentException(TestString);
                },
                (errorDetail) =>
                {
                    return new TestResult(errorDetail, true);
                });

            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Detail.Contains(" Automation003:"));
            Assert.IsTrue(result.Detail.Contains("System.ArgumentException"));
            Assert.IsTrue(result.Detail.Contains(TestString));
        }

        [TestMethod]
        [Timeout (1000)]
        public void ExecuteCommand_CommandThrowsAutomationException_CallsErrorFactory_Automation003InDetail()
        {
            TestResult result = ExecutionWrapper.ExecuteCommand<TestResult>(
                () =>
                {
                    throw new A11yAutomationException(TestString);
                },
                (errorDetail) =>
                {
                    return new TestResult(errorDetail, true);
                });

            Assert.IsTrue(result.IsError);
            Assert.AreEqual(TestString, result.Detail);
        }

        [TestMethod]
        [Timeout (1000)]
        public void ExecuteCommand_CommandReturnsObject_SameObjectIsReturnedToCaller()
        {
            TestResult result = ExecutionWrapper.ExecuteCommand<TestResult>(
                () => new TestResult(TestString),
                null);

            Assert.AreEqual(TestString, result.Detail);
        }
    }
}
