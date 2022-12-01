// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.Extensions.DiskLoggingTelemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetryTests
{
    [TestClass]
    public class LocalTelemetryUnitTests
    {
        private Mock<ILogWriter> _logWriterMock;
        private LocalTelemetry _testSubject;

        private const string EventName = "Some event name";
        private const string PublishEventTitle = "Event was published";
        private const string ReportExceptionTitle = "Exception was reported";
        private const string TestExceptionMessage = "This is the test exception";
        private readonly IReadOnlyDictionary<string, string> EventProperties
            = new Dictionary<string, string>
            {
                { "Event Property 1", "Event Value 1" },
                { "Event Property 2", "Event Value 2" },
            };
        private readonly Exception TestException = new OutOfMemoryException(TestExceptionMessage);

        private readonly IReadOnlyDictionary<string, string> ContextProperties
            = new Dictionary<string, string>
            {
                { "Context Property 1", "Context Value 1" },
                { "Context Property 2", "Context Value 2" },
                { "Context Property 3", "Context Value 3" },
            };

        private void SetContextProperties()
        {
            foreach (KeyValuePair<string, string> pair in ContextProperties)
            {
                _testSubject.AddOrUpdateContextProperty(pair.Key, pair.Value);
            }
        }

        private static ReportedExceptionData ExceptionDataFromJson(string json)
        {
            return JsonConvert.DeserializeObject<ReportedExceptionData>(json);
        }

        private static PublishedEventData EventDataFromJson(string json)
        {
            return JsonConvert.DeserializeObject<PublishedEventData>(json);
        }

        private static void AssertDictionariesAreEquivalent(
            IReadOnlyDictionary<string, string> dictionary1,
            IReadOnlyDictionary<string, string> dictionary2
            )
        {
            Assert.IsNotNull(dictionary1);
            Assert.IsNotNull(dictionary2);
            Assert.AreEqual(dictionary1.Count, dictionary2.Count);
            foreach (KeyValuePair<string, string> pairFrom1 in dictionary1)
            {
                Assert.AreEqual(pairFrom1.Value, dictionary2[pairFrom1.Key]);
            }
        }

        [TestInitialize]
        public void BeforeEachTest()
        {
            _logWriterMock = new Mock<ILogWriter>(MockBehavior.Strict);
            _testSubject = new LocalTelemetry(_logWriterMock.Object);
        }

        [TestMethod]
        public void Ctor_LogWriterIsNull_ThrowsArgumentNUllException()
        {
            ArgumentNullException e = Assert.ThrowsException<ArgumentNullException>
                (() => new LocalTelemetry(null));
            Assert.AreEqual("logWriter", e.ParamName);
        }

        [TestMethod]
        public void PublishEvent_NoPropertiesAreSet_TitleIsPassedThrough()
        {
            _logWriterMock
                .Setup(x => x.LogThisData(PublishEventTitle, It.IsAny<string>()));

            _testSubject.PublishEvent(EventName, null);

            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void PublishEvent_NoPropertiesAreSet_DataIsSerializedToJson()
        {
            PublishedEventData eventData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(PublishEventTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                {
                    eventData = EventDataFromJson(data);
                });

            _testSubject.PublishEvent(EventName, null);

            Assert.IsNotNull(eventData);
            Assert.AreEqual(EventName, eventData.EventName);
            Assert.IsNull(eventData.EventProperties);
            Assert.IsFalse(eventData.ContextProperties.Any());
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void PublishEvent_EventPropertiesAreSet_DataIsSerializedToJson()
        {
            PublishedEventData eventData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(PublishEventTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                {
                    eventData = EventDataFromJson(data);
                });

            _testSubject.PublishEvent(EventName, EventProperties);

            Assert.IsNotNull(eventData);
            Assert.AreEqual(EventName, eventData.EventName);
            AssertDictionariesAreEquivalent(EventProperties, eventData.EventProperties);
            Assert.IsFalse(eventData.ContextProperties.Any());
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void PublishEvent_ContextPropertiesAreSet_DataIsSerializedToJson()
        {
            PublishedEventData eventData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(PublishEventTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                {
                    eventData = EventDataFromJson(data);
                });
            SetContextProperties();

            _testSubject.PublishEvent(EventName, null);

            Assert.IsNotNull(eventData);
            Assert.AreEqual(EventName, eventData.EventName);
            Assert.IsNull(eventData.EventProperties);
            AssertDictionariesAreEquivalent(ContextProperties, eventData.ContextProperties);
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void PublishEvent_ContextPropertiesAreSetTwice_DataIsSerializedToJson()
        {
            PublishedEventData eventData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(PublishEventTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                {
                    eventData = EventDataFromJson(data);
                });
            SetContextProperties();
            SetContextProperties();

            _testSubject.PublishEvent(EventName, null);

            Assert.IsNotNull(eventData);
            Assert.AreEqual(EventName, eventData.EventName);
            Assert.IsNull(eventData.EventProperties);
            AssertDictionariesAreEquivalent(ContextProperties, eventData.ContextProperties);
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void PublishEvent_EventPropertiesAndContextPropertiesAreSet_DataIsSerializedToJson()
        {
            PublishedEventData eventData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(PublishEventTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                {
                    eventData = EventDataFromJson(data);
                });
            SetContextProperties();

            _testSubject.PublishEvent(EventName, EventProperties);

            Assert.IsNotNull(eventData);
            Assert.AreEqual(EventName, eventData.EventName);
            AssertDictionariesAreEquivalent(EventProperties, eventData.EventProperties);
            AssertDictionariesAreEquivalent(ContextProperties, eventData.ContextProperties);
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void ReportException_NoPropertiesAreSet_TitleIsPassedThrough()
        {
            _logWriterMock
                .Setup(x => x.LogThisData(ReportExceptionTitle, It.IsAny<string>()));

            _testSubject.ReportException(TestException);

            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void ReportException_NoPropertiesAreSet_DataIsSerializedToJson()
        {
            ReportedExceptionData exceptionData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(ReportExceptionTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                 {
                     exceptionData = ExceptionDataFromJson(data);
                 });

            _testSubject.ReportException(TestException);

            Assert.IsNotNull(exceptionData);
            Assert.AreEqual(TestExceptionMessage, exceptionData.Exception.Message);
            Assert.IsFalse(exceptionData.ContextProperties.Any());
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void ReportException_ContextPropertiesAreSet_DataIsSerializedToJson()
        {
            ReportedExceptionData exceptionData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(ReportExceptionTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                {
                    exceptionData = ExceptionDataFromJson(data);
                });
            SetContextProperties();

            _testSubject.ReportException(TestException);

            Assert.IsNotNull(exceptionData);
            Assert.AreEqual(TestExceptionMessage, exceptionData.Exception.Message);
            AssertDictionariesAreEquivalent(ContextProperties, exceptionData.ContextProperties);
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void ReportException_ContextPropertiesAreSetTwice_DataIsSerializedToJson()
        {
            ReportedExceptionData exceptionData = null;
            _logWriterMock
                .Setup(x => x.LogThisData(ReportExceptionTitle, It.IsAny<string>()))
                .Callback<string, string>((_, data) =>
                {
                    exceptionData = ExceptionDataFromJson(data);
                });
            SetContextProperties();
            SetContextProperties();

            _testSubject.ReportException(TestException);

            Assert.IsNotNull(exceptionData);
            Assert.AreEqual(TestExceptionMessage, exceptionData.Exception.Message);
            AssertDictionariesAreEquivalent(ContextProperties, exceptionData.ContextProperties);
            _logWriterMock.VerifyAll();
        }

        [TestMethod]
        public void FlushAndShutDown_DoesNothing()
        {
            _testSubject.FlushAndShutDown();
            _logWriterMock.VerifyAll();
        }
    }
}
