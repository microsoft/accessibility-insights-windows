// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Class to improve gridview accessibility issues
    /// </summary>
    public class CustomGridView : GridView
    {
        protected override IViewAutomationPeer GetAutomationPeer(ListView parent)
        {
            return new CustomGridViewAutomationPeer(this, parent);
        }
    }

    /// <summary>
    /// Improves accessibility on gridviewcellitems
    /// </summary>
    public class CustomGridViewAutomationPeer : GridViewAutomationPeer, IViewAutomationPeer
    {
        private ListView listView = null;
        public CustomGridViewAutomationPeer(GridView owner, ListView listView)
            : base(owner, listView)
        {
            this.listView = listView;
        }
    }

    /// <summary>
    /// Class to remove custom control wrapping GridView cell
    /// </summary>
    public class CustomGridViewItemAutomationPeer : GridViewItemAutomationPeer
    {
        private ListViewAutomationPeer listviewAP;
        public CustomGridViewItemAutomationPeer(object owner, ListViewAutomationPeer listviewAP): base(owner, listviewAP)
        {
            this.listviewAP = listviewAP;
        }
    }
}
