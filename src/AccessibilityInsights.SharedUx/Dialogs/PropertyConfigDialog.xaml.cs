// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Settings;
using Axe.Windows.Core.Types;
using Axe.Windows.Desktop.UIAutomation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for PropertyConfigDialog.xaml
    /// </summary>
    public partial class PropertyConfigDialog : Window
    {
        /// <summary>
        /// Original selected properties list
        /// </summary>
        readonly List<RecordEntitySetting> CoreProperties;

        /// <summary>
        /// Constructor for dialog
        /// </summary>
        /// <param name="coreProps"></param>
        public PropertyConfigDialog(IEnumerable<int> coreProps, TypeBase source, string title)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            InitializeComponent();

            this.Title = string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.PropertyConfigDialog_TitleFormat, title);
            this.ctrlPropertySelect.LeftColumnTitle = string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.PropertyConfigDialog_AllItemsFormat, title);
            this.ctrlPropertySelect.RightColumnTitle = string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.PropertyConfigDialog_SelectedItemsFormat, title);

            var list = (from kv in source.GetKeyValuePairList()
                        where !DesktopElement.IsExcludedProperty(kv.Key, kv.Value)
                        select new RecordEntitySetting
                        {
                            Id = kv.Key,
                            Name = kv.Value,
                            IsRecorded = coreProps.Contains(kv.Key),
                            Type = RecordEntityType.Property
                        }).ToList();

            var selList = coreProps.Select(l => list.Where(r => r.Id == l).FirstOrDefault()).ToList();
            CoreProperties = selList.ToList();// generate base list here.
            ctrlPropertySelect.SetLists(list.ToList(), selList);
            ((INotifyCollectionChanged)ctrlPropertySelect.lvRight.Items).CollectionChanged += lvRight_CollectionChanged;
        }

        /// <summary>
        /// Cancel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// OK button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Enable/disable save button if selected properties selections changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvRight_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CoreProperties.SequenceEqual(this.ctrlPropertySelect.SelectedList))
            {
                this.buttonOk.IsEnabled = false;
            }
            else
            {
                this.buttonOk.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handle KeyUp event
        /// Close when ESC is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
