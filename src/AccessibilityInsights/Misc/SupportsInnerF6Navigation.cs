// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;

namespace AccessibilityInsights
{
    /// <summary>
    /// Implemented by the mode controls to allow F6 navigation into the inheriting control
    /// </summary>
    interface ISupportInnerF6Navigation
    {
        FrameworkElement GetFirstPane();
        FrameworkElement GetLastPane();
    } // interface
} // navigation
