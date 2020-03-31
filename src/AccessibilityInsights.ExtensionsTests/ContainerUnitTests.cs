// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.ExtensionsTests.DummyClasses;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using System;
using System.Reflection;
using System.IO;

namespace AccessibilityInsights.ExtensionsTests
{
    [TestClass]
    public class ContainerUnitTests
    {
        const string NoMatchOverride = "this_file_does_not_exist.dll";
        static string MatchOverride = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

        [TestMethod]
        [Timeout(1000)]
        public void AutoUpdate_NoMatch_ReturnsNull()
        {
            using (Container container = new Container(NoMatchOverride))
            {
                Assert.IsNull(container.AutoUpdate);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void AutoUpdate_Match_ReturnsCorrectType()
        {
            using (Container container = new Container(MatchOverride))
            {
                IAutoUpdate autoUpdate = container.AutoUpdate;
                Assert.IsNotNull(autoUpdate);
                Assert.IsInstanceOfType(autoUpdate, typeof(DummyAutoUpdate));
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void Telemetry_NoMatch_ReturnsNull()
        {
            using (Container container = new Container(NoMatchOverride))
            {
                Assert.IsNull(container.Telemetry);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void Telemetry_Match_ReturnsCorrectType()
        {
            using (Container container = new Container(MatchOverride))
            {
                ITelemetry telemetry = container.Telemetry;
                Assert.IsNotNull(telemetry);
                Assert.IsInstanceOfType(telemetry, typeof(DummyTelemetry));
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IssueReportingOptions_NoMatch_ReturnsNull()
        {
            using (Container container = new Container(NoMatchOverride))
            {
                IEnumerable<IIssueReporting> reportingOptions = container.IssueReportingOptions;
                Assert.IsNotNull(reportingOptions);
                Assert.IsFalse(reportingOptions.Any());
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void IssueReportingOptions_Match_ReturnsCorrectTypes()
        {
            using (Container container = new Container(MatchOverride))
            {
                IEnumerable<IIssueReporting> reportingOptions = container.IssueReportingOptions;
                Assert.IsNotNull(reportingOptions);
                List<IIssueReporting> optionList = reportingOptions.ToList();
                Assert.AreEqual(2, optionList.Count);
                HashSet<Type> set = new HashSet<Type>();
                set.Add(optionList[0].GetType());
                set.Add(optionList[1].GetType());

                Assert.IsTrue(set.Contains(typeof(DummyIssueReporting1)));
                Assert.IsTrue(set.Contains(typeof(DummyIssueReporting2)));
            }
        }
    }
}
