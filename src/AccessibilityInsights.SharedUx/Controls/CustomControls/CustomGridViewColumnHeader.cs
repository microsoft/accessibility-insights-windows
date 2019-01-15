// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Override GridViewColumnHeader in case we decide to remove default supported patterns
    /// </summary>
    public class CustomGridViewColumnHeader : GridViewColumnHeader
    {
        protected override AutomationPeer OnCreateAutomationPeer() => new CustomGridViewColumnHeaderAutomationPeer(this);
    }

    public class CustomGridViewColumnHeaderAutomationPeer : GridViewColumnHeaderAutomationPeer
    {
        public CustomGridViewColumnHeaderAutomationPeer(GridViewColumnHeader owner)
            : base(owner) { }

        public override object GetPattern(PatternInterface patternInterface)
        {
            /*
             * Unclear whether we should support these patterns
             * For now we support invoke & transform by default (parent)
            if (patternInterface == PatternInterface.Invoke)
            {
                return null;
            }
            */
            return base.GetPattern(patternInterface);
        }
    }

    
}
