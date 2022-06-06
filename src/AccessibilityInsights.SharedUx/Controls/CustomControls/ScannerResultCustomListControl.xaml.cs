using System;
using System.Collections.Generic;
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

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Interaction logic for ScannerResultCustomListControl.xaml
    /// </summary>
    public partial class ScannerResultCustomListControl : UserControl
    {
        public ScannerResultCustomListControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Simulate * width for rule column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvDetails_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*if (!HasUserResizedLvHeader)
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
            }*/
        }

        private void lvDetails_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.spHowToFix.DataContext = (e.AddedItems.Count > 0) ? (ScanListViewItemViewModel)e.AddedItems[0] : null;
        }

        private void lvDetails_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            /*if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }*/
        }

        private void lvDetails_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
#pragma warning restore CA1801 // unused parameter
        {
            /*if ((bool)e.NewValue)
            {
                var visible = this.btnShowAll.Visibility;
                this.ShowAllResults = visible == Visibility.Collapsed;
                UpdateTree();
                this.btnShowAll.Visibility = visible;
            }*/
        }

        private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.Enter)
            {
                ((sender as ListViewItem).DataContext as ScanListViewItemViewModel).InvokeHelpLink();
                e.Handled = true;
            }*/
        }

        private void btnFileBug_Click(object sender, RoutedEventArgs e)
        {
            /*var vm = ((Button)sender).Tag as ScanListViewItemViewModel;
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
                        FileIssueAction.AttachIssueData(issueInformation, this.EcId, vm.Element.BoundingRectangle, vm.Element.UniqueId);
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
                        SwitchToServerLogin();
                    }
                }
            }*/
        }

        private void ListViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            /*(sender as ListViewItem).SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.Local);

            if (e.Key == Key.Right && !(Keyboard.FocusedElement is Button))
            {
                MoveFocus(FocusNavigationDirection.Next);
            }
            else if (e.Key == Key.Left && !(Keyboard.FocusedElement is ListViewItem))
            {
                MoveFocus(FocusNavigationDirection.Previous);
            }

            (sender as ListViewItem).SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None);*/
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            //((sender as Hyperlink).DataContext as ScanListViewItemViewModel).InvokeHelpLink();
        }
    }


}
