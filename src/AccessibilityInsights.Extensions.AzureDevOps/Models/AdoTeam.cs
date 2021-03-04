// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// Team
    /// </summary>
    public class AdoTeam : AzureDevOpsEntity
    {
        public TeamProject ParentProject { get; set; }

        /// <summary>
        /// Default ctor -- used exclusively for JSON serialization
        /// </summary>
        public AdoTeam() { }

        public AdoTeam(string name, Guid id, TeamProject parent = null) : base(name, id)
        {
            ParentProject = parent;
        }

        /// <summary>
        /// Copy ctor - enforces correct types internally
        /// </summary>
        /// <param name="original">The original object being copied</param>
#pragma warning disable CA1062 // Validate arguments of public methods
        public AdoTeam(AdoTeam original) : this(original.Name, original.Id, original.ParentProject) { }
#pragma warning restore CA1062 // Validate arguments of public methods
    }
}
