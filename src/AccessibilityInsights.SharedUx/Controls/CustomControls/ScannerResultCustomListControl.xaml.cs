using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AccessibilityInsights.SharedUx.ViewModels;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.FileIssue;
using System.IO;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Enums;

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

        public int SelectedIndex { get; set; }

        public ViewBase View { get; set; }


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
        }

        internal void SetControlContext(ScannerResultCustomListContext controlContext)
        {
            _controlContext = controlContext ?? throw new ArgumentNullException(nameof(controlContext));
        }

        /// <summary>
        /// Simulate * width for rule column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvDetails_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            _controlContext.DataContext.DataContext = (e.AddedItems.Count > 0) ? (ScanListViewItemViewModel)e.AddedItems[0] : null;
        }

        /// <summary>
        /// Pass scrolling events through listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvDetails_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
#pragma warning restore CA1801 // unused parameter
        {
            if ((bool)e.NewValue)
            {
                _controlContext.changeVisibility();
            }
        }

        /// <summary>
        /// Navigate to link on enter in listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileBug_Click(object sender, RoutedEventArgs e)
        {
            var vm = ((Button)sender).Tag as ScanListViewItemViewModel;
            if (vm.IssueLink != null)
            {
                // Bug already filed, open it in a new window
                try
                {
                    System.Diagnostics.Process.Start(vm.IssueLink.OriginalString);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    ex.ReportException();
                    // Happens when bug is deleted, message describes that work item doesn't exist / possible permission issue
                    MessageDialog.Show(ex.InnerException?.Message);
                    vm.IssueDisplayText = null;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            else
            {
                // File a new bug
                var telemetryEvent = TelemetryEventFactory.ForIssueFilingRequest(FileBugRequestSource.HowtoFix);
                Logger.PublishTelemetryEvent(telemetryEvent);

                if (IssueReporter.IsConnected)
                {
                    IssueInformation issueInformation = null;
                    try
                    {
                        issueInformation = vm.GetIssueInformation();
                        FileIssueAction.AttachIssueData(issueInformation, _controlContext.EcId, vm.Element.BoundingRectangle, vm.Element.UniqueId);
                        IIssueResult issueResult = FileIssueAction.FileIssueAsync(issueInformation);
                        if (issueResult != null)
                        {
                            vm.IssueDisplayText = issueResult.DisplayText;
                            vm.IssueLink = issueResult.IssueLink;
                        }
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex)
                    {
                        ex.ReportException();
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                    finally
                    {
                        if (issueInformation != null && File.Exists(issueInformation.TestFileName))
                        {
                            File.Delete(issueInformation.TestFileName);
                        }
                    }
                }
                else
                {
                    bool? accepted = MessageDialog.Show(Properties.Resources.ScannerResultControl_btnFileBug_Click_File_Issue_Configure);
                    if (accepted.HasValue && accepted.Value)
                    {
                        _controlContext.SwitchToServerLogin();
                    }
                }
            }
        }

        /// <summary>
        /// Custom keyboard nav behavior for listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="dir"></param>
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
