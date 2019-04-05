// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Axe.Windows.Core.Misc;

namespace Axe.Windows.Core.Fingerprint
{
    /// <summary>
    /// In-memory implementation of IIssueStore
    /// </summary>
    internal class InMemoryIssueStore : IIssueStore
    {
        private Dictionary<IFingerprint, Issue> _store = new Dictionary<IFingerprint, Issue>();

        /// <summary>
        /// Can this store enumerate contents?
        /// </summary>
        public bool IsEnumerable { get => true; }

        /// <summary>
        /// Can this store be updated?
        /// </summary>
        public bool IsUpdatable { get => true; }

        /// <summary>
        /// The Issues currently in the store. Use this only to enumerate; use TryFindIssue to search.
        /// The Issues are unsorted, and may be provided in any order
        /// </summary>
        public IEnumerable<Issue> Issues => _store.Values;

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
        /// Add an Issue to the store
        /// </summary>
        /// <param name="issue">The issue to add</param>
        /// <returns>The result of the operation</returns>
        public AddResult AddIssue(Issue issue)
        {
            issue.ArgumentIsNotNull(nameof(issue));
            if (_store.TryGetValue(issue.Fingerprint, out Issue existingIssue))
                return AddResult.ItemAlreadyExists;

            _store.Add(issue.Fingerprint, issue);
            return AddResult.ItemAdded;
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
