// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.CommonUxComponents.Controls
{
    class CommonAutomationPeerCreator
    {
        public static AutomationPeer CreateIconAutomationPeer(UserControl owner, bool showInControlView)
        {
            return new CustomControlOverridingAutomationPeer(
                owner,
                localizedControl: "icon",
                isContentElement: false,
                isControlElement: showInControlView,
                hideChildren: true,
                controlType: AutomationControlType.Image);
        }
    }
}
