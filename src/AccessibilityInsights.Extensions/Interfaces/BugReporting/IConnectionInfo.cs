// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.Interfaces.BugReporting
{
    /// <summary>
    /// Interface to represent a single server connection
    /// </summary>
    public interface IConnectionInfo : IEquatable<IConnectionInfo>
    {
        /// <summary>
        /// Returns whether the data connection information is populated
        /// </summary>
        bool IsPopulated { get; }

        /// <summary>
        /// The Uri to the server
        /// </summary>
        Uri ServerUri { get; }

        /// <summary>
        /// The project identifier
        /// </summary>
        IProject Project { get; }

        /// <summary>
        /// The team identifier
        /// </summary>
        ITeam Team { get; }

        /// <summary>
        /// The time of the last update to this connection
        /// </summary>
        DateTime LastUsage { get; }

        /// <summary>
        /// Update the last update time
        /// </summary>
        /// <param name="updateTime">The new update time</param>
        void SetLastUsage(DateTime? updateTime = null);

        /// <summary>
        /// Serialize the data contents to a config string
        /// </summary>
        /// <returns>A config string that can later be used to rehydrate this object</returns>
        string ToConfigString();
    }
}
