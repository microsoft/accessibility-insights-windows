// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.ActionViews
{
    /// <summary>
    /// Interaction logic for NotSupportActionView.xaml
    /// </summary>
    public partial class NotSupportActionView : UserControl
    {
        private BaseActionViewModel ActionViewModel;

        public NotSupportActionView(BaseActionViewModel a)
        {
            InitializeComponent();
            this.ActionViewModel = a ?? throw new ArgumentNullException(nameof(a));
            this.tbName.Text = string.Format(CultureInfo.InvariantCulture,"{0}.{1}", ActionViewModel.PatternName, ActionViewModel.Name);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("name", this.tbName.Text);
        }
    }
}
