// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    internal class CustomListView : ListView
    {
        private readonly List<CustomGridViewColumn> columns = new List<CustomGridViewColumn>();

        internal CustomListView() : base()
        {
            Initialized += CustomListView_Initialized;
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
                    if (!columns.Contains(customCol)) continue;

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
