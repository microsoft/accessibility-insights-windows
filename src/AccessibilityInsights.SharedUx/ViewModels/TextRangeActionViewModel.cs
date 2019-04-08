// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using Axe.Windows.Core.Bases;
using AccessibilityInsights.SharedUx.ActionViews;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// ViewModel class for TextRange actions
    /// </summary>
    [TargetActionView(ViewType = typeof(TextRangeActionView))]
    public class TextRangeActionViewModel : BaseActionViewModel
    {

        public TextRangeActionViewModel(A11yPattern pattern, MethodInfo m):base(pattern, m) { }
    }
}
