// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Class to improve gridview accessibility issues
    /// </summary>
    public class CustomGridView : GridView
    {
        protected override IViewAutomationPeer GetAutomationPeer(ListView parent)
        {
            return new GridViewAutomationPeer(this, parent);
        }
    }
}
