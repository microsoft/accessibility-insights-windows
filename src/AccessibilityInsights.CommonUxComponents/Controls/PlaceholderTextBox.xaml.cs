// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.CommonUxComponents.Controls
{
    /// <summary>
    /// A textbox that displays placeholder text when the user has not entered text
    /// </summary>
    public partial class PlaceholderTextBox : TextBox
    {
        #region Placeholder (Dependency Property)

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(null));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }

            set { SetValue(PlaceholderProperty, value); }
        }

        #endregion

        /// <summary>
        /// Constructor with default size
        /// </summary>
        public PlaceholderTextBox()
        {
            InitializeComponent();
        }
    }
}
