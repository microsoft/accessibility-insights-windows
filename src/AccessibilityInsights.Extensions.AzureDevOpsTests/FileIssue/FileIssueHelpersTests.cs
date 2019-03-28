// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if FAKES_SUPPORTED
using AccessibilityInsights.Extensions.AzureDevOps.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace AccessibilityInsights.Extensions.AzureDevOpsTests.FileIssue
{
    [TestClass]
    public class FileIssueHelpersTests
    {
        static readonly Uri FAKE_SERVER_URL = new Uri("https://myaccount.visualstudio.com/");

        [TestMethod]
        [Timeout(10000)]
        public void RemoveInternalFromIssueText_MatchingTextExists()
        {
            var guid = Guid.NewGuid().ToString();
            // Internal id doesn't exist if the text is modified by user in edit pane. this scenario simulate the case. 
            string original = $"<br><br><div><hr>{guid}<hr></div>";
            string expected = "\r\n<BODY><BR><BR>\r\n<DIV></DIV></BODY>";

            Assert.AreEqual(expected, FileIssueHelpers.RemoveInternalHTML(original, guid));
        }

        [TestMethod]
        [Timeout(10000)]
        public void RemoveInternalFromIssueText_NoMatchingText()
        {
            var guid = Guid.NewGuid().ToString();

            string original = "<br><br><div><hr>should not be removed<hr></div>";
            string expected = "\r\n<BODY><BR><BR>\r\n<DIV>\r\n<HR>\r\nshould not be removed\r\n<HR>\r\n</DIV></BODY>";

            Assert.AreEqual(expected, FileIssueHelpers.RemoveInternalHTML(original, guid));
        }

#if FAKES_SUPPORTED
        [TestMethod]
        public void FileNewIssue_IsNotEnabled_ReturnsPlaceholder()
        {
            using (ShimsContext.Create())
            {
                ShimAzureDevOpsIntegration.AllInstances.ConnectedToAzureDevOpsGet = (_) => false;
                var issueInfo = new IssueInformation();
                var connInfo = new ConnectionInfo();
                var output = FileIssueHelpers.FileNewIssue(issueInfo,
                    connInfo, false, 0, (_) => { });

                Assert.IsNull(output.issueId);
                Assert.IsNotNull(output.newIssueId);
                Assert.IsTrue(string.IsNullOrEmpty(output.newIssueId));
            }
        }

        public static void SetUpShims()
        {
            ShimAzureDevOpsIntegration.AllInstances.ConnectedToAzureDevOpsGet = (_) => true;
        }
#endif
    }
}