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
        public PlaceHolderTextBox()
        {
            this.Text = UIResources.PlaceHolder;
            this.Foreground = UIResources.GrayBrush;

            this.GotFocus += RemoveText;
            this.LostFocus += AddText;
        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (this.Text.Equals(UIResources.PlaceHolder, StringComparison.InvariantCulture))
            {
                this.Text = "";
                this.Foreground = UIResources.BlackBrush;
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = UIResources.PlaceHolder;
                this.Foreground = UIResources.GrayBrush;
            }
            else
            {
                this.Foreground = UIResources.BlackBrush;
            }
         }
    }
}
