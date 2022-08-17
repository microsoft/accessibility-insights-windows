// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// A ListView styled to look like a DataGrid, complete with some custom column resizing behavior.
    /// Meant to be used in conjunction with <see cref="CustomGridViewColumn"/>.
    /// </summary>
    public class CustomListView : ListView
    {
        private readonly List<CustomGridViewColumn> columns = new List<CustomGridViewColumn>();

        public CustomListView() : base()
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, HandleCopy, HandleCanCopy));
            Initialized += CustomListView_Initialized;
            PreviewMouseWheel += CustomListView_PreviewMouseWheel;
        }

        private void CustomListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = MouseWheelEvent,
                    Source = sender
                };
                var parent = (sender as Control)?.Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }

        private void HandleCanCopy(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = SelectedItems.Count > 0;

        private void HandleCopy(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (PropertyListViewItemModel vm in SelectedItems)
            {
                var line = $"{vm.Name}\t{vm.Value ?? Properties.Resources.PropertyDoesNotExist}";
                sb.AppendLine(line);
            }

            sb.CopyStringToClipboard();
            sb.Clear();
        }

        internal void UpdateAllColumnWidths() => columns.ForEach(col => col.UpdateWidth());

        private void CustomListView_Initialized(object sender, EventArgs e)
        {
            (View as GridView).Columns.CollectionChanged += Columns_CollectionChanged;

            foreach (var col in (View as GridView).Columns)
            {
                if (col is CustomGridViewColumn customCol)
                {
                    UpdateColumn(customCol);
                }
            }
        }

        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddNewColumns(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    AddNewColumns(e.NewItems);
                    RemoveOldColumns(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldColumns(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    columns.Clear();
                    break;
            }
        }

        private void AddNewColumns(IList newItems)
        {
            foreach (var col in newItems)
            {
                if (col is CustomGridViewColumn customCol)
                {
                    UpdateColumn(customCol);
                }
            }
        }

        private void RemoveOldColumns(IList oldItems)
        {
            foreach (var col in oldItems)
            {
                if (col is CustomGridViewColumn customCol)
                {
                    if (!columns.Contains(customCol))
                        continue;

                    columns.Remove(customCol);
                }
            }
        }

        private void UpdateColumn(CustomGridViewColumn customCol)
        {
            var gridName = $"Column_{Guid.NewGuid().ToString().Replace("-", "")}";

            RegisterName(gridName, customCol);
            customCol.RegisteredName = gridName;
            columns.Add(customCol);
        }
    }
}
