// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using Axe.Windows.Core.Bases;
using AccessibilityInsights.SharedUx.ActionViews;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// ViewModel class for general actions
    /// </summary>
    [TargetActionView(ViewType = typeof(GeneralActionView))]
    public class GeneralActionViewModel : BaseActionViewModel
    {
        /// <summary>
        /// GeneralActionViewModel constructor
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="m"></param>
        public GeneralActionViewModel(A11yPattern pattern, MethodInfo m):base(pattern, m) { }
    }
}
