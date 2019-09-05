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
