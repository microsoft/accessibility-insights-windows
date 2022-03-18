// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.Extensions.DiskLoggingTelemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetryTests
{
    [TestClass]
    public class LogWriterUnitTests
    {
        private readonly DateTime TestTime = new DateTime(2022, 3, 18, 20, 6, 10, DateTimeKind.Utc);
        private Mock<ILogFileHelper> _logFileHelperMock;
        private LogWriter _testSubject;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _logFileHelperMock = new Mock<ILogFileHelper>(MockBehavior.Strict);
            _testSubject = new LogWriter(() => TestTime, _logFileHelperMock.Object);
        }

        [TestMethod]
        public void Ctor_TimeProviderIsNull_ThrowsArgumentNullException()
        {
            ArgumentNullException e = Assert.ThrowsException<ArgumentNullException>
                (() => new LogWriter(null, _logFileHelperMock.Object));
            Assert.AreEqual("timeProvider", e.ParamName);
        }

        [TestMethod]
        public void Ctor_LogFileHelperIsNull_ThrowsArgumentNullException()
        {
            ArgumentNullException e = Assert.ThrowsException<ArgumentNullException>
                (() => new LogWriter(() => TestTime, null));
            Assert.AreEqual("logFileHelper", e.ParamName);
        }

        [TestMethod]
        public void LogThisData_SingleCall_CallsResetLogFileOnce()
        {
            _logFileHelperMock.Setup(x => x.ResetLogFile());
            _logFileHelperMock.Setup(x => x.AppendLinesToLogFile(It.IsAny<IEnumerable<string>>()));

            _testSubject.LogThisData("title", "data");

            _logFileHelperMock.VerifyAll();
            _logFileHelperMock.Verify(x => x.ResetLogFile(), Times.Once());
        }

        [TestMethod]
        public void LogThisData_MultipleCalls_CallsResetLogFileOnlyOnce()
        {
            _logFileHelperMock.Setup(x => x.ResetLogFile());
            _logFileHelperMock.Setup(x => x.AppendLinesToLogFile(It.IsAny<IEnumerable<string>>()));

            _testSubject.LogThisData("title", "data1");
            _testSubject.LogThisData("title", "data2");
            _testSubject.LogThisData("title", "data3");

            _logFileHelperMock.VerifyAll();
            _logFileHelperMock.Verify(x => x.ResetLogFile(), Times.Once());
            _logFileHelperMock.Verify(x =>
                x.AppendLinesToLogFile(It.IsAny<IEnumerable<string>>()), Times.Exactly(3));
        }

        [TestMethod]
        public void LogThisData_DataLoggedMatchesExpectation()
        {
            const string expectedTitle = "My title";
            const string expectedData = "Mhy data";

            List<string> actualLines = null;

            _logFileHelperMock.Setup(x => x.ResetLogFile());
            _logFileHelperMock
                .Setup(x => x.AppendLinesToLogFile(It.IsAny<IEnumerable<string>>()))
                .Callback((IEnumerable<string> lines) => actualLines = lines.ToList());

            _testSubject.LogThisData(expectedTitle, expectedData);

            _logFileHelperMock.VerifyAll();
            Assert.AreEqual(3, actualLines.Count);
            Assert.AreEqual("--------------------------------------------------", actualLines[0]);
            Assert.AreEqual("My title at 2022-03-18T20:06:10.0000000Z", actualLines[1]);
            Assert.AreEqual(expectedData, actualLines[2]);
        }
    }
}
