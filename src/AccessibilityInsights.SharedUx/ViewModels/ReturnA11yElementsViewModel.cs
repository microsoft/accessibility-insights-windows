// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ActionViews;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.UIAutomation;
using System.Collections.Generic;
using System.Reflection;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// ViewModel class for actions without any parameter
    /// </summary>
    [TargetActionView(ViewType = typeof(ReturnA11yElementsView))]
    public class ReturnA11yElementsViewModel : BaseActionViewModel
    {
        public ReturnA11yElementsViewModel(A11yPattern pattern, MethodInfo m) : base(pattern, m) { }

        /// <summary>
        /// get new data
        /// </summary>
        protected override void InvokeMethod()
        {
            var items = this.methodinfo.Invoke(this.pattern, GetParametersArray());

            if (items is DesktopElement)
            {
                this.ReturnValue = new List<DesktopElement>() { (DesktopElement)items };
            }
            else
            {
                this.ReturnValue = (List<DesktopElement>)items;
            }
        }
    }
}
