using System;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CheckableTreeViewDataItemAutomationPeer : TreeViewDataItemAutomationPeer, IToggleProvider
    {
        private CheckBox _checkBox;

        public CheckableTreeViewDataItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer, TreeViewDataItemAutomationPeer parentDataItemAutomationPeer, CheckBox checkBox)
            : base(item, itemsControlAutomationPeer, parentDataItemAutomationPeer)
        {
            _checkBox = checkBox;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Toggle)
                return this;

            return base.GetPattern(patternInterface);
        }

        public ToggleState ToggleState
        {
            get
            {
                return _checkBox.IsChecked == true ? ToggleState.On : ToggleState.Off;
            }
        }

        public void Toggle()
        {
            throw new NotImplementedException();
        }
    }
}
