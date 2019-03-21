// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// Team
    /// </summary>
    public class Team : AzureDevOpsEntity
    {
        // Concrete* fields exists for JSON serialization (don't serialize the public counterparts)
        public TeamProject ConcreteParentProject { get; set; }

        [JsonIgnore]
        public TeamProject ParentProject => ConcreteParentProject;

        /// <summary>
        /// Default ctor -- used exclusively for JSON serialization
        /// </summary>
        public Team() { }

        public Team(string name, Guid id, TeamProject parent = null) : base(name, id)
        {
            if (parent != null)
            {
                TeamProject typedParent = parent as TeamProject;
                ConcreteParentProject = typedParent ?? new TeamProject(parent);
            }
        }

        /// <summary>
        /// Copy ctor - enforces correct types internally
        /// </summary>
        /// <param name="original">The original object being copied</param>
        public Team(Team original) : this(original.Name, original.Id, original.ParentProject) { }
    }
}
