// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Automation peer for a grouped list view with group items that 
    /// support the toggle patter via checkboxes in the expander
    /// </summary>
    public class ListViewWithGroupItem : ListView
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BasicPeerWithGroupItemToggled(this);
        }
    }

    public class BasicPeerWithGroupItemToggled : ListViewAutomationPeer
    {
        public BasicPeerWithGroupItemToggled(ListView owner) : base(owner)
        {
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            var list = new List<AutomationPeer>();
            var existingPeers = base.GetChildrenCore();
            if (existingPeers != null)
            {
                foreach (var child in existingPeers)
                {
                    if (child is GroupItemAutomationPeer)
                    {
                        var groupItemUIElement = (child as GroupItemAutomationPeer).Owner as GroupItem;
                        list.Add(new GroupItemWithTogglePatternPeer(groupItemUIElement));
                    }
                    else
                    {
                        list.Add(child);
                    }
                }
            }
            return list;
        }
    }

    public class GroupItemWithTogglePatternPeer : GroupItemAutomationPeer
    {
        public GroupItemWithTogglePatternPeer(GroupItem owner) : base(owner)
        {
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Toggle)
            {
                var cb = HelperMethods.GetFirstChildElement<CheckBox>(Owner) as CheckBox;
                AutomationPeer checkboxPeer = UIElementAutomationPeer.CreatePeerForElement(cb);
                if (checkboxPeer != null && checkboxPeer is IToggleProvider)
                {
                    return (IToggleProvider)checkboxPeer;
                }
            }
            return base.GetPattern(patternInterface);
        }
    }
}
