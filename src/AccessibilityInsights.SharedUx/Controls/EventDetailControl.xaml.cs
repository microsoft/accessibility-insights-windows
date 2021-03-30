// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using Axe.Windows.Desktop.UIAutomation.EventHandlers;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for EventDetailControl.xaml
    /// </summary>
    public partial class EventDetailControl : UserControl
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        public EventDetailControl()
        {
            InitializeComponent();
        }

        public void SetEventMessage(EventMessage msg)
        {
            if (msg != null && msg.Properties != null)
            {
                dgEvents.ItemsSource = msg.Properties;
            }
            else
            {
                dgEvents.ItemsSource = null;
            }
        }

        public void Clear()
        {
            dgEvents.ItemsSource = null;
        }

        /// <summary>
        /// Fix datagrid keyboard nav behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgEvents_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg.Items != null && !dg.Items.IsEmpty && !(e.OldFocus is DataGridCell))
            {
                if (dg.SelectedIndex == -1)
                {
                    dg.SelectedIndex = 0;
                    dg.CurrentCell = new DataGridCellInfo(dg.Items[0], dg.Columns[0]);
                }
                else
                {
                    dg.CurrentCell = new DataGridCellInfo(dg.Items[dg.SelectedIndex], dg.Columns[0]);
                }
            }
        }
    }
}
