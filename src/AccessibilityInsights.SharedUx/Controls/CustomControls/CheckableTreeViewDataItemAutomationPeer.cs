// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CheckableTreeViewDataItemAutomationPeer : TreeViewDataItemAutomationPeer, IToggleProvider
    {
        private readonly EventConfigNodeViewModel _owner;

        public CheckableTreeViewDataItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer, TreeViewDataItemAutomationPeer parentDataItemAutomationPeer)
            : base(item, itemsControlAutomationPeer, parentDataItemAutomationPeer)
        {
            _owner = item as EventConfigNodeViewModel;
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
                return ExtensionMethods.ConvertToToggleState(_owner.IsChecked);
            }
        }

        public void Toggle()
        {
            _owner.IsChecked = !_owner.IsChecked;
            RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ExtensionMethods.ConvertToToggleState(!_owner.IsChecked), ExtensionMethods.ConvertToToggleState(_owner.IsChecked));
        }
    }
}
