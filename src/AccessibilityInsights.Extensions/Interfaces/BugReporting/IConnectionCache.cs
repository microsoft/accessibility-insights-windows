// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.Interfaces.BugReporting
{
    public interface IConnectionCache
    {
        /// <summary>
        /// Adds the given connection to the cache and returns whether there was an existing
        ///     connection that this connection replaced (same server Uri)
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        bool AddToCache(IConnectionInfo connection);

        /// <summary>
        /// Returns the most recent connection with the given server URL (most recent globally if server url is null)
        /// </summary>
        /// <param name="serverUri"></param>
        /// <returns></returns>
        IConnectionInfo GetMostRecentConnection(Uri serverUri = null);

        /// <summary>
        /// Get all of the cached connections
        /// </summary>
        /// <returns></returns>
        IEnumerable<Uri> GetCachedConnections();

        /// <summary>
        /// Serialize the data contents to a config string
        /// </summary>
        /// <returns>A config string that can later be used to rehydrate this object</returns>
        string ToConfigString();
    }
}
