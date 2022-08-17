// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Desktop.Types;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for TextRangeControl.xaml
    /// </summary>
#pragma warning disable CA1001
    public partial class TextRangeControl : UserControl
#pragma warning restore CA1001
    {
        private TextRangeViewModel TextRangeViewModel;
        private readonly TextRangeHilighter Hilighter;

        private bool _isArrayCollapsed = true;
        /// <summary>
        /// Should the annotation array in the attribute
        /// list be collapsed into a single row
        /// </summary>
        internal bool IsArrayCollapsed
        {
            get
            {
                return _isArrayCollapsed;
            }

            set
            {
                _isArrayCollapsed = value;
                UpdateAttributeList();
            }
        }
        public TwoStateButtonViewModel vmHilighter { get; private set; } = new TwoStateButtonViewModel(ButtonState.On);

        /// <summary>
        /// Constructor
        /// </summary>
        public TextRangeControl()
        {
            this.Hilighter = new TextRangeHilighter();

            InitializeComponent();

            this.mniWhitespace.IsChecked = ConfigurationManager.GetDefaultInstance().AppConfig.ShowWhitespaceInTextPatternViewer;
            this.listAttributes.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyLVItems));
        }

        /// <summary>
        /// Copy the selected items of attributes listview
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void CopyLVItems(object source, ExecutedRoutedEventArgs e)
        {
            ListView lv = e.OriginalSource as ListView;
            StringBuilder sb = new StringBuilder();
            foreach (var item in lv.SelectedItems)
            {
                if (item is TextAttributeViewModel vm)
                {
                    sb.AppendLine(vm.ToString());
                }
            }
            sb.CopyStringToClipboard();
            sb.Clear();
        }

        /// <summary>
        /// Set the TextRangeViewModel to update UI
        /// </summary>
        /// <param name="vm"></param>
        public void SetTextRangeViewModel(TextRangeViewModel vm)
        {
            this.TextRangeViewModel = vm;

            if (vm != null)
            {
                this.btnHilight.Visibility = Visibility.Visible;

                UpdateAttributeList();
                RefreshHighlighter();
            }
            else
            {
                CleanUi();
            }
        }

        /// <summary>
        /// Updates the attribute list
        /// </summary>
        private void UpdateAttributeList()
        {
            this.tbText.Text = TextRangeViewModel.GetText(mniWhitespace.IsChecked);

            this.textboxSearch.Text = "";
            var list = from p in TextRangeViewModel.GetAttributes(IsArrayCollapsed)
                       select p;
            if (this.mniShowAll.IsChecked)
            {
                // This is an outer join of the TP's attributes and the user's list of core attributes, sorted alphabetically
                // If a selected attribute is not present in the TP, the VM is created with a null value
                var coreAtts = from att in ConfigurationManager.GetDefaultInstance().AppConfig.CoreTPAttributes select new TextAttributeViewModel(att);
                list = list.Concat(coreAtts).ToLookup(p => p.Id).Select(g => g.Aggregate((p1, p2) => p1)).OrderBy(l => l.Name);
                this.listAttributes.ItemsSource = list;
            }
            else
            {
                // This populates the list with the user's selected attributes in the user's order
                // If a selected attribute is not present in the TP, the VM is created with a null value
                this.listAttributes.ItemsSource = from att in ConfigurationManager.GetDefaultInstance().AppConfig.CoreTPAttributes
                                                  join l in list
                                                  on att equals l.Id into newList
                                                  from l in newList.DefaultIfEmpty(new TextAttributeViewModel(att))
                                                  select l;
            }
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listAttributes.ItemsSource);
            view.Filter = NameFilter;

        }

        /// <summary>
        /// Clean up UI to show nothing.
        /// </summary>
        private void CleanUi()
        {
            this.Hilighter.HilightBoundingRectangles(false);

            this.tbText.Text = null;
            this.btnHilight.Visibility = Visibility.Hidden;

            this.listAttributes.ItemsSource = null;
        }

        /// <summary>
        /// Filter logic for listview
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool NameFilter(object item)
        {
            if (String.IsNullOrEmpty(textboxSearch.Text))
                return true;
            else
            {

                string name = (string)((TextAttributeViewModel)item).Name;
                return (name.IndexOf(textboxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        /// <summary>
        /// Handle text change for update list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textboxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.listAttributes.ItemsSource != null && (this.textboxSearch.Text.Length >= 2 || this.textboxSearch.Text.Length == 0))
            {
                CollectionViewSource.GetDefaultView(this.listAttributes.ItemsSource).Refresh();
            }
        }

        /// <summary>
        /// Handle highlighter on/off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHilight_Click(object sender, RoutedEventArgs e)
        {
            this.vmHilighter.FlipButtonState();
            RefreshHighlighter();
        }

        /// <summary>
        /// Handle UI unloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUi();
        }

        /// <summary>
        /// Refresh Highlighter by getting new bounding rectangles
        /// based on the highlighter state, turn on or off.
        /// </summary>
        internal void RefreshHighlighter()
        {
            if (this.Hilighter != null && this.TextRangeViewModel != null)
            {
                this.Hilighter.SetBoundingRectangles(this.TextRangeViewModel.BoundingRectangles);
                this.Hilighter.HilightBoundingRectangles(this.vmHilighter.State == ButtonState.On);
            }
        }

        /// <summary>
        /// Toggles the collapse annotations array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsArrayCollapsed = (sender as MenuItem).IsChecked;
        }

        /// <summary>
        /// Show all attributes check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowAll_Click(object sender, RoutedEventArgs e)
        {
            UpdateAttributeList();
        }

        /// <summary>
        /// Configure attributes to show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniConfig_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PropertyConfigDialog(ConfigurationManager.GetDefaultInstance().AppConfig.CoreTPAttributes, TextAttributeType.GetInstance(), "Attributes")
            {
                Owner = Window.GetWindow(this)
            };
            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                ConfigurationManager.GetDefaultInstance().AppConfig.CoreTPAttributes = dlg.ctrlPropertySelect.SelectedList.Select(v => v.Id).ToList();
                UpdateAttributeList();
            }
        }

        /// <summary>
        /// Toggle whitespace symbols
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniWhitespace_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.GetDefaultInstance().AppConfig.ShowWhitespaceInTextPatternViewer = mniWhitespace.IsChecked;
            this.tbText.Text = TextRangeViewModel.GetText(mniWhitespace.IsChecked);
        }
    }
}
