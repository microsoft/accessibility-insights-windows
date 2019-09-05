// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.CommonUxComponents.Controls
{
    class CommonAutomationPeerCreator
    {
        /// <summary>
        /// Returns a peer for a decorative icon (hide children, raw tree only)
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static AutomationPeer CreateIconAutomationPeer(UserControl owner)
        {
            return new CustomControlOverridingAutomationPeer(
                owner,
                localizedControl: "icon",
                isContentElement: false,
                isControlElement: false,
                hideChildren: true,
                controlType: AutomationControlType.Image);
        }
    }
}
