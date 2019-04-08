// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Axe.Windows.Core.Bases
{
    /// <summary>
    /// Container for Property and Patterns for passing these over wires. 
    /// </summary>
    public class A11yElementData
    {
#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// Properties. it is populated automatically at construction
        /// </summary>
        public Dictionary<int, A11yProperty> Properties { get; set; }

        /// <summary>
        /// Patterns
        /// </summary>
        public List<A11yPattern> Patterns { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
