// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Properties;
using Axe.Windows.Core.Attributes;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.UIAutomation.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// UI wrapper for Pattern
    /// </summary>
    public class PatternViewModel : ViewModelBase
    {
        public PatternViewModel(A11yElement e, A11yPattern pattern, bool isActionAllowed, bool isExpanded)
        {
            this.IsExpanded = isExpanded;
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.Element = e;
            this.ActionType = GetActionType();
            this.ActionVisibility = isActionAllowed && this.ActionType != ActionType.NotApplicable ? Visibility.Visible : Visibility.Collapsed;
            this.ActionName = GetActionButtonText();
            this.Properties = (from p in this.Pattern.Properties select new PatternPropertyUIWrapper(p)).ToList();
        }

        /// <summary>
        /// Get ActionViewModels based on Pattern Methods
        /// </summary>
        /// <returns></returns>
        public IList<BaseActionViewModel> GetActionViewModels()
        {
            return (from m in this.Pattern.Methods
                    select BaseActionViewModel.GetActionViewModel(this.Pattern, m)).ToList();
        }

        /// <summary>
        ///  Check whether pattern has any action.
        /// </summary>
        /// <returns></returns>
        private ActionType GetActionType()
        {
            switch (this.Pattern.Id)
            {
                case PatternType.UIA_TextPatternId:
                    return ActionType.TextExplorer;
                case PatternType.UIA_TextPattern2Id:
                    return ActionType.NotApplicable;
                default:
                    return ((from m in this.Pattern.GetType().GetMethods()
                             let a = m.GetCustomAttribute(typeof(PatternMethodAttribute))
                             where a != null
                             select m).Any() ? ActionType.Action : ActionType.NotApplicable);
            }
        }

        /// <summary>
        /// Get action button text
        /// </summary>
        /// <returns>The string to display as button text</returns>
        private string GetActionButtonText()
        {
            switch (this.ActionType)
            {
                case ActionType.Action:
                    return Resources.PatternViewModel_GetActionButtonText_Actions;
                case ActionType.TextExplorer:
                    return Resources.PatternViewModel_GetActionButtonText_Explore;
                default:
                    return string.Empty;
            }
        }

        public A11yPattern Pattern { get; private set; }

        public string Name
        {
            get
            {
                return this.Pattern.Name;
            }
        }

        /// <summary>
        /// Is tree node expanded
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// No tooltip for parent nodes
        /// </summary>
#pragma warning disable CA1822
        public string ToolTip
#pragma warning restore CA1822
        {
            get
            {
                return null;
            }
        }

        public string ActionName { get; private set; }

        public string NodeValue
        {
            get
            {
                return this.Pattern.Name;
            }
        }

        public IList<PatternPropertyUIWrapper> Properties { get; private set; }

        public Visibility ActionVisibility { get; private set; }

        private ICommand _clickCommand;
        private readonly ActionType ActionType;
        private readonly A11yElement Element;

        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => ShowControlPatternDialog(), true));
            }
        }

        public void ShowControlPatternDialog()
        {
            EnableSelection(false);

            if (this.ActionType == ActionType.Action)
            {
                ShowActionDialog();
            }
            else if (this.ActionType == ActionType.TextExplorer)
            {
                ShowTextExplorerDialog();
            }

            EnableSelection(true);
        }

        private static void EnableSelection(bool enabled)
        {
            IMainWindow wnd = Application.Current.MainWindow as IMainWindow;

            // Watson crashes suggest that this will be null sometimes
            if (wnd != null)
            {
                wnd.SetAllowFurtherAction(enabled);
            }
        }

        internal void Clear()
        {
            this.Properties.Clear();
            this.Properties = null;
            this.Pattern = null;
        }

        private void ShowTextExplorerDialog()
        {
            var tp2 = this.Element.Patterns.ById(PatternType.UIA_TextPattern2Id);

            var dlg = new TextPatternExplorerDialog((TextPattern)this.Pattern, tp2 == null ? null : (TextPattern2)tp2)
            {
                Owner = Application.Current.MainWindow
            };

            dlg.ShowDialog();
        }

        private void ShowActionDialog()
        {
            ControlPatternDialog dlg = new ControlPatternDialog(this)
            {
                Owner = Application.Current.MainWindow
            };

            dlg.ShowDialog();
        }
    }

    public class PatternPropertyUIWrapper
    {
        /// <summary>
        /// tooltip for child nodes
        /// </summary>
        public string ToolTip { get; private set; }
        public Visibility ActionVisibility { get; private set; }
        public string NodeValue { get; private set; }
        public PatternPropertyUIWrapper(A11yPatternProperty p)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p));

            /// Trim node text if it contains a newline and append ...
            /// Full value will be available via tooltip
            int idx = p.NodeValue.IndexOfAny(new char[] { '\r', '\n' });
            this.NodeValue = idx == -1 ? p.NodeValue : p.NodeValue.Substring(0, idx) + "...\"";

            this.ToolTip = p.NodeValue;

            this.ActionVisibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Indicate the type of Action
    /// </summary>
    public enum ActionType
    {
        Action,
        TextExplorer,
        NotApplicable
    }
}
