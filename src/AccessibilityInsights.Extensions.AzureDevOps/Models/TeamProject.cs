// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// class TeamProject
    /// </summary>
    public class TeamProject : AzureDevOpsEntity
    {
        /// <summary>
        /// Default constructor -- used exclusively for JSON serialization
        /// </summary>
        public TeamProject() { }

        public TeamProject(string name, Guid id) : base(name, id) { }

        /// <summary>
        /// Copy constructor - enforces correct types internally
        /// </summary>
        /// <param name="original">The original object being copied</param>
#pragma warning disable CA1062 // Validate arguments of public methods
        public TeamProject(TeamProject original) : this(original.Name, original.Id) { }
#pragma warning restore CA1062 // Validate arguments of public methods

        internal Task<IEnumerable<AdoTeam>> GetTeamsAsync(IDevOpsIntegration devOpsIntegration)
        {
            return Task.Run(() => devOpsIntegration.GetTeamsFromProject(this));
        }
    }
}
