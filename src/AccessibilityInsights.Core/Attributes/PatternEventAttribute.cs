// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Axe.Windows.Core.Attributes
{
    /// <summary>
    /// PatternEventAttribute class
    /// indicate the expected events for the pattern
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class PatternEventAttribute:Attribute
    {
        /// <summary>
        /// Event ID
        /// </summary>
        public int Id { get; set; }
    }
}
