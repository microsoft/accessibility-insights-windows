// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
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
        /// Value for custom IsContentElement
        /// </summary>
        private bool IsContentElem;

        /// <summary>
        /// String for custom LocalizedControlType
        /// </summary>
        private string LocalizedControlType;

        private AutomationControlType ControlType;

        /// <summary>
        /// Whether to hide all children automation peers
        /// </summary>
        private bool HideChildren;

        public AutomationOrientation AutomationOrientation { get; set; }

        public CustomControlOverridingAutomationPeer(UserControl owner,string localizedControl, bool isControlElement=true, bool isContentElement=false, bool hideChildren=false, AutomationControlType controlType=AutomationControlType.Custom)
        : base(owner)
        {
            this.LocalizedControlType = localizedControl;
            this.IsControlElem = isControlElement;
            this.IsContentElem = isContentElement;
            this.HideChildren = hideChildren;
            this.ControlType = controlType;
        }

        protected override string GetLocalizedControlTypeCore()
        {
            return LocalizedControlType;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return ControlType;
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

        protected override List<AutomationPeer> GetChildrenCore()
        {
            return HideChildren ? null : base.GetChildrenCore();
        }

        protected override AutomationOrientation GetOrientationCore() => AutomationOrientation;
    }
}
