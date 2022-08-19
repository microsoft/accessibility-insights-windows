// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.AzureDevOps.Properties;
using Newtonsoft.Json;
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    /*
    Encapsulates information about a single connection to an AzureDevOps team
    */
    public class ConnectionInfo
    {
        // * fields exists for JSON serialization (don't serialize the public counterparts)
        public Uri ServerUri { get; set; }
        public TeamProject Project { get; set; }
        public AdoTeam Team { get; set; }
        public DateTime LastUsage { get; set; }

        // This is the time we'll use if no other time is specified. We use this instead of
        // default(DateTime) because we use UTC times internally and default(DateTime) has
        // issues with time zones east of UTC
        private static readonly DateTime DefaultTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Returns whether the data connection information is populated
        /// </summary>
        [JsonIgnore]
        public bool IsPopulated => ServerUri != null && Project != null;

        /// <summary>
        /// Default constructor -- used exclusively for JSON serialization
        /// </summary>
        public ConnectionInfo() { }

        /// <summary>
        /// Populates the server URL, project, and team
        /// - does not populate LastUsage, which is automatically
        /// set to a very early date (1/1/1900)
        /// </summary>
        /// <param name="serverUrl">Uri of the server</param>
        /// <param name="project">Identifies the project--null is a valid value</param>
        /// <param name="team">Identifies the team--null is a valid value</param>
        public ConnectionInfo(Uri serverUrl, TeamProject project, AdoTeam team)
        {
            ServerUri = serverUrl;
            Project = project;
            Team = team;
        }

        /// <summary>
        /// Populates a ConnectionInfo from a saved string. Will throw ArgumentException if the string is not usable
        /// </summary>
        /// <param name="configString"></param>
        public ConnectionInfo(string configString)
        {
            if (string.IsNullOrEmpty(configString))
                throw new ArgumentException(Resources.CantCreateConnectionInfo, nameof(configString));

            // If this throws, we'll catch it upstream
            ConnectionInfo savedConnectionInfo = JsonConvert.DeserializeObject<ConnectionInfo>(configString);
            ServerUri = savedConnectionInfo.ServerUri;
            Project = savedConnectionInfo.Project;
            Team = savedConnectionInfo.Team;
            LastUsage = savedConnectionInfo.LastUsage;
        }

        /// <summary>
        /// Copy constructor - enforces correct types internally
        /// </summary>
        /// <param name="original">The original object being copied</param>
        public ConnectionInfo(ConnectionInfo original)
#pragma warning disable CA1062 // Validate arguments of public methods
            : this(original.ServerUri, original.Project, original.Team)
#pragma warning restore CA1062 // Validate arguments of public methods
        {
        }

        /// <summary>
        /// Returns true if this connection info has the same
        ///     data field values (server URL, project, and team)
        ///     as the other object. Ignores LastUsage
        /// </summary>
        /// <param name="other">The other object to compare</param>
        /// <returns>true if and only if data fields match (ignores LastUsage field)</returns>
        public bool DataEquals(ConnectionInfo other)
        {
            if (other == null)
            {
                return false;
            }

            // crash was reported over watson.
            // make sure Project and Team are not null first.
            return ServerUri != null && ServerUri.Equals(other.ServerUri)
                && Project != null && Project.Equals(other.Project)
                && Team != null && Team.Equals(other.Team);
        }

        /// <summary>
        /// Returns the chosen connection, assumes a visual studio URL
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("/", Project?.Name, Team?.Name);
        }

        public void SetLastUsage(DateTime? updateTime = null)
        {
            DateTime newTime = updateTime.HasValue ? updateTime.Value.ToUniversalTime() : DefaultTime;
            LastUsage = newTime;
        }

        public bool Equals(ConnectionInfo other)
        {
            return DataEquals(other as ConnectionInfo);
        }

        public string ToConfigString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
