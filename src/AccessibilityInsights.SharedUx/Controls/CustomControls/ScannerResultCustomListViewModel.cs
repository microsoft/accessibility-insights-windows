using Axe.Windows.Actions.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    class ScannerResultCustomListViewModel
    {
        internal ElementContext ElementContext { get; }
        internal Action UpdateTree;

        public ScannerResultCustomListViewModel(ElementContext elementContext, Action updateTree)
        {
            ElementContext = elementContext ?? throw new ArgumentNullException(nameof(elementContext));
            UpdateTree = updateTree ?? throw new ArgumentNullException(nameof(updateTree));
        }
    }
}
