using System;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class ScannerResultCustomListViewModel
    {
        //internal ElementContext ElementContext { get; }
        internal Action UpdateTree { get; }
        internal bool ShowAllResults;
        internal Button BtnShowAll;
        internal StackPanel DataContext;
        internal Guid EcId;

        public ScannerResultCustomListViewModel( Action updateTree, Button btnShowAll, StackPanel dataContext, Guid ecId)
        {
            //ElementContext = elementContext ?? throw new ArgumentNullException(nameof(elementContext));
            UpdateTree = updateTree ?? throw new ArgumentNullException(nameof(updateTree));
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
