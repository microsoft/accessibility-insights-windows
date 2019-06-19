// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
#if FAKES_SUPPORTED
using AccessibilityInsights.SharedUx.Telemetry.Fakes;
using AccessibilityInsights.Extensions.Interfaces.Telemetry.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace AccessibilityInsights.SharedUXTests.Telemetry
{
    [TestClass]
    public class LoggerUnitTests
    {
#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_ImplementationIsNull_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                ShimTelemetrySink.TelemetryGet = () => null;

                Assert.IsFalse(Logger.IsEnabled);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_ImplementationIsNotNull_IsTelemetryAllowedGetIsTrue_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                ShimTelemetrySink.TelemetryGet = () => new StubITelemetry();
                TelemetrySink.IsTelemetryAllowed = true;

                Assert.IsTrue(Logger.IsEnabled);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_ImplementationIsNotNull_IsTelemetryAllowedGetIsFalse_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                ShimTelemetrySink.TelemetryGet = () => new StubITelemetry();
                TelemetrySink.IsTelemetryAllowed = false;

                Assert.IsFalse(Logger.IsEnabled);
            }
        }
#endif

        [TestMethod]
        [Timeout(1000)]
        public void ConvertFromProperties_InputIsNull_OutputIsNull()
        {
            Assert.IsNull(Logger.ConvertFromProperties(null));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ConvertFromProperties_InputIsTrivial_OutputIsNull()
        {
            Assert.IsNull(Logger.ConvertFromProperties(new Dictionary<TelemetryProperty, string>()));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ConvertFromProperties_InputIsNontrivial_OutputIsCorrect()
        {
            const string value1 = "January";
            const string value2 = "February";
            const string value3 = "March";

            // Specific values of TelemetryProperty are unimportant for this test
            Dictionary<TelemetryProperty, string> input = new Dictionary<TelemetryProperty, string>
            {
                { (TelemetryProperty)1, value1 },
                { (TelemetryProperty)2, value2 },
                { (TelemetryProperty)3, value3 },
            };

            IReadOnlyDictionary<string, string> output = Logger.ConvertFromProperties(input);

            Assert.AreEqual(input.Count, output.Count);
            foreach (KeyValuePair<TelemetryProperty, string> pair in input)
            {
                Assert.AreEqual(pair.Value, output[pair.Key.ToString()]);
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemetryIsNotEnabled_DoesNothing()
        {
            using (ShimsContext.Create())
            {
                TelemetrySink.IsTelemetryAllowed = false;

                // Specific values of TelemetryAction and TelemetryProperty are unimportant for this test
                Logger.PublishTelemetryEvent((TelemetryAction)0, (TelemetryProperty)0, "abc");
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemteryIsEnabled_EnablesAndChainsThrough()
        {
            using (ShimsContext.Create())
            {
                // Specific values of TelemetryAction and TelemetryProperty are unimportant for this test
                const TelemetryAction action = (TelemetryAction)4;
                const TelemetryProperty property = (TelemetryProperty)5;
                const string value = "Friday";
                string actualName = null;
                IReadOnlyDictionary<string, string> actualTelemetryPropertyBag = null;
                ITelemetry telemetry = new StubITelemetry
                {
                    PublishEventStringIReadOnlyDictionaryOfStringString = (name, telemetryPropertyBag) =>
                    {
                        actualName = name;
                        actualTelemetryPropertyBag = telemetryPropertyBag;
                    }
                };
                TelemetrySink.IsTelemetryAllowed = true;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                Logger.PublishTelemetryEvent(action, property, value);

                Assert.AreEqual(action.ToString(), actualName);
                Assert.AreEqual(1, actualTelemetryPropertyBag.Count);
                Assert.AreEqual(value, actualTelemetryPropertyBag[property.ToString()]);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEventContainer_TelemetryIsEnabled_EnablesAndChainsThrough()
        {
            using (ShimsContext.Create())
            {
                // Specific values of TelemetryAction and TelemetryProperty are unimportant for this test
                TelemetryAction action = (TelemetryAction)6;
                TelemetryProperty property = (TelemetryProperty)7;
                var fakeId = "id";
                var fakeEvent = new TelemetryEvent(action, new Dictionary<TelemetryProperty, string>
                {
                    { property, fakeId },
                });

                string actualName = null;
                IReadOnlyDictionary<string, string> actualTelemetryPropertyBag = null;
                ITelemetry telemetry = new StubITelemetry
                {
                    PublishEventStringIReadOnlyDictionaryOfStringString = (name, telemetryPropertyBag) =>
                    {
                        actualName = name;
                        actualTelemetryPropertyBag = telemetryPropertyBag;
                    }
                };
                TelemetrySink.IsTelemetryAllowed = true;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                Logger.PublishTelemetryEvent(fakeEvent);

                Assert.AreEqual(fakeEvent.Action.ToString(), actualName);
                Assert.AreEqual(1, actualTelemetryPropertyBag.Count);
                Assert.AreEqual(fakeId, actualTelemetryPropertyBag[property.ToString()]);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_MultiProperty_TelemteryIsNotEnabled_DoesNothing()
        {
            using (ShimsContext.Create())
            {
                TelemetrySink.IsTelemetryAllowed = false;

                // Specific value of TelemetryAction is unimportant for this test
                Logger.PublishTelemetryEvent((TelemetryAction)7, null);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_MultiProperty_TelemteryIsEnabled_EnablesAndChainsThrough()
        {
            using (ShimsContext.Create())
            {
                // Specific values of TelemetryAction and TelemetryProperty are unimportant for this test
                const TelemetryAction action = (TelemetryAction)8;
                const TelemetryProperty property = (TelemetryProperty)9;
                const string value = "Saturday";
                string actualName = null;
                IReadOnlyDictionary<TelemetryProperty, string> actualConverterInput = null;
                IReadOnlyDictionary<string, string> actualTelemetryPropertyBag = null;
                ITelemetry telemetry = new StubITelemetry
                {
                    PublishEventStringIReadOnlyDictionaryOfStringString = (name, telemetryPropertyBag) =>
                    {
                        actualName = name;
                        actualTelemetryPropertyBag = telemetryPropertyBag;
                    }
                };
                TelemetrySink.IsTelemetryAllowed = true;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                Dictionary<string, string> expectedConverterOutput = new Dictionary<string, string>
                {
                    { "abc", "def" },
                };
                ShimLogger.ConvertFromPropertiesIReadOnlyDictionaryOfTelemetryPropertyString = (input) =>
                {
                    actualConverterInput = input;
                    return expectedConverterOutput;
                };

                Dictionary<TelemetryProperty, string> initalInput = new Dictionary<TelemetryProperty, string>
                {
                    { property, value },
                };

                Logger.PublishTelemetryEvent(action, initalInput);

                Assert.AreEqual(action.ToString(), actualName);
                Assert.AreEqual(initalInput, actualConverterInput);
                Assert.AreSame(expectedConverterOutput, actualTelemetryPropertyBag);
                Assert.AreEqual(action.ToString(), actualName);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_TelemetryIsNotEnabled_DoesNothing()
        {
            using (ShimsContext.Create())
            {
                TelemetrySink.IsTelemetryAllowed = false;
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_TelemetryIsEnabled_DoesNothing()
        {
            using (ShimsContext.Create())
            {
                // Specific value of TelemetryProperty is unimportant for this test
                const TelemetryProperty expectedProperty = (TelemetryProperty)9;
                const string expectedValue = "carrot";

                string actualProperty = null;
                string actualValue = null;
                StubITelemetry telemetry = new StubITelemetry
                {
                    AddOrUpdateContextPropertyStringString = (property, value) =>
                    {
                        actualProperty = property;
                        actualValue = value;
                    }
                };
                TelemetrySink.IsTelemetryAllowed = true;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                Logger.AddOrUpdateContextProperty(expectedProperty, expectedValue);

                Assert.AreEqual(expectedProperty.ToString(), actualProperty);
                Assert.AreEqual(expectedValue, actualValue);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_IsNotEnabled_DoesNotChainThrough()
        {
            using (ShimsContext.Create())
            {
                Exception expectedException = new Exception("blah");

                StubITelemetry telemetry = new StubITelemetry
                {
                    ReportExceptionException = (e) =>
                    {
                        Assert.Fail("This should not be called");
                    }
                };

                TelemetrySink.IsTelemetryAllowed = false;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                Logger.ReportException(expectedException);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_IsEnabled_ExceptionIsNull_DoesNotChainThrough()
        {
            using (ShimsContext.Create())
            {
                Exception expectedException = null;
                Exception actualException = null;

                StubITelemetry telemetry = new StubITelemetry
                {
                    ReportExceptionException = (e) =>
                    {
                        Assert.Fail("This should not be called");
                    }
                };

                TelemetrySink.IsTelemetryAllowed = true;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                Logger.ReportException(expectedException);
                Assert.IsNull(actualException);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_IsEnabled_ExceptionIsNotNull_ChainsThrough()
        {
            using (ShimsContext.Create())
            {
                Exception expectedException = new Exception("blah");
                Exception actualException = null;

                StubITelemetry telemetry = new StubITelemetry
                {
                    ReportExceptionException = (e) =>
                    {
                        actualException = e;
                    }
                };

                TelemetrySink.IsTelemetryAllowed = true;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                Logger.ReportException(expectedException);
                Assert.AreSame(expectedException, actualException);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_CalledAsExtension_IsEnabled_ExceptionIsNotNull_ChainsThrough()
        {
            using (ShimsContext.Create())
            {
                Exception expectedException = new Exception("blah");
                Exception actualException = null;

                StubITelemetry telemetry = new StubITelemetry
                {
                    ReportExceptionException = (e) =>
                    {
                        actualException = e;
                    }
                };

                TelemetrySink.IsTelemetryAllowed = true;
                ShimTelemetrySink.TelemetryGet = () => telemetry;

                expectedException.ReportException();
                Assert.AreSame(expectedException, actualException);
            }
        }
#endif
    }
}
