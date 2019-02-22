// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Windows.Media;

namespace WildlifeManager
{
    /// <summary>
    ///  sample button with two patterns(Invoke and Toggle). 
    /// </summary>
    public class InvokeToggleButton : Button
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ButtonWithInvokeAndToggleAutomationPeer(this);
        }
    }

    /// <summary>
    /// class to override GetPattern for Button Control with Invoke and Toggle both.
    /// </summary>
    public class ButtonWithInvokeAndToggleAutomationPeer : ButtonAutomationPeer, IToggleProvider
    {

        public ButtonWithInvokeAndToggleAutomationPeer(Button owner) : base(owner)
        {
        }

        ToggleState IToggleProvider.ToggleState => throw new NotImplementedException();

        public override object GetPattern(PatternInterface patternInterface)
        {
            switch(patternInterface)
            {
                case PatternInterface.Invoke:
                    return this;
                case PatternInterface.Toggle:
                    return this;
            }
            return null;
        }

        void IToggleProvider.Toggle()
        {
            throw new NotImplementedException();
        }
    }

}
