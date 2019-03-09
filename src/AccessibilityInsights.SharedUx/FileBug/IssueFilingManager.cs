using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.FileBug
{
    internal class IssueReporterManager
    {
        public static Guid SelectedIssueReporterGuid { get; set; }
        readonly static object _lockObject = new object();
        static Dictionary<Guid, IIssueReporting> IssueReportingOptionsDict = new Dictionary<Guid, IIssueReporting>();
        private static IssueReporterManager _defaultInstance = null;

        public static IssueReporterManager GetInstance()
        {
            if (_defaultInstance == null)
            {
                lock (_lockObject)
                {
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = new IssueReporterManager();
                    }
                }
            }
            return _defaultInstance;
        }

        private IssueReporterManager()
        {
            List<IIssueReporting> IssueReportingOptions = Container.GetDefaultInstance().IssueReporting;
            foreach (IIssueReporting issueReporter in IssueReportingOptions) {
                try
                {
                    if (issueReporter != null)
                        IssueReportingOptionsDict.Add(issueReporter.StableIdentifier, issueReporter);
                }
                catch (ArgumentException ex) {
                    // Fail silently in case of dups.
                    Console.WriteLine("Found duplicate extensions" + ex.StackTrace);
                }
            }
        }

        public static IReadOnlyDictionary<Guid, IIssueReporting> getIssueFilingOptionsDict() {
            return IssueReportingOptionsDict;
        }
    }

}
