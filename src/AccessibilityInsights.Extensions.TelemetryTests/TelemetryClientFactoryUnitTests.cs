// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.Extensions.TelemetryTests
{
    [TestClass]
    public class TelemetryClientFactoryUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void Ctor_CreatesObjectWithExpectedProperties()
        {
            TelemetryConfiguration config = new TelemetryConfiguration();
            string expectedVersion = OSHelpers.GetVersion();

            TelemetryClient client = TelemetryClientFactory.GetClient(config);

            Assert.AreSame(config, client.TelemetryConfiguration);
            Assert.AreEqual(expectedVersion, client.Context.Device.OperatingSystem);
            Assert.AreEqual("undefined", client.Context.Cloud.RoleInstance);
        }
    }
}
