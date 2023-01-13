// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SharedUx.Utilities;
using System.IO;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class ToggleMenuItemAutomationPeer : MenuItemAutomationPeer, IToggleProvider
    {
        private readonly RadioButton _radioButton;

        public ToggleMenuItemAutomationPeer(MenuItem item)
            : base(item)
        {
            _radioButton = item.Header as RadioButton;
            if (_radioButton == null)
            {
                throw new InvalidDataException("Invalid menu item header");
            }
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
                return ExtensionMethods.ConvertToToggleState(_radioButton.IsChecked);
            }
        }

        public void Toggle()
        {
            _radioButton.IsChecked = !_radioButton.IsChecked;
            RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ExtensionMethods.ConvertToToggleState(!_radioButton.IsChecked), ExtensionMethods.ConvertToToggleState(_radioButton.IsChecked));
        }
    }
}
