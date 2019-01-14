// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for AddTextRangeToCustomListDialog.xaml
    /// </summary>
    public partial class AddTextRangeToCustomListDialog : Window
    {
        public AddTextRangeToCustomListDialog(string name)
        {
            InitializeComponent();

            this.tbName.Text = name;
        }

        /// <summary>
        /// TextRange Name
        /// </summary>
        public string TextRangeName { get; private set; }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.TextRangeName = this.tbName.Text;
            this.Close();
        }
    }
}
