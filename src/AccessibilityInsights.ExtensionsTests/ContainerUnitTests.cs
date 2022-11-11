// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.ExtensionsTests.DummyClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AccessibilityInsights.ExtensionsTests
{
    [TestClass]
    public class ContainerUnitTests
    {
        const string ExtensionsDoNotExistSearchPattern = null;
        static readonly string ExtensionsExistSearchPattern = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

        [TestMethod]
        [Timeout(1000)]
        public void AutoUpdate_ExtensionsDoNotExist_ReturnsNull()
        {
            using (Container container = new Container(ExtensionsDoNotExistSearchPattern))
            {
                Assert.IsNull(container.AutoUpdate);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void AutoUpdate_ExtensionsExist_ReturnsCorrectType()
        {
            using (Container container = new Container(ExtensionsExistSearchPattern))
            {
                IAutoUpdate autoUpdate = container.AutoUpdate;
                Assert.IsNotNull(autoUpdate);
                Assert.IsInstanceOfType(autoUpdate, typeof(DummyAutoUpdate));
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void Telemetry_ExtensionsDoNotExist_ReturnsEmptySet()
        {
            using (Container container = new Container(ExtensionsDoNotExistSearchPattern))
            {
                Assert.IsFalse(container.TelemetryClasses.Any());
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void Telemetry_ExtensionsExist_ReturnsCorrectType()
        {
            using (Container container = new Container(ExtensionsExistSearchPattern))
            {
                List<ITelemetry> telemetry = container.TelemetryClasses.ToList();
                Assert.AreEqual(1, telemetry.Count);
                Assert.IsInstanceOfType(telemetry[0], typeof(DummyTelemetry));
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IssueReportingOptions_ExtensionsDoNotExist_ReturnsNull()
        {
            using (Container container = new Container(ExtensionsDoNotExistSearchPattern))
            {
                IEnumerable<IIssueReporting> reportingOptions = container.IssueReportingOptions;
                Assert.IsNotNull(reportingOptions);
                Assert.IsFalse(reportingOptions.Any());
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IssueReportingOptions_ExtensionsExist_ReturnsCorrectTypes()
        {
            using (Container container = new Container(ExtensionsExistSearchPattern))
            {
                IEnumerable<IIssueReporting> reportingOptions = container.IssueReportingOptions;
                Assert.IsNotNull(reportingOptions);
                List<IIssueReporting> optionList = reportingOptions.ToList();
                Assert.AreEqual(2, optionList.Count);
                HashSet<Type> set = new HashSet<Type>
                {
                    optionList[0].GetType(),
                    optionList[1].GetType(),
                };

                Assert.IsTrue(set.Contains(typeof(DummyIssueReporting1)));
                Assert.IsTrue(set.Contains(typeof(DummyIssueReporting2)));
            }
        }
    }
}
