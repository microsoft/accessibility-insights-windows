// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Desktop.Types;
using System.Collections.Generic;
using UIAutomationClient;

using static System.FormattableString;
using TreeScope = UIAutomationClient.TreeScope;

namespace Axe.Windows.Desktop.UIAutomation.EventHandlers
{
    /// <summary>
    /// Class TextEditTextChanged Event handler
    /// It is place holder. it is not hooked up yet since AccEvent doesn't support it yet. 
    /// </summary>
    public class TextEditTextChangedEventListener : EventListenerBase, IUIAutomationTextEditTextChangedEventHandler
    {
        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public TextEditTextChangedEventListener(CUIAutomation8 uia8, IUIAutomationElement element, TreeScope scope, HandleUIAutomationEventMessage peDelegate) : base(uia8, element, scope, EventType.UIA_TextEdit_TextChangedEventId, peDelegate)
        {
            Init();
        }

        public override void Init()
        {
            IUIAutomation4 uia4 = this.IUIAutomation4;
            if (uia4 != null)
            {
                uia4.AddTextEditTextChangedEventHandler(this.Element, this.Scope, TextEditChangeType.TextEditChangeType_AutoComplete, null, this);
                uia4.AddTextEditTextChangedEventHandler(this.Element, this.Scope, TextEditChangeType.TextEditChangeType_AutoCorrect, null, this);
                uia4.AddTextEditTextChangedEventHandler(this.Element, this.Scope, TextEditChangeType.TextEditChangeType_Composition, null, this);
                uia4.AddTextEditTextChangedEventHandler(this.Element, this.Scope, TextEditChangeType.TextEditChangeType_CompositionFinalized, null, this);
                this.IsHooked = true;
            }
        }

        public void HandleTextEditTextChangedEvent(IUIAutomationElement sender, TextEditChangeType type, string[] array)
        {
            var m = EventMessage.GetInstance(this.EventId, sender);

            if (m != null)
            {
                m.Properties = new List<KeyValuePair<string, dynamic>>
                {
                    new KeyValuePair<string, dynamic>("TextEditChangeType", type.ToString()),
                };
                for (int i = 0; i < array.Length; i++)
                {
                    m.Properties.Add(new KeyValuePair<string, dynamic>(Invariant($"[{i}]"), array.GetValue(i)));
                };

                this.ListenEventMessage(m);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.IsHooked)
                    {
                        IUIAutomation4 uia4 = this.IUIAutomation4;
                        if (uia4 != null)
                        {
                            uia4.RemoveTextEditTextChangedEventHandler(this.Element, this);
                        }
                    }
                }
            }
            base.Dispose(disposing);
        }
    }
}
