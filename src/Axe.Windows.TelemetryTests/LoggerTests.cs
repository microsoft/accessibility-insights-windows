// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Axe.Windows.TelemetryTests
{
    [TestClass]
    public class LoggerTests
    {
        [TestInitialize]
        public void TestSetup()
        {
            Logger.SetTelemetrySink(null);
        }

        [TestMethod]
        public void PublishTelemetryEvent_SingleProp_TelemetryEnabled()
        {
            var m = new Mock<IAxeWindowsTelemetry>(MockBehavior.Strict);
            m.Setup(x => x.PublishEvent(It.IsNotNull<string>(), It.IsNotNull<IReadOnlyDictionary<string, string>>()));
            m.Setup(x => x.IsEnabled).Returns(true);

            Logger.SetTelemetrySink(m.Object);
            Logger.PublishTelemetryEvent(TelemetryAction.Scan_Statistics, TelemetryProperty.ElementsInScan, "some value");

            m.VerifyAll();
        }

        [TestMethod]
        public void PublishTelemetryEvent_SingleProp_TelemetryDisabled()
        {
            var m = new Mock<IAxeWindowsTelemetry>(MockBehavior.Strict);
            m.Setup(x => x.PublishEvent(It.IsNotNull<string>(), It.IsNotNull<IReadOnlyDictionary<string, string>>()));
            m.Setup(x => x.IsEnabled).Returns(false);

            Logger.SetTelemetrySink(m.Object);
            Logger.PublishTelemetryEvent(TelemetryAction.Scan_Statistics, TelemetryProperty.ElementsInScan, "some value");

            m.VerifyGet(x => x.IsEnabled, Times.AtLeastOnce());
            m.Verify(x => x.PublishEvent(It.IsNotNull<string>(), It.IsNotNull<IReadOnlyDictionary<string, string>>()), Times.Never());
        }

        [TestMethod]
        public void PublishTelemetryEvent_SingleProp_NoTelemetry_NoException()
        {
            Logger.PublishTelemetryEvent(TelemetryAction.Scan_Statistics, TelemetryProperty.ElementsInScan, "some value");
        }

        [TestMethod]
        public void PublishTelemetryEvent_PropDictionary_TelemetryEnabled()
        {
            var m = new Mock<IAxeWindowsTelemetry>(MockBehavior.Strict);
            m.Setup(x => x.PublishEvent(It.IsNotNull<string>(), It.IsNotNull<IReadOnlyDictionary<string, string>>()));
            m.Setup(x => x.IsEnabled).Returns(true);

            Logger.SetTelemetrySink(m.Object);
            var propertyBag = new Dictionary<TelemetryProperty, string>
            {
                { TelemetryProperty.ElementsInScan, "some value" },
                { TelemetryProperty.UIFramework, "Who knows"  },
            };

            Logger.PublishTelemetryEvent(TelemetryAction.Scan_Statistics, propertyBag);

            m.VerifyAll();
        }

        [TestMethod]
        public void PublishTelemetryEvent_PropDictionary_TelemetryDisabled()
        {
            var m = new Mock<IAxeWindowsTelemetry>(MockBehavior.Strict);
            m.Setup(x => x.PublishEvent(It.IsNotNull<string>(), It.IsNotNull<IReadOnlyDictionary<string, string>>()));
            m.Setup(x => x.IsEnabled).Returns(false);

            Logger.SetTelemetrySink(m.Object);
            var propertyBag = new Dictionary<TelemetryProperty, string>
            {
                { TelemetryProperty.ElementsInScan, "some value" },
                { TelemetryProperty.UIFramework, "Who knows"  },
            };

            Logger.PublishTelemetryEvent(TelemetryAction.Scan_Statistics, propertyBag);

            m.VerifyGet(x => x.IsEnabled, Times.AtLeastOnce());
            m.Verify(x => x.PublishEvent(It.IsNotNull<string>(), It.IsNotNull<IReadOnlyDictionary<string, string>>()), Times.Never());
        }

        [TestMethod]
        public void PublishTelemetryEvent_PropDictionary_NoTelemetry_NoException()
        {
            var propertyBag = new Dictionary<TelemetryProperty, string>
            {
                { TelemetryProperty.ElementsInScan, "some value" },
                { TelemetryProperty.UIFramework, "Who knows"  },
            };

            Logger.PublishTelemetryEvent(TelemetryAction.Scan_Statistics, propertyBag);
        }

        [TestMethod]
        public void ReportException_TelemetryCalled()
        {
            var m = new Mock<IAxeWindowsTelemetry>(MockBehavior.Strict);
            m.Setup(x => x.ReportException(It.IsNotNull<Exception>()));

            Logger.SetTelemetrySink(m.Object);
            Logger.ReportException(new Exception());

            m.VerifyAll();
        }

        [TestMethod]
        public void ReportException_ParamNull()
        {
            var m = new Mock<IAxeWindowsTelemetry>(MockBehavior.Strict);
            m.Setup(x => x.ReportException(It.IsNotNull<Exception>()));

            Logger.SetTelemetrySink(m.Object);
            Logger.ReportException(null);

            m.Verify(x => x.ReportException(It.IsNotNull<Exception>()), Times.Never());
        }

        [TestMethod]
        public void ReportException_NoTelemetry_NoException()
        {
            Logger.ReportException(new Exception());
        }
    } // class
} // namespace
