// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Properties;
using System;
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
        private bool IsControlElem;

        private bool IsContentElem;

        private string LocalizedControlType;

        private AutomationControlType ControlType;

        private bool HideChildren;

        public AutomationOrientation AutomationOrientation { get; set; }

        public CustomControlOverridingAutomationPeer(
            UserControl owner,
            string localizedControl,
            bool isControlElement = true,
            bool isContentElement = false,
            bool hideChildren = false,
            AutomationControlType controlType = AutomationControlType.Custom)
        : base(owner)
        {
            if (isContentElement && !isControlElement)
            {
                throw new ArgumentException(Resources.ContentTreeIsSubsetofControlTree);
            }
            this.LocalizedControlType = localizedControl;
            this.IsControlElem = isControlElement;
            this.IsContentElem = isContentElement;
            this.HideChildren = hideChildren;
            this.ControlType = controlType;
        }

        protected override string GetLocalizedControlTypeCore() => LocalizedControlType;
        protected override AutomationControlType GetAutomationControlTypeCore() => ControlType;

        public override object GetPattern(PatternInterface patternInterface) => null;

        protected override bool IsControlElementCore() => this.Owner.IsVisible && IsControlElem;

        protected override bool IsContentElementCore() => this.Owner.IsVisible && IsContentElem;

        protected override List<AutomationPeer> GetChildrenCore() => HideChildren ? null : base.GetChildrenCore();

        protected override AutomationOrientation GetOrientationCore() => AutomationOrientation;
    }
}
