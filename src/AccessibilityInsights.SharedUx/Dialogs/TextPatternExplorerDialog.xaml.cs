// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Desktop.UIAutomation.Patterns;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for TextPatternDialog.xaml
    /// </summary>
    public partial class TextPatternExplorerDialog : Window
    {
        /// <summary>
        /// TextPattern
        /// </summary>
        private TextPattern TextPattern;
        /// <summary>
        /// TextPattern2
        /// </summary>
        private TextPattern2 TextPattern2;
        /// <summary>
        /// Custom list which contains the user-added TextRangeViewModels
        /// </summary>
        private List<TextRangeViewModel> CustomList;

        /// <summary>
        /// indidate the type of source methods
        /// </summary>
        private SourceTypes CurrentSourceType;

        /// <summary>
        /// SourceTypes to indicate the source of Ranges in listbox
        /// </summary>
        internal enum SourceTypes
        {
            Document,
            Visible,
            Selection,
            Caret,
            Listed,
        }

        /// <summary>
        /// Constructor
        /// take both of TextPattern and TextPattern2
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="tp2"></param>
        public TextPatternExplorerDialog(TextPattern tp, TextPattern2 tp2)
        {
            InitializeComponent();

            this.TextPattern = tp;
            this.TextPattern2 = tp2;
            this.cbiCaret.IsEnabled = tp2 != null;
            this.CustomList = new List<TextRangeViewModel>();
            cbRanges.SelectedIndex = 0;
        }

        /// <summary>
        /// Get Ranges based on menu
        /// </summary>
        /// <param name="st">SourceType</param>
        void GetRanges(SourceTypes st)
        {
            this.CurrentSourceType = st;

            try
            {
                switch (st)
                {
                    case SourceTypes.Document:
                        UpdateTextRanges(this.TextPattern.DocumentRange());
                        break;
                    case SourceTypes.Selection:
                        UpdateTextRanges(this.TextPattern.GetSelection());
                        break;
                    case SourceTypes.Visible:
                        UpdateTextRanges(this.TextPattern.GetVisibleRanges());
                        break;
                    case SourceTypes.Caret:
                        int isActive;
                        UpdateTextRanges(this.TextPattern2.GetCaretRange(out isActive));
                        break;
                    case SourceTypes.Listed:
                        UpdateRangesList(this.CustomList);
                        break;
                    default:
                        MessageDialog.Show(Properties.Resources.TextPatternExplorerDialog_GetRanges_This_option_is_not_supported_yet);
                        break;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(string.Format(CultureInfo.InvariantCulture, Properties.Resources.TextPatternExplorerDialog_GetRanges_Failed_to_retrieve_range_s____0, ex.Message));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void UpdateTextRanges(Axe.Windows.Desktop.UIAutomation.Patterns.TextRange textRange)
        {
            UpdateTextRanges(new List<Axe.Windows.Desktop.UIAutomation.Patterns.TextRange>() { textRange });
        }

        private void UpdateTextRanges(IList<Axe.Windows.Desktop.UIAutomation.Patterns.TextRange> list)
        {
            var prefix = this.CurrentSourceType.ToString();
            List<TextRangeViewModel> trvms = new List<TextRangeViewModel>();

            if (list.Count > 1)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    trvms.Add(new TextRangeViewModel(list[i], string.Format(CultureInfo.InvariantCulture, Properties.Resources.TextPatternExplorerDialog_UpdateTextRanges__0___1, prefix, i)));
                }
            }
            else if(list.Count == 1)
            {
                trvms.Add(new TextRangeViewModel(list[0], string.Format(CultureInfo.InvariantCulture, Properties.Resources.TextPatternExplorerDialog_UpdateTextRanges__0, prefix)));
            }

            UpdateRangesList(trvms);
        }

        /// <summary>
        /// Update TextRanges ComboBox
        /// </summary>
        /// <param name="list"></param>
        private void UpdateRangesList(List<TextRangeViewModel> list)
        {
            this.ltbRanges.ItemsSource = null; // clean up first.
            this.ltbRanges.ItemsSource = list;

            if (list.Count != 0)
            {
                if (this.CurrentSourceType == SourceTypes.Listed)
                {
                    this.ltbRanges.SelectedIndex = list.Count - 1; // select the last added item.
                }
                else
                {
                    this.ltbRanges.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Event handler for Range selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ltbRanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedTextRangeInfo();
        }

        /// <summary>
        /// Update TextRange Info with selected TextRangeViewModel
        /// </summary>
        void UpdateSelectedTextRangeInfo()
        {
            this.ctrlTextRangeInfo.SetTextRangeViewModel((TextRangeViewModel)this.ltbRanges.SelectedItem);
        }

        #region GetRanges menu handlers
        /// <summary>
        /// document menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GetRanges(SourceTypes.Document);
        }

        /// <summary>
        /// selection(s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            GetRanges(SourceTypes.Selection);
        }

        /// <summary>
        /// Visible(s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            GetRanges(SourceTypes.Visible);
        }

        /// <summary>
        /// Caret
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniCaret_Click(object sender, RoutedEventArgs e)
        {
            GetRanges(SourceTypes.Caret);
        }

        /// <summary>
        /// Listed Ranges
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            GetRanges(SourceTypes.Listed);
        }
        #endregion

        #region Range menu handlers

        /// <summary>
        /// Get the selected TextRange from Range list.
        /// </summary>
        /// <returns></returns>
        TextRangeViewModel GetSelectedTextRangeViewModel()
        {
            return (TextRangeViewModel)this.ltbRanges.SelectedItem;
        }

        private void mniElementChildren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = GetSelectedTextRangeViewModel().TextRange.GetChildren();
                if (list.Count != 0)
                {
                    ElementInfoDialog dlg = new ElementInfoDialog(list);
                    dlg.Owner = Application.Current.MainWindow;
                    dlg.ShowDialog();
                }
                else
                {
                    MessageDialog.Show(Properties.Resources.NoChildElementFromTextRange);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void mniElementEnclosure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var el = GetSelectedTextRangeViewModel().TextRange.GetEnclosingElement();
                ElementInfoDialog dlg = new ElementInfoDialog(el);
                dlg.Owner = Application.Current.MainWindow;
                dlg.ShowDialog();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void mniRemoveFromSelection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSelectedTextRangeViewModel().TextRange.RemoveFromSelection();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void mniAddToSelection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSelectedTextRangeViewModel().TextRange.AddToSelection();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void mniSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSelectedTextRangeViewModel().TextRange.Select();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void mniScrollIntoBottom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSelectedTextRangeViewModel().TextRange.ScrollIntoView(false);
                UpdateSelectedTextRangeInfo();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void mniScrollIntoTop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSelectedTextRangeViewModel().TextRange.ScrollIntoView(true);
                UpdateSelectedTextRangeInfo();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Use MoveTextRangeDialog for Move or Compare
        /// </summary>
        /// <param name="mode"></param>
        private void RunMoveCompareDialog(MoveTextRangeDialog.OpMode mode)
        {
            try
            {
                MoveTextRangeDialog dlg = new MoveTextRangeDialog(GetSelectedTextRangeViewModel(), mode, this.CustomList, UpdateHilighter);
                dlg.Owner = Application.Current.MainWindow;
                dlg.ShowDialog();
                switch (mode)
                {
                    case MoveTextRangeDialog.OpMode.Move:
                    case MoveTextRangeDialog.OpMode.MoveEndpointByRange:
                    case MoveTextRangeDialog.OpMode.MoveEndpointByUnit:
                        // scroll into first.
                        UpdateSelectedTextRangeInfo();
                        break;
                    case MoveTextRangeDialog.OpMode.Compare:
                    case MoveTextRangeDialog.OpMode.CompareEndpoints:
                        break;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Refresh highliter with new Bounding Rectangle information.
        /// </summary>
        private void UpdateHilighter()
        {
            GetSelectedTextRangeViewModel().TextRange.ScrollIntoView(true);
            this.ctrlTextRangeInfo.RefreshHighlighter();
        }

        private void mniMoveEndPointByUnit_Click(object sender, RoutedEventArgs e)
        {
            RunMoveCompareDialog(MoveTextRangeDialog.OpMode.MoveEndpointByUnit);
        }

        private void mniMoveEndPointByRange_Click(object sender, RoutedEventArgs e)
        {
            RunMoveCompareDialog(MoveTextRangeDialog.OpMode.MoveEndpointByRange);

        }

        private void mniMove_Click(object sender, RoutedEventArgs e)
        {
            RunMoveCompareDialog(MoveTextRangeDialog.OpMode.Move);

        }

        private void mniCompare_Click(object sender, RoutedEventArgs e)
        {
            RunMoveCompareDialog(MoveTextRangeDialog.OpMode.Compare);
        }

        private void mniCompareEndPoints_Click(object sender, RoutedEventArgs e)
        {
            RunMoveCompareDialog(MoveTextRangeDialog.OpMode.CompareEndpoints);
        }

        private void ExpandToEnclosingUnit(UIAutomationClient.TextUnit tu)
        {
            try
            {
                GetSelectedTextRangeViewModel().TextRange.ExpandToEnclosingUnit(tu);
                UpdateSelectedTextRangeInfo();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                MessageDialog.Show(GetExceptionString(e));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void mniExpandByCharacter_Click(object sender, RoutedEventArgs e)
        {
            ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Character);
        }

        private void mniExpandByFormat_Click(object sender, RoutedEventArgs e)
        {
            ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Format);
        }

        private void mniExpandByWord_Click(object sender, RoutedEventArgs e)
        {
            ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Word);
        }

        private void mniExpandByLine_Click(object sender, RoutedEventArgs e)
        {
            ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Line);
        }

        private void mniExpandByParagraph_Click(object sender, RoutedEventArgs e)
        {
            ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Paragraph);
        }

        private void mniExpandByPage_Click(object sender, RoutedEventArgs e)
        {
            ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Page);
        }

        private void mniExpandByDocument_Click(object sender, RoutedEventArgs e)
        {
            ExpandToEnclosingUnit(UIAutomationClient.TextUnit.TextUnit_Document);
        }

        private void mniAddToList_Click(object sender, RoutedEventArgs e)
        {
            var svm = GetSelectedTextRangeViewModel();
            if (svm != null && this.CustomList.Contains(svm) == false)
            {
                AddTextRangeViewModelToCustomList(svm);
            }
            else
            {
                MessageDialog.Show(Properties.Resources.RangeSelectionNotChanged);
            }
        }

        private void mniRemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            var svm = GetSelectedTextRangeViewModel();
            if (svm != null && this.CustomList.Contains(svm))
            {
                this.CustomList.Remove(svm);
                UpdateRangesList(this.CustomList);
            }
        }

        private void mniClone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var svm = GetSelectedTextRangeViewModel();
                var trvm = new TextRangeViewModel(svm.TextRange.Clone(), string.Format(CultureInfo.InvariantCulture, "{0} - cloned", svm.Header));
                AddTextRangeViewModelToCustomList(trvm);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(GetExceptionString(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void AddTextRangeViewModelToCustomList(TextRangeViewModel trvm)
        {
            var dlg = new AddTextRangeToCustomListDialog(trvm.Header);
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                trvm.Header = dlg.TextRangeName;
                trvm.Listed = true;
                this.CustomList.Add(trvm);
                if (cbRanges.SelectedItem == cbiListed)
                {
                    GetRanges(SourceTypes.Listed);
                }
                else
                {
                    cbRanges.SelectedItem = cbiListed;
                }
            }
        }

        private void mniFind_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new TextRangeFindDialog(GetSelectedTextRangeViewModel());
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
        }

        private static string GetExceptionString(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(e.Message);
            sb.AppendFormat(CultureInfo.InvariantCulture, "HResult: 0x{0:X8}", e.HResult);
            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// Key up handler to close window when ESC is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Change range selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                Dispatcher.BeginInvoke(new Action( () => GetRanges((SourceTypes)(e.AddedItems[0] as ComboBoxItem).Tag)), DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Update the range based on currently selected scope
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetRanges((SourceTypes)(cbRanges.SelectedItem as ComboBoxItem).Tag);
        }
    }
}
