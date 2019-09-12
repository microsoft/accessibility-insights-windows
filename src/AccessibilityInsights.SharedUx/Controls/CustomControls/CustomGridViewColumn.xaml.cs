// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    public partial class CustomGridViewColumn : GridViewColumn
    {
        string _setName;
        public string SetName
        {
            get => _setName;
            set
            {
                _setName = value;
                borders.ForEach(bd => UpdateBorderWidthBinding(bd));
            }
        }
        List<Border> borders = new List<Border>();
        List<TextBlock> textBlocks = new List<TextBlock>();
        private double Wid => textBlocks.Count == 0 ? 0 : textBlocks.Max(tb => tb.ActualWidth);

        string _headerText;
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                tbHeader.Text = value;
            }
        }

        public static readonly DependencyProperty HeaderTextProperty =
           DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(CustomGridViewColumn),
               new PropertyMetadata(new PropertyChangedCallback(HeaderTextPropertyChanged)));


        private static void HeaderTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomGridViewColumn customCol)
            {
                customCol.HeaderText = (string)e.NewValue;
            }
        }

        string _contentPath;
        public string ContentPath
        {
            get => _contentPath;
            set
            {
                _contentPath = value;
                textBlocks.ForEach(tb => UpdateTextBinding(tb));
            }
        }

        public static readonly DependencyProperty ContentPathProperty =
           DependencyProperty.Register(nameof(ContentPath), typeof(string), typeof(CustomGridViewColumn),
               new PropertyMetadata(new PropertyChangedCallback(ContentPathPropertyChanged)));


        private static void ContentPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomGridViewColumn customCol)
            {
                customCol.ContentPath = (string)e.NewValue;
            }
        }

        public CustomGridViewColumn()
        {
            InitializeComponent();
        }

        private void TextBlock_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            textBlocks.Add(sender as TextBlock);
            if (!string.IsNullOrEmpty(ContentPath)) UpdateTextBinding(sender as TextBlock);
        }

        private void TextBlock_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            textBlocks.Remove(sender as TextBlock);
        }

        private void Border_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            borders.Add(sender as Border);
            if (!string.IsNullOrEmpty(SetName)) UpdateBorderWidthBinding(sender as Border);
        }

        private void Border_Unloaded(object sender, System.Windows.RoutedEventArgs e)
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
                ElementName = SetName,
                Path = new PropertyPath("ActualWidth"),
            };
            bd.SetBinding(Border.WidthProperty, bind);
        }

        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged) return;
            this.Width = Wid + 12;
        }

        private void PART_HeaderGripper_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Width = Wid + 12;
        }
    }
}
