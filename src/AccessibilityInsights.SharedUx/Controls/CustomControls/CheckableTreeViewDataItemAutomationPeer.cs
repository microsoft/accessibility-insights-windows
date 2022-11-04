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
            {
                return this;
            }

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
            RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ConvertToToggleState(!_checkBox.IsChecked), ConvertToToggleState(_checkBox.IsChecked));
        }

        private static ToggleState ConvertToToggleState(bool? value)
        {
            switch (value)
            {
                case (true): return ToggleState.On;
                case (false): return ToggleState.Off;
                default: return ToggleState.Indeterminate;
            }
        }
    }
}
