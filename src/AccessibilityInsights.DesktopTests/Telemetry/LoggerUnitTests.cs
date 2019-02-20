// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.Desktop.Telemetry.Fakes;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using AccessibilityInsights.Extensions.Interfaces.Telemetry.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.DesktopTests.Telemetry
{
    [TestClass]
    public class LoggerUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_ImplementationIsNull_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                ShimLogger.TelemetryGet = () => null;

                Assert.IsFalse(Logger.IsEnabled);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsTelemetryAllowed_ImplementationIsNull_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                Assert.IsFalse(Logger.IsTelemetryAllowed);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_ImplementationIsNotNull_IsTelemetryAllowedGetIsTrue_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                ShimLogger.TelemetryGet = () => new StubITelemetry();
                ShimLogger.IsTelemetryAllowedGet = () => true;

                Assert.IsTrue(Logger.IsEnabled);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_ImplementationIsNotNull_IsTelemetryAllowedGetIsFalse_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                ShimLogger.TelemetryGet = () => new StubITelemetry();
                ShimLogger.IsTelemetryAllowedGet = () => false;

                Assert.IsFalse(Logger.IsEnabled);
            }
        }

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

            Dictionary<TelemetryProperty, string> input = new Dictionary<TelemetryProperty, string>
            {
                { TelemetryProperty.AppSessionID, value1 },
                { TelemetryProperty.HandledException, value2 },
                { TelemetryProperty.SessionType, value3 },
            };

            IReadOnlyDictionary<string, string> output = Logger.ConvertFromProperties(input);

            Assert.AreEqual(input.Count, output.Count);
            foreach (KeyValuePair<TelemetryProperty, string> pair in input)
            {
                Assert.AreEqual(pair.Value, output[pair.Key.ToString()]);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemetryIsNotEnabled_DoesNothing()
        {
            using (ShimsContext.Create())
            {
                ShimLogger.IsEnabledGet = () => false;

                // TelemetryAction used here is arbitrary
                Logger.PublishTelemetryEvent(TelemetryAction.Bug_Cancel,
                    TelemetryProperty.By, "abc");
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemteryIsEnabled_EnablesAndChainsThrough()
        {
            using (ShimsContext.Create())
            {
                // TelemetryAction used here is arbitrary
                const TelemetryAction action = TelemetryAction.ColorContrast_Click_Eyedropper;
                const TelemetryProperty property = TelemetryProperty.Comment;
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
                ShimLogger.IsEnabledGet = () => true;
                ShimLogger.TelemetryGet = () => telemetry;

                Logger.PublishTelemetryEvent(action, property, value);

                Assert.AreEqual(action.ToString(), actualName);
                Assert.AreEqual(1, actualTelemetryPropertyBag.Count);
                Assert.AreEqual(value, actualTelemetryPropertyBag[property.ToString()]);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_MultiProperty_TelemteryIsNotEnabled_DoesNothing()
        {
            using (ShimsContext.Create())
            {
                ShimLogger.IsEnabledGet = () => false;

                // TelemetryAction used here is arbitrary
                Logger.PublishTelemetryEvent(TelemetryAction.ColorContrast_Click_Dropdown, null);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_MultiProperty_TelemteryIsEnabled_EnablesAndChainsThrough()
        {
            using (ShimsContext.Create())
            {
                // TelemetryAction used here is arbitrary
                const TelemetryAction action = TelemetryAction.ColorContrast_Click_Eyedropper;
                const TelemetryProperty property = TelemetryProperty.Error;
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
                ShimLogger.IsEnabledGet = () => true;
                ShimLogger.TelemetryGet = () => telemetry;

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
                ShimLogger.IsEnabledGet = () => false;
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_TelemetryIsEnabled_DoesNothing()
        {
            using (ShimsContext.Create())
            {
                const TelemetryProperty expectedProperty = TelemetryProperty.Comment;
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
                ShimLogger.IsEnabledGet = () => true;
                ShimLogger.TelemetryGet = () => telemetry;

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

                ShimLogger.IsEnabledGet = () => false;
                ShimLogger.TelemetryGet = () => telemetry;

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

                ShimLogger.IsEnabledGet = () => true;
                ShimLogger.TelemetryGet = () => telemetry;

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

                ShimLogger.IsEnabledGet = () => true;
                ShimLogger.TelemetryGet = () => telemetry;

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

                ShimLogger.IsEnabledGet = () => true;
                ShimLogger.TelemetryGet = () => telemetry;

                expectedException.ReportException();
                Assert.AreSame(expectedException, actualException);
            }
        }
    }
}
