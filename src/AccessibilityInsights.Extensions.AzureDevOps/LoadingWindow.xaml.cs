using Microsoft.VisualStudio.Services.Client.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private Func<Task> Operation { get; set; }

        public LoadingWindow(Window owner, Func<Task> operation)
        {
            InitializeComponent();
            this.Owner = owner;
            this.Operation = operation;
        }

        /* ADO window from HandleLoginAsync throws if this window is closed while the other is up
             Microsoft.VisualStudio.Services.Client.Controls.BrowserFlowException
  HResult=0x80131600
  Message=SP324098: Your browser could not complete the operation.
  Source=Microsoft.VisualStudio.Services.Client.Interactive
  StackTrace:
   at Microsoft.VisualStudio.Services.Client.Controls.VssFederatedCredentialPrompt.InvokeDialog(IntPtr owner, Object state)
   at Microsoft.VisualStudio.Services.Client.Controls.DialogHost.<>c__DisplayClass0_1.<InvokeDialogAsync>b__0()
   at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.ThreadHelper.ThreadStart()

Inner Exception 1:
InvalidCastException: Unable to cast COM object of type 'System.__ComObject' to interface type 'IInternetSession'. This operation failed because the QueryInterface call on the COM component for the interface with IID '{79EAC9E7-BAF9-11CE-8C82-00AA004BA90B}' failed due to the following error: No such interface supported (Exception from HRESULT: 0x80004002 (E_NOINTERFACE)).

             
             */

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Operation().ConfigureAwait(true);
            this.Close();
        }
    }
}
