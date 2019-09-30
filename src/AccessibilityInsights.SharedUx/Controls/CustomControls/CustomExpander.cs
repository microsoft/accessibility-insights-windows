using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    public class CustomExpander : Expander
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomExpanderAutomationPeer(this);
        }
    }
    public class CustomExpanderAutomationPeer : ExpanderAutomationPeer
    {
        public CustomExpanderAutomationPeer(Expander owner)
            : base(owner)
        {
        }
        override protected bool HasKeyboardFocusCore() => true;
    }
}