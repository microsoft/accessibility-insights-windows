// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Interaction logic for ScannerResultCustomListControl.xaml
    /// </summary>
    public partial class ScannerResultCustomListControl : UserControl
    {
        private ScannerResultCustomListContext _controlContext;

        public IEnumerable ItemsSource
        {
            get => lvDetails.ItemsSource;
            set { lvDetails.ItemsSource = value; }
        }

        public int SelectedIndex => lvDetails.SelectedIndex;

        public ViewBase View => this.gvRules;

        /// <summary>
        /// Keeps track of if we should automatically set lv column widths
        /// </summary>
        public bool HasUserResizedLvHeader { get; set; }

        #region DataGridAccessibleName (Dependency Property)

        public string DataGridAccessibleName
        {
            get { return (string)GetValue(DataGridAccessibleNameProperty); }
            set { SetValue(DataGridAccessibleNameProperty, value); }
        }

        public static readonly DependencyProperty DataGridAccessibleNameProperty =
            DependencyProperty.Register("DataGridAccessibleName", typeof(string), typeof(ScannerResultCustomListControl), new PropertyMetadata(null));

        #endregion

        public ScannerResultCustomListControl()
        {
            InitializeComponent();
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        internal void SetControlContext(ScannerResultCustomListContext controlContext)
        {
            _controlContext = controlContext ?? throw new ArgumentNullException(nameof(controlContext));
        }

        /// <summary>
        /// Make bug column fixed width
        /// </summary>
        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb senderAsThumb = e.OriginalSource as Thumb;
            GridViewColumnHeader header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if ((header.Content as string) == Properties.Resources.ScannerResultControl_Thumb_DragDelta_Rule)
            {
                HasUserResizedLvHeader = true;
            }
            if ((header.Content as string) == Properties.Resources.ScannerResultControl_Thumb_DragDelta_Issue)
            {
                header.Column.Width = HelperMethods.FileIssueColumnWidth;
            }
        }

        /// <summary>
        /// Simulate * width for rule column
        /// </summary>
        private void lvDetails_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!HasUserResizedLvHeader)
            {
                ListView listView = sender as ListView;
                GridView gView = listView.View as GridView;

                if (gView.Columns.Count >= 2)
                {
                    var width = (listView.ActualWidth - SystemParameters.VerticalScrollBarWidth) - gView.Columns[1].ActualWidth;
                    //leave the width as it was, if the resulting width goes in negative.
                    if (width >= 0)
                    {
                        gView.Columns[0].Width = width;
                    }
                }
            }
        }

        /// <summary>
        /// Update control based on failure selection
        /// </summary>
        private void lvDetails_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            _controlContext.ElementToBind.DataContext = (e.AddedItems.Count > 0) ? (ScanListViewItemViewModel)e.AddedItems[0] : null;
        }

        /// <summary>
        /// Pass scrolling events through listview
        /// </summary>
        private void lvDetails_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        /// <summary>
        /// Ensure listview contents are displayed properly
        /// </summary>
        private void lvDetails_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
#pragma warning restore CA1801 // unused parameter
        {
            if ((bool)e.NewValue)
            {
                _controlContext.ChangeVisibility();
            }
        }

        /// <summary>
        /// Navigate to link on enter in listview
        /// </summary>
        private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((sender as ListViewItem).DataContext as ScanListViewItemViewModel).InvokeHelpLink();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles click on file bug button
        /// </summary>
        private void btnFileBug_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((Button)sender).Tag as ScanListViewItemViewModel;
            var input = new FileIssueWrapperInput(
                vm,
                _controlContext.EcId,
                _controlContext.SwitchToServerLogin,
                vm.GetIssueInformation,
                FileBugRequestSource.HowtoFix,
                Properties.Resources.ScannerResultControl_btnFileBug_Click_File_Issue_Configure);
            FileIssueWrapper.FileIssueFromControl(input);
        }

        /// <summary>
        /// Custom keyboard nav behavior for listview
        /// </summary>
        private void ListViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            (sender as ListViewItem).SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.Local);

            if (e.Key == Key.Right && !(Keyboard.FocusedElement is Button))
            {
                MoveFocus(FocusNavigationDirection.Next);
            }
            else if (e.Key == Key.Left && !(Keyboard.FocusedElement is ListViewItem))
            {
                MoveFocus(FocusNavigationDirection.Previous);
            }

            (sender as ListViewItem).SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ((sender as Hyperlink).DataContext as ScanListViewItemViewModel).InvokeHelpLink();
        }

        /// <summary>
        /// Moves focus from currently focused element in given direction
        /// </summary>
        private static void MoveFocus(FocusNavigationDirection dir)
        {
            if (Keyboard.FocusedElement is FrameworkElement fe)
            {
                fe.MoveFocus(new TraversalRequest(dir));

            }
            else if (Keyboard.FocusedElement is FrameworkContentElement fce)
            {
                fce.MoveFocus(new TraversalRequest(dir));
            }
        }
    }
}
