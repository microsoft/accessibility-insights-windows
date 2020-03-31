// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;

namespace AccessibilityInsights.ExtensionsTests
{
    [TestClass]
    public class ContainerUnitTests
    {
        private class TestContainer : Container
        {
            public TestContainer() : base() { }
        }

        [TestMethod]
        [Timeout(1000)]
        public void AutoUpdate_ReturnsNull()
        {
            using (Container container = new TestContainer())
            {
                Assert.IsNull(container.AutoUpdate);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void Telemetry_ReturnsNull()
        {
            using (Container container = new TestContainer())
            {
                Assert.IsNull(container.Telemetry);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IssueReportingOptions_ReturnsNull()
        {
            using (Container container = new TestContainer())
            {
                IEnumerable<IIssueReporting> reportingOptions = container.IssueReportingOptions;
                Assert.IsNotNull(reportingOptions);
                Assert.IsFalse(reportingOptions.Any());
            }
        }
    }
}
