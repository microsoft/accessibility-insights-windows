// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// class TeamProject
    /// </summary>
    public class TeamProject : AzureDevOpsEntity, IProject
    {
        /// <summary>
        /// Default ctor -- used exclusively for JSON serialization
        /// </summary>
        public TeamProject() { }

        public TeamProject(string name, Guid id) : base(name, id) { }

        /// <summary>
        /// Copy ctor - enforces correct types internally
        /// </summary>
        /// <param name="original">The original object being copied</param>
        public TeamProject(IProject original) : this(original.Name, original.Id) { }

        public Task<IEnumerable<ITeam>> GetTeamsAsync()
        {
            return Task<IEnumerable<ITeam>>.Run(() => AzureDevOpsIntegration.GetCurrentInstance().GetTeamsFromProject(this));
        }
    }
}
