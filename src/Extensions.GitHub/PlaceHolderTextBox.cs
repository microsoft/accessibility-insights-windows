// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class PlaceHolderTextBox : TextBox
    {
        private readonly string PlaceHolder = "https//github.com/owner/repo";
        private readonly SolidColorBrush BlackBrush = Application.Current.Resources["HLBlack"] as SolidColorBrush;
        private readonly SolidColorBrush GrayBrush = Application.Current.Resources["HLGray"] as SolidColorBrush;

        public PlaceHolderTextBox()
        {
            this.Text = this.PlaceHolder;
            this.Foreground = GrayBrush;

            this.GotFocus += RemoveText;
            this.LostFocus += AddText;
        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (this.Text.Equals(this.PlaceHolder))
            {
                this.Text = "";
                this.Foreground = BlackBrush;
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = this.PlaceHolder;
                this.Foreground = GrayBrush;
            }
        }
    }
}
