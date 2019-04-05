// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Axe.Windows.Core.Attributes
{
    /// <summary>
    /// PatternActionAttribute class
    /// indicate the method is actionable via UI
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PatternMethodAttribute:Attribute
    {
        /// <summary>
        /// if it is true, the method is related with UI Action
        /// </summary>
        public bool IsUIAction { get; set; }
    }
}
