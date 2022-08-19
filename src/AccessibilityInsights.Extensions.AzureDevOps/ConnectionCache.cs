// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    /// <summary>
    /// Stores a cache of connections (AzureDevOps)
    /// </summary>
    public class ConnectionCache
    {
        public const int CAPACITY = 5;

        private List<ConnectionInfo> CachedConnections { get; }

        /// <summary>
        /// Creates a cache with the given capacity
        /// </summary>
        public ConnectionCache()
        {
            CachedConnections = new List<ConnectionInfo>(CAPACITY);
        }

        /// <summary>
        /// Creates a cache with the given capacity and attempts to populate it with the saved connections
        /// </summary>
        /// <param name="configString">The previously saved connections. Supports null as a valid value</param>
        public ConnectionCache(string configString) : this()
        {
            if (string.IsNullOrEmpty(configString))
                return;

            try
            {
                ConnectionInfo[] saved = JsonConvert.DeserializeObject<ConnectionInfo[]>(configString);
                {
                    int max = Math.Min(saved.Length, CAPACITY);
                    for (int loop = 0; loop < max; loop++)
                    {
                        ConnectionInfo configConnection = saved[loop];
                        if (configConnection.IsPopulated)
                        {
                            AddToCachePrivate(configConnection);
                        }
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Adds the given connection to the cache and returns whether there was an existing
        ///     connection that this connection replaced (same server URL)
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool AddToCache(ConnectionInfo connection)
        {
            return AddToCachePrivate(connection);
        }

        /// <summary>
        /// Private implementation of AddToCache. Exists so we can safely call it from our constructor
        /// </summary>
        private bool AddToCachePrivate(ConnectionInfo connection)
        {
            int removed = CachedConnections.RemoveAll(c => c.ServerUri.Equals(connection.ServerUri) && c.LastUsage <= connection.LastUsage);

            bool reachedCapacity = CachedConnections.Count >= CAPACITY;
            if (reachedCapacity)
            {
                // Note that the new item is always added, even if we bump a newer entry
                CachedConnections.Sort(delegate (ConnectionInfo x, ConnectionInfo y)
                {
                    return x.LastUsage.CompareTo(y.LastUsage);
                });
                CachedConnections.RemoveAt(0); // assumes capacity > 0
            }

            // For serialization purposes, we need a concrete class. Allocate one if necessary
            ConnectionInfo typedConnection = connection as ConnectionInfo;
            if (typedConnection == null)
            {
                typedConnection = new ConnectionInfo(connection);
            }

            CachedConnections.Add(typedConnection);
            return removed > 0 || reachedCapacity;
        }

        /// <summary>
        /// Returns the most recent connection with the given server URL (most recent globally if server URL is null)
        /// </summary>
        /// <param name="serverUri"></param>
        /// <returns></returns>
        public ConnectionInfo GetMostRecentConnection(Uri serverUri = null)
        {
            return (from c in CachedConnections
                    where serverUri == null || c.ServerUri.AbsoluteUri == serverUri.AbsoluteUri
                    orderby c.LastUsage descending
                    select c)
                    .FirstOrDefault();
        }

        public IEnumerable<Uri> GetCachedConnections()
        {
            return CachedConnections.Select(conn => conn.ServerUri);
        }

        public string ToConfigString()
        {
            return JsonConvert.SerializeObject(CachedConnections);
        }
    }
}
