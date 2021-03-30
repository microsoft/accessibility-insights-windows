// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// Represents an entity with a name and id
    /// </summary>
    public class AzureDevOpsEntity
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        /// <summary>
        /// Default ctor -- used exclusively for JSON serialization
        /// </summary>
        public AzureDevOpsEntity() { }

        public AzureDevOpsEntity(string name, Guid id)
        {
            this.Name = name;
            this.Id = id;
        }

        public override int GetHashCode()
        {
            return new Tuple<string, Guid>(this.Name, this.Id).GetHashCode();
        }

        /// <summary>
        /// equal override
        /// since we compare the contents than object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            AzureDevOpsEntity entity = obj as AzureDevOpsEntity;
            if (entity == null)
                return false;
            else
                return this.Name == entity.Name && this.Id == entity.Id;
        }
    }
}
