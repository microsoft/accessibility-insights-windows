// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Misc;
using System;

namespace Axe.Windows.Core.Fingerprint
{
    /// <summary>
    /// Extension methods for IIssueStore
    /// </summary>
    public static class IssueStoreExtensions
    {
        /// <summary>
        /// Merge the contents of an enumerable IIssueStore into this IIssueStore
        /// </summary>
        /// <param name="sourceStore">The IssueStore that will provide the data</param>
        /// <returns>The number of issues merged/added</returns>
        /// <exception cref="InvalidOperationException">Thrown if the incompatible stores are chosen</exception>
        public static int MergeIssuesFromStore(this IIssueStore targetStore, IIssueStore sourceStore)
        {
            sourceStore.ArgumentIsNotNull(nameof(sourceStore));
            targetStore.ArgumentIsNotNull(nameof(targetStore));

            if (!sourceStore.IsEnumerable)
                throw new InvalidOperationException("The Source store is not enumerable!");

            if (!targetStore.IsUpdatable)
                throw new InvalidOperationException("The Target store is not updatable!");

            int issuesMergedOrAdded = 0;

            foreach (Issue sourceIssue in sourceStore.Issues)
            {
                bool updated = false;

                if (targetStore.TryFindIssue(sourceIssue.Fingerprint, out Issue existingIssue))
                {
                    foreach (ILocation newLocation in sourceIssue.Locations)
                    {
                        updated |= (existingIssue.AddLocation(newLocation) == AddResult.ExistingItemUpdated);
                    }
                }
                else
                {
                    updated |= (targetStore.AddIssue(sourceIssue) == AddResult.ItemAdded);
                }

                if (updated)
                {
                    issuesMergedOrAdded++;
                }
            }

            return issuesMergedOrAdded;
        }
    }
}
