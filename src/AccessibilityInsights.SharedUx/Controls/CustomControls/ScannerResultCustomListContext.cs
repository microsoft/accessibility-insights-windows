// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class ScannerResultCustomListContext
    {
        internal Action UpdateTree { get; }
        internal Action SwitchToServerLogin { get; }
        internal Action ChangeVisibility { get; }
        internal FrameworkElement ElementToBind { get; }
        internal Guid EcId { get; }

        public ScannerResultCustomListContext(Action updateTree, Action switchToServerLogin, Action changeAction, FrameworkElement dataContext, Guid ecId)
        {
            UpdateTree = updateTree ?? throw new ArgumentNullException(nameof(updateTree));
            SwitchToServerLogin = switchToServerLogin ?? throw new ArgumentNullException(nameof(switchToServerLogin));
            ChangeVisibility = changeAction ?? throw new ArgumentNullException(nameof(changeAction));
            ElementToBind = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            EcId = ecId;
        }
    }
}
