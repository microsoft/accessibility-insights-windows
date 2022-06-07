// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Actions.Contexts;
using System;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class AutomatedChecksCustomListControlContext
    {
        internal ElementContext ElementContext { get; }
        internal ElementDataContext DataContext => ElementContext.DataContext;
        internal Action NotifyElementSelected { get; }
        internal Action SwitchToServerLogin { get; }

        public AutomatedChecksCustomListControlContext(ElementContext elementContext, Action notifyElementSelected, Action switchToServerLogin)
        {
            ElementContext = elementContext ?? throw new ArgumentNullException(nameof(elementContext));
            NotifyElementSelected = notifyElementSelected ?? throw new ArgumentNullException(nameof(notifyElementSelected));
            SwitchToServerLogin = switchToServerLogin ?? throw new ArgumentNullException(nameof(switchToServerLogin));
        }
    }
}
