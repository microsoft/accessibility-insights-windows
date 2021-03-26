// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// A GridViewColumn styled so that "cells" look like DataGridCells, complete with some custom
    /// resizing behavior. Meant to be used in conjunction with <see cref="GridView"/>.
    /// </summary>
    public partial class CustomGridViewColumn : GridViewColumn
    {
        List<Border> borders = new List<Border>();
        List<TextBlock> textBlocks = new List<TextBlock>();

        string _registeredName;
        internal string RegisteredName
        {
            get => _registeredName;
            set
            {
                _registeredName = value;
                borders.ForEach(bd => UpdateBorderWidthBinding(bd));
            }
        }

        private double MaxColumnWidth => textBlocks.Count == 0 ? 0 : textBlocks.Max(tb => tb.ActualWidth) + 12;

        #region DependencyProperties
        #region HeaderText
        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set
            {
                SetValue(HeaderTextProperty, value);
                tbHeader.Text = value;
                AutomationProperties.SetName(gvcHeader, value);
            }
        }

        internal static readonly DependencyProperty HeaderTextProperty =
           DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(CustomGridViewColumn),
               new PropertyMetadata(new PropertyChangedCallback(HeaderTextPropertyChanged)));

        private static void HeaderTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomGridViewColumn customCol)
            {
                customCol.HeaderText = (string)e.NewValue;
            }
        }
        #endregion HeaderText
        #region ContentPath
        internal string ContentPath
        {
            get => (string)GetValue(ContentPathProperty);

            set
            {
                SetValue(ContentPathProperty, value);
                textBlocks.ForEach(tb => UpdateTextBinding(tb));
            }
        }

        internal static readonly DependencyProperty ContentPathProperty =
           DependencyProperty.Register(nameof(ContentPath), typeof(string), typeof(CustomGridViewColumn),
               new PropertyMetadata(null));
        #endregion ContentPath
        #endregion DependencyProperties

        public CustomGridViewColumn() => InitializeComponent();

        internal void UpdateWidth() => Width = MaxColumnWidth;

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            textBlocks.Add(sender as TextBlock);
            if (!string.IsNullOrEmpty(ContentPath))
            {
                UpdateTextBinding(sender as TextBlock);
            }
        }

        private void TextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            textBlocks.Remove(sender as TextBlock);
        }

        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            UpdateWidth();
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            borders.Add(sender as Border);
            if (!string.IsNullOrEmpty(RegisteredName))
            {
                UpdateBorderWidthBinding(sender as Border);
            }
        }

        private void Border_Unloaded(object sender, RoutedEventArgs e)
        {
            borders.Remove(sender as Border);
        }

        private void UpdateTextBinding(TextBlock tb)
        {
            var bind = new Binding()
            {
                TargetNullValue = Properties.Resources.PropertyDoesNotExist,
                Path = new PropertyPath(ContentPath),
            };
            tb.SetBinding(TextBlock.TextProperty, bind);
        }

        private void UpdateBorderWidthBinding(Border bd)
        {
            var bind = new Binding()
            {
                ElementName = RegisteredName,
                Path = new PropertyPath("ActualWidth"),
            };
            bd.SetBinding(Border.WidthProperty, bind);
        }

        private void PART_HeaderGripper_MouseDoubleClick(object sender, MouseButtonEventArgs e) => UpdateWidth();
    }
}
