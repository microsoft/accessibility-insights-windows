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
        public static string PlaceHolder { get; } = Properties.Resources.PlaceHolder;
        public static SolidColorBrush BlackBrush { get; } = Application.Current.Resources["TextBrush"] as SolidColorBrush;
        public static SolidColorBrush GrayBrush { get; } = Application.Current.Resources["TextBrushGray"] as SolidColorBrush;

        public PlaceHolderTextBox()
        {
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
