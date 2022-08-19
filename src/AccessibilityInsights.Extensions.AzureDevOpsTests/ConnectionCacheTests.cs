// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AccessibilityInsights.Extensions.AzureDevOpsTests
{
    [TestClass]
    public class ConnectionCacheTests
    {
        private const string KnownConfigString = @"[{""ServerUri"":""http://fake.fake/1"",""Project"":{""Name"":""Team1"",""Id"":""3f574102-ea99-46e7-a945-864d3ebb57d1""},""Team"":null,""LastUsage"":""2000-01-02T03:04:05.0060001Z""},{""ServerUri"":""http://fake.fake/2"",""Project"":{""Name"":""Team2"",""Id"":""3f574102-ea99-46e7-a945-864d3ebb57d2""},""Team"":null,""LastUsage"":""2000-01-02T03:04:05.0060002Z""}]";

        private static readonly DateTime BaseDateTime = new DateTime(2000, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc);

        /// <summary>
        /// MRU should be null when empty
        /// </summary>
        [TestMethod]
        [Timeout(1000)]
        public void TestEmpty()
        {
            ConnectionCache cache = new ConnectionCache();
            Assert.IsNull(cache.GetMostRecentConnection());
        }

        /// <summary>
        /// should only hold "capacity" objects
        /// </summary>
        [TestMethod]
        [Timeout(1000)]
        public void TestCapacity()
        {
            ConnectionCache cache = new ConnectionCache();
            for (int i = 0; i < ConnectionCache.CAPACITY; i++)
            {
                cache.AddToCache(GetConnectionInfo(i));
                Assert.AreEqual(i + 1, cache.GetCachedConnections().Count());
            }

            // add one more over capacity
            cache.AddToCache(GetConnectionInfo(ConnectionCache.CAPACITY));
            Assert.AreEqual(ConnectionCache.CAPACITY, cache.GetCachedConnections().Count());
        }

        /// <summary>
        /// various manipulations, check ordering
        /// </summary>
        [TestMethod]
        [Timeout(1000)]
        public void TestOrder()
        {
            ConnectionCache cache = new ConnectionCache();

            // set to some high number
            const int highNumber = ConnectionCache.CAPACITY + 100;
            ConnectionInfo expectedMostRecentConnection = null;

            // Full cache with lastUsage values in reverse order (to force sorting).
            // Intentionally add one more than will fit in the cache
            for (int i = ConnectionCache.CAPACITY; i >= 0; i--)
            {
                ConnectionInfo newConnectionInfo = GetConnectionInfo(i + highNumber);
                cache.AddToCache(newConnectionInfo);

                if (expectedMostRecentConnection == null)
                {
                    expectedMostRecentConnection = newConnectionInfo;
                }
                AssertExpectedMostRecent(cache, expectedMostRecentConnection);
            }

            // Add a new ConnectionInfo with a newer time--GetMostRecentConnectionInfo should update
            ConnectionInfo newerConnection = GetConnectionInfo(2 * highNumber);
            cache.AddToCache(newerConnection);
            AssertExpectedMostRecent(cache, newerConnection);

            // Revise a ConnectionInfo with an even newer time--GetMostRecentConnectionInfo should update
            newerConnection.SetLastUsage(newerConnection.LastUsage.AddDays(1));
            cache.AddToCache(newerConnection);
            AssertExpectedMostRecent(cache, newerConnection);
        }

        private void AssertExpectedMostRecent(ConnectionCache cache, ConnectionInfo expectedMostRecent)
        {
            ConnectionInfo actualMostRecent = cache.GetMostRecentConnection();
            Assert.AreEqual(expectedMostRecent.ServerUri, actualMostRecent.ServerUri);
            Assert.AreEqual(expectedMostRecent.LastUsage, actualMostRecent.LastUsage);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_ConfigStringIsTrivial_CacheIsEmpty()
        {
            ConnectionCache cache = new ConnectionCache(null);
            Assert.AreEqual(0, cache.GetCachedConnections().Count());
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_ConfigStringIsInvalidJson_CacheIsEmpty()
        {
            ConnectionCache cache = new ConnectionCache("This isn't valid Json");
            Assert.IsFalse(cache.GetCachedConnections().Any());
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_ConfigStringIsValidJson_CacheIsCorrect()
        {
            ConnectionCache cache = new ConnectionCache(KnownConfigString);
            List<Uri> connections = cache.GetCachedConnections().ToList();
            Assert.AreEqual(2, connections.Count);

            ConnectionInfo info1 = GetConnectionInfo(1, true);
            ConnectionInfo info2 = GetConnectionInfo(2, true);

            CompareConnectionInfos(info1, cache.GetMostRecentConnection(info1.ServerUri));
            CompareConnectionInfos(info2, cache.GetMostRecentConnection(info2.ServerUri));
        }

        private void CompareConnectionInfos(ConnectionInfo expected, ConnectionInfo actual)
        {
            Assert.AreEqual(expected.ServerUri, actual.ServerUri);
            Assert.AreEqual(expected.Project.Name, actual.Project.Name);
            Assert.AreEqual(expected.Project.Id, actual.Project.Id);
            Assert.AreEqual(expected.LastUsage, actual.LastUsage);
            Assert.IsNull(actual.Team);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToConfigString_MatchesExpectedString()
        {
            // Note: This test might break if the serialization order changes
            ConnectionCache cache = new ConnectionCache();
            cache.AddToCache(GetConnectionInfo(1, true));
            cache.AddToCache(GetConnectionInfo(2, true));
            string configString = cache.ToConfigString();
            Assert.AreEqual(KnownConfigString, configString);
        }

        private TeamProject GetProject(int i)
        {
            if (i < 0 || i > 9)
                throw new ArgumentException("out of range", nameof(i));

            string projectName = string.Format(CultureInfo.InvariantCulture, "Team{0}", i);
            string projectGuid = string.Format(CultureInfo.InvariantCulture, "3F574102-EA99-46E7-A945-864D3EBB57D{0}", i);
            return new TeamProject(projectName, new Guid(projectGuid));
        }

        /// <summary>
        /// returns info object with both server URL and last usage set
        /// </summary>
        private ConnectionInfo GetConnectionInfo(int i, bool hasProject = false, int? offsetInTicks = null)
        {
            TeamProject project = hasProject ? GetProject(i) : null;
            ConnectionInfo info = new ConnectionInfo(new Uri($"http://fake.fake/{i}"), project, null);
            info.SetLastUsage(BaseDateTime.AddTicks(offsetInTicks ?? i));
            return info;
        }
    }
}
