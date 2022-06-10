using System;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class ScannerResultCustomListContext
    {
        internal Action UpdateTree { get; }
        internal Action SwitchToServerLogin { get; }
        internal bool ShowAllResults;
        internal Button BtnShowAll;
        internal FrameworkElement DataContext;
        internal Guid EcId;

        public ScannerResultCustomListContext( Action updateTree, Action switchToServerLogin, Button btnShowAll, FrameworkElement dataContext, Guid ecId)
        {
            UpdateTree = updateTree ?? throw new ArgumentNullException(nameof(updateTree));
            SwitchToServerLogin = switchToServerLogin ?? throw new ArgumentNullException(nameof(switchToServerLogin));
            BtnShowAll = btnShowAll ?? throw new ArgumentNullException(nameof(btnShowAll));
            DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            EcId = ecId;
        }

        public void changeVisibility()
        {
            var visible = this.BtnShowAll.Visibility;
            this.ShowAllResults = visible == Visibility.Collapsed;
            UpdateTree();
            this.BtnShowAll.Visibility = visible;
        }
    }
}
