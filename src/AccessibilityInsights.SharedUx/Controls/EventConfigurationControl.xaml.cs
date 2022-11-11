// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Misc;
using AccessibilityInsights.SharedUx.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for EventConfigurationControl.xaml
    /// </summary>
    public partial class EventConfigurationControl : UserControl
    {
        public IList<RecordEntitySetting> List { get; private set; }
        public static readonly RoutedCommand AddEventCommand = new RoutedCommand();
        public static readonly RoutedCommand RemoveEventCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToAvailableEventsCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToSelectedEventsCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToSearchCommand = new RoutedCommand();

        /// <summary>
        /// List on right side in user's order
        /// </summary>
        public IList<RecordEntitySetting> SelectedList { get; private set; }

        /// <summary>
        /// Dependency property to allow drag n drop reordering
        /// </summary>
        public static readonly DependencyProperty CanDragReorderProperty =
        DependencyProperty.Register("CanDragReorder", typeof(bool), typeof(EventConfigurationControl),
            new PropertyMetadata(new PropertyChangedCallback(CanDragReorderPropertyChanged)));

        /// <summary>
        /// CanDragReorder Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CanDragReorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EventConfigurationControl;
            if (control != null)
            {
                control.CanDragReorder = (bool)e.NewValue;
                if (control.CanDragReorder)
                {
                    control.gdReorder.Visibility = Visibility.Visible;
                }
                else
                {
                    control.gdReorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool CanDragReorder
        {
            get
            {
                return (bool)this.GetValue(CanDragReorderProperty);
            }
            set
            {
                this.SetValue(CanDragReorderProperty, value);
            }
        }

        /// <summary>
        /// Dependency property to show properties column labels
        /// </summary>
        public static readonly DependencyProperty ShowColumnLabelsProperty =
        DependencyProperty.Register("ShowColumnLabels", typeof(bool), typeof(EventConfigurationControl),
            new PropertyMetadata(new PropertyChangedCallback(ShowColumnLabelsPropertyChanged)));

        /// <summary>
        /// ShowColumnLabels Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ShowColumnLabelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EventConfigurationControl;
            if (control != null)
            {
                control.ShowColumnLabels = (bool)e.NewValue;
                if ((bool)e.NewValue)
                {
                    control.grdColLabels.Visibility = Visibility.Visible;
                }
                else
                {
                    control.grdColLabels.Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool ShowColumnLabels
        {
            get
            {
                return (bool)this.GetValue(ShowColumnLabelsProperty);
            }
            set
            {
                this.SetValue(ShowColumnLabelsProperty, value);
            }
        }

        /// <summary>
        /// Dependency property to set left column label
        /// </summary>
        public static readonly DependencyProperty LeftColumnTitleProperty =
        DependencyProperty.Register("LeftColumnTitle", typeof(string), typeof(EventConfigurationControl),
            new PropertyMetadata(new PropertyChangedCallback(LeftColumnTitlePropertyChanged)));

        /// <summary>
        /// LeftColumnTitle Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void LeftColumnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EventConfigurationControl;
            if (control != null)
            {
                control.LeftColumnTitle = e.NewValue as string;
                control.lblLeftCol.Content = e.NewValue as string;
            }
        }

        public string LeftColumnTitle
        {
            get
            {
                return this.GetValue(LeftColumnTitleProperty) as string;
            }
            set
            {
                this.SetValue(LeftColumnTitleProperty, value);
            }
        }

        /// <summary>
        /// Dependency property to set right column label
        /// </summary>
        public static readonly DependencyProperty RightColumnTitleProperty =
        DependencyProperty.Register("RightColumnTitle", typeof(string), typeof(EventConfigurationControl),
            new PropertyMetadata(new PropertyChangedCallback(RightColumnTitlePropertyChanged)));

        /// <summary>
        /// RightColumnTitle Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void RightColumnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EventConfigurationControl;
            if (control != null)
            {
                control.RightColumnTitle = e.NewValue as string;
                control.lblRightCol.Content = e.NewValue as string;
            }
        }

        public string RightColumnTitle
        {
            get
            {
                return this.GetValue(RightColumnTitleProperty) as string;
            }
            set
            {
                this.SetValue(RightColumnTitleProperty, value);
            }
        }

        public EventConfigurationControl()
        {
            InitializeComponent();

            InitCommandBindings();
        }

        void InitCommandBindings()
        {
            var bindings = CreateCommandBindings();
            this.CommandBindings.AddRange(bindings);
        }

        private CommandBinding[] CreateCommandBindings()
        {
            return new CommandBinding[]{
                new CommandBinding(AddEventCommand, this.btnMoveRight_Click),
                    new CommandBinding(RemoveEventCommand, this.btnMoveLeft_Click),
            new CommandBinding(MoveFocusToAvailableEventsCommand, this.OnMoveFocusToAvailableEventsList),
            new CommandBinding(MoveFocusToSelectedEventsCommand, this.OnMoveFocusToSelectedEventsList),
            new CommandBinding(MoveFocusToSearchCommand, OnMoveFocusToSearchField) };
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Set the list of all events and generate selected list
        /// </summary>
        /// <param name="list"></param>
        public void SetList(IList<RecordEntitySetting> list)
        {
            this.List = list;
            this.SelectedList = this.List.Where(l => l.IsRecorded).ToList();
            UpdateListViews();
        }

        /// <summary>
        /// Set list and selected list
        /// </summary>
        /// <param name="list"></param>
        public void SetLists(IList<RecordEntitySetting> list, IList<RecordEntitySetting> selList)
        {
            this.List = list;
            this.SelectedList = selList;
            UpdateListViews();
        }

        private void UpdateListViews()
        {
#pragma warning disable CA1508 // Dead code warning doesn't apply here
            using (new ListViewSelectionLock(this.lvLeft))
            using (new ListViewSelectionLock(this.lvRight))
            {
                if (CanDragReorder)
                {
                    SetItemsSource(this.lvRight, this.SelectedList);
                }
                else
                {
                    SetItemsSource(this.lvRight, this.SelectedList.OrderBy(l => l.Name));
                }
                SetItemsSource(this.lvLeft, this.List.Where(l => l.IsRecorded == false).OrderBy(l => l.Name));

                textboxSearch.Text = "";
                FireAsyncContentLoadedEvent();
            } // using
#pragma warning restore CA1508 // Dead code warning doesn't apply here
        }

        private void FireAsyncContentLoadedEvent()
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.AsyncContentLoaded))
            {
                UserControlAutomationPeer peer = UIElementAutomationPeer.FromElement(this) as UserControlAutomationPeer;
                peer?.RaiseAsyncContentLoadedEvent(new AsyncContentLoadedEventArgs(AsyncContentLoadedState.Completed, 100));
            }
        }

        private void SetItemsSource(ListView listview, IEnumerable<RecordEntitySetting> items)
        {
            listview.ItemsSource = items;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listview.ItemsSource);
            view.Filter = NameFilter;
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
                string name = (string)item.GetType().GetProperty("Name").GetValue(item);
                return (name.IndexOf(textboxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void textboxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(this.lvLeft.ItemsSource).Refresh();
            CollectionViewSource.GetDefaultView(this.lvRight.ItemsSource).Refresh();
            FireAsyncContentLoadedEvent();
        }

        private void btnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            int old = lvLeft.SelectedIndex; // this is used to preserve the user's previous position in the list

            foreach (RecordEntitySetting l in this.lvLeft.SelectedItems)
            {
                SelectedList.Add(l);
                l.IsRecorded = true;
            }

            UpdateListViews();
            lvLeft.SelectedIndex = old < lvLeft.Items.Count ? old : lvLeft.Items.Count - 1;
        }

        private void btnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            int old = lvRight.SelectedIndex; // this is used to preserve the user's previous position in the list

            foreach (RecordEntitySetting l in this.lvRight.SelectedItems)
            {
                SelectedList.Remove(l);
                l.IsRecorded = false;
            }

            UpdateListViews();
            lvRight.SelectedIndex = old < lvRight.Items.Count ? old : lvRight.Items.Count - 1;
        }

        private void OnMoveFocusToAvailableEventsList(object sender, RoutedEventArgs e)
        {
            MoveFocusToListView(this.lvLeft);
        }

        private void OnMoveFocusToSelectedEventsList(object sender, RoutedEventArgs e)
        {
            MoveFocusToListView(this.lvRight);
        }

        /// <summary>
        /// Handles moving focus to the first child of a list view
        /// </summary>
        /// <param name="lv"></param>
        private static void MoveFocusToListView(ListView lv)
        {
            if (lv == null)
                return;
            if (lv.IsKeyboardFocusWithin)
                return;
            if (lv.Items.Count <= 0)
                return;

            var traversalRequest = new TraversalRequest(FocusNavigationDirection.First);
            lv.MoveFocus(traversalRequest);
        }

        private void OnMoveFocusToSearchField(object sender, RoutedEventArgs e)
        {
            this.textboxSearch.Focus();
        }

        /// <summary>
        /// Handles dropping listviewitem for reordering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvRight_Drop(object sender, DragEventArgs e)
        {
            if (CanDragReorder)
            {
                RecordEntitySetting droppedData = e.Data.GetData(typeof(RecordEntitySetting)) as RecordEntitySetting;
                RecordEntitySetting target = ((ListBoxItem)sender).DataContext as RecordEntitySetting;

                int sourcePos = SelectedList.IndexOf(droppedData);
                int destPos = SelectedList.IndexOf(target);

                if (sourcePos < destPos)
                {
                    SelectedList.Insert(destPos + 1, droppedData);
                    SelectedList.RemoveAt(sourcePos);
                }
                else
                {
                    sourcePos++;

                    if (SelectedList.Count + 1 > sourcePos)
                    {
                        SelectedList.Insert(destPos, droppedData);
                        SelectedList.RemoveAt(sourcePos);
                    }
                }

                lvRight.ItemsSource = SelectedList.ToList();
                UpdateReorderButtons();
            }
        }

        /// <summary>
        /// Lets user "grab" listviewitem to reorder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (CanDragReorder && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        /// <summary>
        /// Add visual indicator of drag n drop location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            if (CanDragReorder)
            {
                var lvi = sender as ListViewItem;
                RecordEntitySetting droppedData = e.Data.GetData(typeof(RecordEntitySetting)) as RecordEntitySetting;
                RecordEntitySetting target = ((ListBoxItem)sender).DataContext as RecordEntitySetting;
                int sourcePos = SelectedList.IndexOf(droppedData);
                int destPos = SelectedList.IndexOf(target);

                lvi.BorderBrush = new SolidColorBrush(Colors.Black);

                if (sourcePos > destPos)
                {
                    lvi.BorderThickness = new Thickness(0, 1, 0, 0);
                    lvi.Padding = new Thickness(5, 1, 5, 2);
                }
                else if (sourcePos < destPos)
                {
                    lvi.BorderThickness = new Thickness(0, 0, 0, 1);
                    lvi.Padding = new Thickness(5, 2, 5, 1);
                }
            }
        }

        /// <summary>
        /// Remove bar when dragged item no longer above listviewitem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_DragLeave(object sender, DragEventArgs e)
        {
            if (CanDragReorder)
            {
                var lvi = sender as ListViewItem;

                lvi.BorderBrush = null;
                lvi.BorderThickness = new Thickness(1);
                lvi.Padding = new Thickness(4, 1, 4, 1);
            }
        }

        /// <summary>
        /// Switch listviewitem side on double-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lvi = sender as ListViewItem;
            var listView = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
            var records = from l in this.List where l.Id == (lvi.Content as RecordEntitySetting).Id select l;
            var record = records.FirstOrDefault();

            if (listView == lvLeft)
            {
                SelectedList.Add(record);
                record.IsRecorded = true;
            }
            else
            {
                SelectedList.Remove(lvi.Content as RecordEntitySetting);
                record.IsRecorded = false;
            }
            UpdateListViews();
        }

        /// <summary>
        /// Move selected item up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var list = from RecordEntitySetting item in this.lvRight.SelectedItems
                       orderby SelectedList.IndexOf(item) ascending
                       select new Tuple<RecordEntitySetting, int>(item, SelectedList.IndexOf(item));

            foreach (var l in list)
            {
                SelectedList.Remove(l.Item1);
                SelectedList.Insert(l.Item2 - 1, l.Item1);
            }

            UpdateReorderButtons();
            UpdateListViews();
        }

        /// <summary>
        /// Move selected item down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var list = from RecordEntitySetting item in this.lvRight.SelectedItems
                       orderby SelectedList.IndexOf(item) descending
                       select new Tuple<RecordEntitySetting, int>(item, SelectedList.IndexOf(item));

            foreach (var l in list)
            {
                SelectedList.Remove(l.Item1);
                SelectedList.Insert(l.Item2 + 1, l.Item1);
            }

            UpdateReorderButtons();
            UpdateListViews();
        }

        /// <summary>
        /// Updated buttons when selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvRight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CanDragReorder)
            {
                UpdateReorderButtons();
            }
        }

        /// <summary>
        /// Update reorder buttons based on selections
        /// </summary>
        private void UpdateReorderButtons()
        {
            if (lvRight.SelectedItems.Count == 0)
            {
                btnMoveUp.IsEnabled = false;
                btnMoveDown.IsEnabled = false;
            }
            else
            {
                btnMoveUp.IsEnabled = !lvRight.SelectedItems.Contains(SelectedList[0]);
                btnMoveDown.IsEnabled = !lvRight.SelectedItems.Contains(SelectedList.Last());
            }
        }

        /// <summary>
        /// In this situation (but not most others), high contrast mode overrides our focus style with the disappointing default.
        /// To avoid having to reconstruct the entire ListViewItem ControlTemplate, we can just set it back to our focus style
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (SystemParameters.HighContrast)
            {
                (sender as ListViewItem).FocusVisualStyle = this.Resources[SystemParameters.FocusVisualStyleKey] as Style;
            }
        }
    }
}
