// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Misc;
using System.Collections.Generic;

namespace AccessibilityInsights.Core.Fingerprint
{
    /// <summary>
    /// Encapsulates data about the locations where a specific fingerprint has been found
    /// </summary>
    public class Issue
    {
        private readonly List<ILocation> _locations;

        /// <summary>
        /// The Fingerprint for this issue
        /// </summary>
        public IFingerprint Fingerprint { get; }

        /// <summary>
        /// The type of issue being reported
        /// </summary>
        public string IssueType { get; }

        /// <summary>
        /// Locations where this issue exists
        /// </summary>
        public IEnumerable<ILocation> Locations { get => _locations; }

        /// <summary>
        /// Add a location to the set associated with the Issue
        /// </summary>
        /// <param name="location">Location to where this issue occurs</param>
        /// <returns>true if the Issue has been changed as a result of this call</returns>
        public AddResult AddLocation(ILocation location)
        {
            location.ArgumentIsNotNull(nameof(location));

            if (_locations.Contains(location))
                return AddResult.ItemAlreadyExists;

            _locations.Add(location);
            return AddResult.ItemAdded;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="fingerprint">The fingerprint of the issue</param>
        /// <param name="issueType">The type of issue being tracked</param>
        public Issue(IFingerprint fingerprint, string issueType)
        {
            fingerprint.ArgumentIsNotNull(nameof(fingerprint));
            issueType.ArgumentIsNotTrivialString(nameof(issueType));

            IssueType = issueType;
            Fingerprint = fingerprint;
            _locations = new List<ILocation>();
        }
    }
}
