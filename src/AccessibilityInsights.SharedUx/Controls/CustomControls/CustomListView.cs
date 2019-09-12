// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class CustomListView : ListView
    {
        public CustomListView() : base()
        {
            Initialized += CustomListView_Initialized;
        }

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
            foreach (var col in e.NewItems)
            {
                if (col is CustomGridViewColumn customCol)
                {
                    UpdateColumn(customCol);
                }
            }
        }

        private void UpdateColumn(CustomGridViewColumn customCol)
        {
            var gridName = $"Column_{Guid.NewGuid().ToString().Replace("-", "")}";
            this.RegisterName(gridName, customCol);
            customCol.SetName = gridName;
        }
    }
}
