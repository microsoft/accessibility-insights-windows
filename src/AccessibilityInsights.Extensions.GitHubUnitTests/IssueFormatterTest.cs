// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace AccessibilityInsights.Extensions.GitHub
{
    [TestClass]
    public class IssueFormatterTest
    {
        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNewIssue_IssueInfoIsNull_ThrowsArgumentNullException()
        {
            IssueFormatterFactory.GetNewIssueLink(string.Empty, null);
        }

        [TestMethod]
        public void IssueFormattingTest()
        {
            string link = "https://github.com/bla/bla-blas";
            IssueInformation singleFailureIssue = new IssueInformation(null, "glimpse", null,
                new Uri("https://www.helpUri.com"), "ruleSource", "ruleDescription",
                "testMessages", null, null,
                "elementPath", null, null,
                "processName", IssueType.SingleFailure, null);

            IssueInformation noFailureIssue = new IssueInformation(null, "glimpse", null,
               new Uri("https://www.helpUri.com"), "ruleSource", "ruleDescription",
               "testMessages", null, null,
               "elementPath", null, null,
               "processName", IssueType.NoFailure, null);

            IIssueFormatter singleIssueFormatter = new SingleFailureIssueFormatter(singleFailureIssue);
            IIssueFormatter noIssueFormatter = new NoFailuresIssueFormatter(noFailureIssue);

            string singleFailureLinkActual = IssueFormatterFactory.GetNewIssueLink(link, singleFailureIssue);
            string singleFailureLinkExpected = GetLink(link, singleIssueFormatter);
            string noFailureLinkActual = IssueFormatterFactory.GetNewIssueLink(link, noFailureIssue);
            string noFailureLinkExpected = GetLink(link, noIssueFormatter);

            Assert.AreEqual(singleFailureLinkExpected, singleFailureLinkActual);
            Assert.AreEqual(noFailureLinkExpected, noFailureLinkActual);
            Assert.AreNotEqual(singleFailureLinkExpected, noFailureLinkActual);
            Assert.AreNotEqual(noFailureLinkExpected, singleFailureLinkActual);
        }

        private string GetLink(string link, IIssueFormatter formatter)
        {
            string FormattedURL = string.Format(CultureInfo.InvariantCulture, Properties.Resources.FormattedLink,
               link,
               formatter.GetFormattedTitle(),
               formatter.GetFormattedBody());
            string escapedURL = Uri.EscapeUriString(FormattedURL).Replace("#", "%23");

            return escapedURL;
        }
    }
}
