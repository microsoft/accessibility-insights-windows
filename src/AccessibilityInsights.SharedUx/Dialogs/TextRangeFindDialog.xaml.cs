// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation.Support;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Dialogs
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable. but here we do dispose it at window close.
    /// <summary>
    /// Interaction logic for TextRangeFindDialog.xaml
    /// </summary>
    public partial class TextRangeFindDialog : Window
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private const int SearchForText = 0;

        private TextRangeViewModel ViewModel;
        private TextRangeHilighter Hilighter;
        private TextRangeFinder Finder;

        public TextRangeFindDialog(TextRangeViewModel trvm)
        {
            this.ViewModel = trvm;
            InitializeComponent();
            this.cbAttributes.ItemsSource = GetSourceTemplate();
            this.Hilighter = new TextRangeHilighter(HighlighterColor.GreenTextBrush); // green color
        }

        private static System.Collections.IEnumerable GetSourceTemplate()
        {
            var textItem = new Tuple<int, string, dynamic, Type>(SearchForText, "Text", null, typeof(string));
            var template = TextAttributeTemplate.GetTemplate();
            template?.Insert(0, textItem);
            return template;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // select first item "Text"
            this.cbAttributes.SelectedIndex = 0;
        }

        private void cbAttributes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = cbAttributes.SelectedItem as Tuple<int, string, dynamic, Type>;
            if (item != null)
            {
                this.Hilighter.HilightBoundingRectangles(false);

                if(item.Item3 is List<KeyValuePair<int, string>> || item.Item3 is List<KeyValuePair<bool, string>>)
                {
                    cbValues.ItemsSource = item.Item3;
                    cbValues.SelectedIndex = 0;
                    tbValue.Visibility = Visibility.Hidden;
                    cbValues.Visibility = Visibility.Visible;
                    lbType.Content = "";
                }
                else
                {
                    tbValue.Text = "";
                    lbType.Content = string.Format(CultureInfo.InvariantCulture, Properties.Resources.TextRangeFindDialog_cbAttributes_SelectionChanged, item.Item4.Name);
                    tbValue.Visibility = Visibility.Visible;
                    cbValues.Visibility = Visibility.Hidden;
                }

                chbIgnoreCase.IsEnabled = item.Item1 == SearchForText;

                this.Finder = new TextRangeFinder(this.ViewModel.TextRange);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Hilighter.Dispose();
            this.Hilighter = null;
        }

        /// <summary>
        /// Get value for attribute
        /// </summary>
        /// <returns></returns>
        private object GetAttributeValue()
        {
            var item = cbAttributes.SelectedItem as Tuple<int, string, dynamic, Type>;
            if(item.Item3 == null)
            {
                return ConvertToType(tbValue.Text, item.Item4);
            }
            else if (item.Item4 == typeof(bool))
            {
                return ((KeyValuePair<bool, string>)cbValues.SelectedItem).Key;
            }

            var value = (KeyValuePair<int, string >)cbValues.SelectedItem;
            return value.Key;
        }

        /// <summary>
        /// Convert text to proper type
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ConvertToType(string text, Type type)
        {
            if(type == typeof(int))
            {
                return Convert.ToInt32(text,CultureInfo.InvariantCulture);
            }
            else if(type == typeof(double))
            {
                return Convert.ToDouble(text, CultureInfo.InvariantCulture);
            }
            else if(type== typeof(string))
            {
                return text;
            }

            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.TextRangeFindDialog_ConvertToType__0__type_is_not_supported,type.Name));
        }

        /// <summary>
        /// Get the Attribute Id
        /// </summary>
        /// <returns></returns>
        private int GetAttributeId()
        {
            var item = cbAttributes.SelectedItem as Tuple<int, string, dynamic, Type>;
            return item.Item1;
        }

        /// <summary>
        /// check whether it is text search or not
        /// </summary>
        /// <returns></returns>
        private bool IsTextSearch()
        {
            var item = cbAttributes.SelectedItem as Tuple<int, string, dynamic, Type>;

            return item.Item1 == SearchForText;
        }

        private void btnBackward_Click(object sender, RoutedEventArgs e)
        {
            FindTextRange(true);
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            FindTextRange(false);
        }

        /// <summary>
        /// Find TextRange via TextRangeFinder
        /// </summary>
        /// <param name="backward"></param>
        private void FindTextRange(bool backward)
        {
            //turn off hilighter
            this.Hilighter.HilightBoundingRectangles(false);

            Axe.Windows.Desktop.UIAutomation.Patterns.TextRange range = null;

            try
            {
                var attributeId = GetAttributeId();

                range =  attributeId == SearchForText
                    ? this.Finder.FindText(GetAttributeValue() as string, backward, chbIgnoreCase.IsChecked ?? false)
                    : this.Finder.Find(attributeId, GetAttributeValue(), backward);

                if (range != null)
                {
                    this.Hilighter.SetBoundingRectangles(range.GetBoundingRectangles());
                    this.Hilighter.HilightBoundingRectangles(true);
                }
                else
                {
                    MessageDialog.Show(Properties.Resources.TextRangeFindDialog_FindTextRange_No_matched_range_is_found);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(Properties.Resources.TextRangeFindDialog_FindTextRange_Please_check_value__it_may_not_be_matched_with_expected);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
