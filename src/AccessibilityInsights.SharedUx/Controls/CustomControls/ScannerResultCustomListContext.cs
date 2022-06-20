// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class ScannerResultCustomListContext
    {
        internal Action UpdateTree { get; }
        internal Action SwitchToServerLogin { get; }
        internal Action<ScannerResultCustomListControl, SelectionChangedEventArgs> ItemSelectedHandler { get; }
        internal Guid EcId { get; }

        public ScannerResultCustomListContext(Action updateTree, Action switchToServerLogin, Action<ScannerResultCustomListControl, SelectionChangedEventArgs> itemSelectedHandler, Guid ecId)
        {
            UpdateTree = updateTree ?? throw new ArgumentNullException(nameof(updateTree));
            SwitchToServerLogin = switchToServerLogin ?? throw new ArgumentNullException(nameof(switchToServerLogin));
            ItemSelectedHandler = itemSelectedHandler ?? throw new ArgumentNullException(nameof(itemSelectedHandler));
            EcId = ecId;
        }
    }
}
