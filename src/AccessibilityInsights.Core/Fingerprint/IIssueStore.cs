// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Axe.Windows.Core.Fingerprint
{
    /// <summary>
    /// Abstraction around an issue store. The actual store could take a variety of different
    /// formats and exist in a variety of locations
    /// </summary>
    public interface IIssueStore : IDisposable
    {
        /// <summary>
        /// Can this store enumerate contents?
        /// </summary>
        bool IsEnumerable { get; }

        /// <summary>
        /// Can this store be updated?
        /// </summary>
        bool IsUpdatable { get; }

        /// <summary>
        /// The Issues currently in the store. Use this only to enumerate; use TryFindIssue to search.
        /// The Issues are unsorted, and may be provided in any order
        /// </summary>
        IEnumerable<Issue> Issues { get; }

        /// <summary>
        /// Search the results for an issue with the specified fingerprint
        /// </summary>
        /// <param name="fingerprint">The fingerprint we're trying to find</param>
        /// <param name="issue">The issue, if it exists (null if not)</param>
        /// <returns>true iff a matching issue was located and returned</returns>
        bool TryFindIssue(IFingerprint fingerprint, out Issue issue);

        /// <summary>
        /// Add an Issue to the store
        /// </summary>
        /// <param name="issue">The issue to add</param>
        /// <returns>The result of the operation</returns>
        AddResult AddIssue(Issue issue);
    }
}
