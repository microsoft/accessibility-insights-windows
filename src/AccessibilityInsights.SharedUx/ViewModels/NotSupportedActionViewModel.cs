// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ActionViews;
using Axe.Windows.Core.Bases;
using System.Reflection;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// class NotSupportedActionViewModel
    /// ViewModel for Not yet supported Action.
    /// </summary>
    [TargetActionView(ViewType = typeof(NotSupportActionView))]
    public class NotSupportedActionViewModel : BaseActionViewModel
    {
        public NotSupportedActionViewModel(A11yPattern p, MethodInfo m) : base(p, m) { }

        /// <summary>
        /// Not needed but it is overridden
        /// </summary>
        protected override void InvokeMethod() { }
    }
}
