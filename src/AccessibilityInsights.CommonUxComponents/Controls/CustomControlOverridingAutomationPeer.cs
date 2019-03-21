// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.CommonUxComponents.Controls
{
    /// <summary>
    /// CustomControlOverridingAutomationPeer class
    /// class to override LocalizedControlType and control/content element values for custom control(UserControl)
    /// </summary>
    public class CustomControlOverridingAutomationPeer: UserControlAutomationPeer
    {
        /// <summary>
        /// Value for custom IsControlElement
        /// </summary>
        private bool IsControlElem;

        /// <summary>
        /// Value for custom IsContentElem
        /// </summary>
        private bool IsContentElem;
        
        /// <summary>
        /// String for custom LocalizedControlType
        /// </summary>
        private string LocalizedControlType;

        public AutomationOrientation AutomationOrientation { get; set; }

        public CustomControlOverridingAutomationPeer(UserControl owner,string localizedcontrol, bool isControlElement=true, bool isContentElement=false)
        : base(owner)
        {
            this.LocalizedControlType = localizedcontrol;
            this.IsControlElem = isControlElement;
            this.IsContentElem = isContentElement;
        }

        protected override string GetLocalizedControlTypeCore()
        {
            return LocalizedControlType;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }

        /// <summary>
        /// Use provided value for IsControlElement
        /// </summary>
        /// <returns></returns>
        protected override bool IsControlElementCore()
        {
            return IsControlElem;
        }

        /// <summary>
        /// Use provided value for IsContentElement
        /// </summary>
        /// <returns></returns>
        protected override bool IsContentElementCore()
        {
            return IsContentElem;
        }

        protected override AutomationOrientation GetOrientationCore() => AutomationOrientation;
    }
}
