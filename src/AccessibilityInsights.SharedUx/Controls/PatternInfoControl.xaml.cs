// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for PatternInfoControl.xaml
    /// </summary>
    public partial class PatternInfoControl : UserControl
    {
        /// <summary>
        /// Keep track of which patterns are expanded
        /// </summary>
        Dictionary<int, bool> ExpStates = new Dictionary<int, bool>();

        public A11yElement Element { get; private set; }
        public bool IsActionAllowed { get; private set; }

        public PatternInfoControl()
        {
            InitializeComponent();
            foreach (var kv in PatternType.GetInstance().GetKeyValuePairList())
            {
                ExpStates[kv.Key] = false;
            }
            HidePatternsTree();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Set Element to display on the UI
        /// </summary>
        /// <param name="e"></param>
        /// <param name="showInvoke">indicate whether invoke button should be visible or not</param>
        public void SetElement(A11yElement e, bool isActionAllowed)
        {
            if (e != null)
            {
                this.Element = e;
                this.IsActionAllowed = isActionAllowed;
                UpdateControlPatterns();
            }
            else
            {
                this.Clear();
            }
        }

        private static IEnumerable<int> GetBasicPropertyList()
        {
            return PropertySettings.DefaultCoreProperties;
        }

        private void UpdateControlPatterns()
        {
            if (this.Element != null)
            {
                if (this.Element.Patterns != null && this.Element.Patterns.Count != 0)
                {
                    var patterns = from p in this.Element.Patterns
                                   select new PatternViewModel(this.Element, p, this.IsActionAllowed, ExpStates.ContainsKey(p.Id) ? ExpStates[p.Id] : false);

                    this.treePatterns.ItemsSource = patterns.ToList();
                    ShowPatternsTree();
                }
                else
                {
                    HidePatternsTree();
                    this.treePatterns.ItemsSource = null;
                }
            }
        }

        private void HidePatternsTree()
        {
            this.treePatterns.Visibility = Visibility.Collapsed;
            this.lbNoPattern.Visibility = Visibility.Visible;
        }

        private void ShowPatternsTree()
        {
            this.lbNoPattern.Visibility = Visibility.Collapsed;
            this.treePatterns.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clear UI
        /// </summary>
        public void Clear()
        {
            if (this.treePatterns.ItemsSource != null)
            {
                var list = (List<PatternViewModel>)this.treePatterns.ItemsSource;
                list.ForEach(l => l.Clear());
                this.treePatterns.ItemsSource = null;
            }
            this.Element = null;
            HidePatternsTree();
        }

        private void treePatterns_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(this.treePatterns.SelectedItem != null)
            {
                dynamic si = this.treePatterns.SelectedItem;
                if (si is ScanListViewItemViewModel)
                {
                    var dic = new Dictionary<string, string>();
                    dic.Add("SelectedPattern", si.Pattern.Name);
                }
            }
        }

        private void copyMenuItemPattern_Click(object sender, RoutedEventArgs e)
        {
            CopyAllPatternsToClipboard();
            e.Handled = true;
        }

        /// <summary>
        /// Copies the available patterns of this element to the clipboard
        /// </summary>
        private void CopyAllPatternsToClipboard()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Properties.Resources.PatternInfoControl_AvailablePatterns);
            // patterns
            foreach (var pt in this.Element.Patterns)
            {
                AddPatternToStringBuilder(sb, pt);
            }
            sb.CopyStringToClipboard();
            sb.Clear();
        }

        /// <summary>
        /// add the pattern and its children to the string builder
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pattern"></param>
        private static void AddPatternToStringBuilder(StringBuilder sb, A11yPattern pattern)
        {
            if (pattern?.Properties != null)
            {
                sb.AppendLine(pattern.Name);
                foreach (var prop in pattern.Properties)
                {
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.PatternInfoControl_PatternPropertyFormat,
                        prop.Name, prop.Value));
                }
            }
        }

        /// <summary>
        /// Only allow copy if patterns exist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treePatterns_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            copyMenuItemPattern.IsEnabled = this.Element?.Patterns != null && this.Element.Patterns.Count > 0;
        }

        /// <summary>
        /// When we do ctrl+c, we only copy the selected tree view item to the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var obj = sender as TreeViewItem;
                var dc = obj.DataContext as PatternViewModel;
                if (dc != null)
                {
                    StringBuilder sb = new StringBuilder();
                    AddPatternToStringBuilder(sb, dc.Pattern);
                    sb.CopyStringToClipboard();
                    sb.Clear();
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Update expanded state when node expanded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem tvi && tvi.DataContext is PatternViewModel vm && vm.Pattern != null)
            {
                    ExpStates[vm.Pattern.Id] = true;
            }
        }

        /// <summary>
        /// Update expanded state when node collapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem tvi && tvi.DataContext is PatternViewModel vm && vm.Pattern != null)
            {
                ExpStates[vm.Pattern.Id] = false;
            }
        }

        /// <summary>
        /// Custom keyboard nav behavior to ensure keyboard nav is consistent within treeview
        /// </summary>
        private void treePatterns_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && Keyboard.FocusedElement is TreeViewItem tvi && (tvi.IsExpanded || tvi.Items.Count == 0))
            {
                // Right arrow: first press expands; second press goes to button
                var header = tvi.Template.FindName("PART_Header", tvi) as ContentPresenter;
                var btn = header.ContentTemplate.FindName("buttonAction", header) as Button;
                e.Handled = btn == null ? false : btn.Focus();
            }
            else if (e.Key == Key.Right && Keyboard.FocusedElement is Button)
            {
                // right arrow on button shouldn't do anything
                e.Handled = true;
            }
            else if (e.Key == Key.Left && Keyboard.FocusedElement is Button btn &&
                btn.TemplatedParent is ContentPresenter header && header.TemplatedParent is TreeViewItem tvi2)
            {
                // left arrow from button goes back to treeviewitem
                e.Handled = tvi2.Focus();
            }
        }
    }
}
