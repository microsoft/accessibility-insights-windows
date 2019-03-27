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
        public string PlaceHolder { get; }
        public SolidColorBrush BlackBrush { get; }
        public SolidColorBrush GrayBrush { get; }

        public PlaceHolderTextBox()
        {
            PlaceHolder = Properties.Resources.PlaceHolder;
            BlackBrush = Application.Current.Resources["TextBrush"] as SolidColorBrush;
            GrayBrush = Application.Current.Resources["TextBrushGray"] as SolidColorBrush;
            this.Text = PlaceHolder;
            this.Foreground = GrayBrush;

            this.GotFocus += RemoveText;
            this.LostFocus += AddText;
        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (this.Text.Equals(PlaceHolder, StringComparison.InvariantCulture))
            {
                this.Text = "";
                this.Foreground = BlackBrush;
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = PlaceHolder;
                this.Foreground = GrayBrush;
            }
            else
            {
                this.Foreground = BlackBrush;
            }
         }
    }
}
