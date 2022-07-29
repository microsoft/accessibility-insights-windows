// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Misc
{
    /// <summary>
    /// Attempts to maintain the closest possible selection index in a list view
    /// whose items may be removed or added while the list view has focus.
    /// </summary>
    class ListViewSelectionLock : IDisposable
    {
        private readonly ListView ListView;
        private readonly bool HadFocus;
        private readonly int SelectedIndex;

        public ListViewSelectionLock(ListView listView)
        {
            this.ListView = listView ?? throw new ArgumentNullException(nameof(listView));
            this.HadFocus = listView.IsKeyboardFocusWithin;
            this.SelectedIndex = listView.SelectedIndex;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!HadFocus)
                        return;
                    if (this.ListView.Items.Count <= 0)
                        return;
                    if (this.SelectedIndex < 0)
                        return;

                    var newIndex = this.SelectedIndex >= this.ListView.Items.Count
                        ? this.ListView.Items.Count - 1
                        : this.SelectedIndex;

                    this.ListView.SelectedIndex = newIndex;
                    this.ListView.UpdateLayout();
                    var item = (ListViewItem)this.ListView.ItemContainerGenerator.ContainerFromIndex(newIndex);
                    item?.Focus();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    } //class
} //namespace
