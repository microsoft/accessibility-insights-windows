// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Types;
using System;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls
{
    public class NPISlider : Slider
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NPISliderAutomationPeer(this);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            this.Value = (int)newValue;
            FirePropertyChangeEvent((int)oldValue, (int)newValue);
        }

        private void FirePropertyChangeEvent(int oldValue, int newValue)
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = UIElementAutomationPeer.FromElement(this) as NPISliderAutomationPeer;
                peer?.RaisePropertyChangedEvent(
                    AutomationProperty.LookupById(PropertyType.UIA_ValuePattern_ValuePropertyId),
                    oldValue.ToString(CultureInfo.InvariantCulture),
                    newValue.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    public class NPISliderAutomationPeer : SliderAutomationPeer, IValueProvider
    {
        public NPISliderAutomationPeer(NPISlider owner)
        : base(owner)
        {
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }

            return null;
        }

        public string Value
        {
            get
            {
                return ((int)(((NPISlider)base.Owner).Value)).ToString(CultureInfo.InvariantCulture);
            }

            set
            {
                ((NPISlider)base.Owner).Value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void SetValue(string value)
        {
            ((NPISlider)base.Owner).Value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
    }
}
