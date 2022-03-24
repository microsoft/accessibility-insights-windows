// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Class TargetActionViewAttribute
    /// indicate the target action view for the ActionViewModel
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TargetActionViewAttribute : Attribute
    {
        public Type ViewType { get; set; }
    }
}
