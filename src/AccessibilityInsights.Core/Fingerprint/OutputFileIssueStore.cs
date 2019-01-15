// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Core.Results;
using System;
using System.Collections.Generic;

using static System.FormattableString;

namespace AccessibilityInsights.Core.Fingerprint
{
    /// <summary>
    /// Implementation of IIssueStore to manage issues found within a results file
    /// </summary>
    public class OutputFileIssueStore : IIssueStore
    {
        private readonly IReadOnlyDictionary<IFingerprint, Issue> _store;

        /// <summary>
        /// Can this store enumerate contents? (Yes)
        /// </summary>
        public bool IsEnumerable => true;

        /// <summary>
        /// Can this store be updated? (No)
        /// </summary>
        public bool IsUpdatable => false;

        /// <summary>
        /// The Issues currently in the store. Use this only to enumerate; use TryFindIssue to search.
        /// The Issues are unsorted, and may be provided in any order
        /// </summary>
        public IEnumerable<Issue> Issues => _store.Values;

        /// <summary>
        /// Add an Issue to the store
        /// </summary>
        /// <param name="issue">The issue to add</param>
        /// <returns>The result of the operation (AddResult.NotSupported)</returns>
        public AddResult AddIssue(Issue issue)
        {
            issue.ArgumentIsNotNull(nameof(issue));
            return AddResult.NotSupported;
        }

        /// <summary>
        /// Search the results for an issue with the specified fingerprint
        /// </summary>
        /// <param name="fingerprint">The fingerprint we're trying to find</param>
        /// <param name="issue">The issue, if it exists (null if not)</param>
        /// <returns>true iff a matching issue was located and returned</returns>
        public bool TryFindIssue(IFingerprint fingerprint, out Issue issue)
        {
            fingerprint.ArgumentIsNotNull(nameof(fingerprint));

            return _store.TryGetValue(fingerprint, out issue);
        }

        /// <summary>
        /// ctor (production)
        /// </summary>
        /// <param name="fileName">Path to the file that provided the elements</param>
        /// <param name="elementSet">Elements from the provided file</param>
        public OutputFileIssueStore(string fileName, IEnumerable<A11yElement> elementSet)
            : this(fileName, elementSet, ExtractIssues)
        {
        }

        /// <summary>
        /// ctor (unit tests only)
        /// </summary>
        /// <param name="fileName">Path to the file that provided the elements</param>
        /// <param name="elementSet">Elements from the provided file</param>
        /// <param name="extractor">The action that populates the internal store from the provided elements</param>
        internal OutputFileIssueStore(string fileName, IEnumerable<A11yElement> elementSet, Action<string, IEnumerable<A11yElement>, IDictionary<IFingerprint, Issue>> extractor)
        {
            extractor.ArgumentIsNotNull(nameof(extractor));

            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            // Populate the store
            extractor(fileName, elementSet, store);
            _store = store;
        }

        /// <summary>
        /// Convert the provided elements to issues for the store. In a separate method to assist with unit testing
        /// </summary>
        /// <param name="fileName">Path to the file that provided the elements</param>
        /// <param name="elementSet">Elements from the provided file</param>
        /// <param name="store">The list of elements that we need to update</param>
        internal static void ExtractIssues(string fileName, IEnumerable<A11yElement> elementSet, IDictionary<IFingerprint, Issue> store)
        {
            fileName.ArgumentIsNotTrivialString(nameof(fileName));
            elementSet.ArgumentIsNotNull(nameof(elementSet));

            foreach (A11yElement element in elementSet)
            {
                if (element.ScanResults == null)
                    continue;

                ScanStatus status = element.ScanResults.Status;

                // Include only elements with failures or uncertains
                if (status != ScanStatus.Fail && status != ScanStatus.Uncertain)
                    continue;

                foreach (ScanResult scanResults in element.ScanResults.Items)
                {
                    foreach (RuleResult ruleResult in scanResults.Items)
                    {
                        // Update the issue store--duplicate fingerprints are possible
                        // with some UIA trees
                        IFingerprint fingerprint = BuildFingerprint(element, ruleResult.Rule, ruleResult.Status);
                        ILocation location = BuildLocation(element, fileName);

                        if (!store.TryGetValue(fingerprint, out Issue issue))
                        {
                            store.Add(fingerprint, issue = BuildIssue(SynthesizeIssueType(ruleResult.Rule, ruleResult.Status), fingerprint));
                        }
                        issue.AddLocation(location);
                    }
                }
            }
        }

        private static string SynthesizeIssueType(RuleId ruleId, ScanStatus status)
        {
            return Invariant($"{ruleId}_{status}");
        }

        // Simple factory method. Pulled out so that we can easily shim this for testing
        private static IFingerprint BuildFingerprint(A11yElement element, RuleId ruleId, ScanStatus status)
        {
            return new ScanResultFingerprint(element, ruleId, status);
        }

        // Simple factory method. Pulled out so that we can easily shim this for testing
        private static Issue BuildIssue(string issueType, IFingerprint fingerprint)
        {
            return new Issue(fingerprint, issueType);
        }

        // Simple factory method. Pulled out so that we can easily shim this for testing
        private static ILocation BuildLocation(A11yElement element, string fileName)
        {
            return new OutputFileLocation(fileName, element);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
         }
        #endregion
    }
}
