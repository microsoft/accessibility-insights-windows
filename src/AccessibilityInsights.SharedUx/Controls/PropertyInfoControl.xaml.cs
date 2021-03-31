// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for PropertyInfoControl.xaml
    /// </summary>
    public partial class PropertyInfoControl : UserControl
    {
        public static ConfigurationModel Configuration
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppConfig;
            }
        }

        A11yElement Element;
        private CollectionView CollectionView;

        public PropertyInfoControl()
        {
            InitializeComponent();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Set Element to display on the UI
        /// </summary>
        /// <param name="e"></param>
        public void SetElement(A11yElement e)
        {
            if (e != null)
            {
                this.Element = e;
                UpdateUI();
            }
            else
            {
                this.Clear();
            }
        }

        /// <summary>
        /// Update properties grid
        /// </summary>
        private void UpdateUI()
        {
            UpdateProperties();
        }

        /// <summary>
        /// Update Properties
        /// </summary>
        private void UpdateProperties()
        {
            if (this.Element != null && this.Element.Properties != null && this.Element.Properties.Count != 0)
            {
                this.textboxSearch.Text = "";
                var list = from p in this.Element.Properties.Values
                           select p;

                if (Configuration.ShowAllProperties == false)
                {
                    this.lvProperties.ItemsSource = (from id in Configuration.CoreProperties
                                                     join l in list
                                                     on id equals l.Id into newList
                                                     from l in newList.DefaultIfEmpty(new A11yProperty(id, null))
                                                     select new PropertyListViewItemModel(l)).ToList();
                }
                else
                {
                    var coreProps = (from id in Configuration.CoreProperties select new A11yProperty(id, null));

                    list = list.Concat(coreProps)
                            .ToLookup(p => p.Id)
                            .Select(g => g.Aggregate((p1, p2) => p1));

                    this.lvProperties.ItemsSource = (from l in list
                                                     orderby l.Name
                                                     select new PropertyListViewItemModel(l)).ToList();

                    // make sure that we are release from original view
                    this.CollectionView?.DetachFromSourceCollection();

                    this.CollectionView = (CollectionView)CollectionViewSource.GetDefaultView(lvProperties.ItemsSource);
                    this.CollectionView.Filter = NameFilter;
                }
            }
            else
            {
                ClearProperties();
            }
        }

        /// <summary>
        /// Clear Properties ItemsSource
        /// </summary>
        private void ClearProperties()
        {
            if (this.lvProperties.ItemsSource != null)
            {
                var list = (List<PropertyListViewItemModel>)this.lvProperties.ItemsSource;
                this.lvProperties.ItemsSource = null;
                list.ForEach(l => l.Clear());
                list.Clear();
            }
        }

        /// <summary>
        /// Filter logic for listview
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool NameFilter(object item)
        {
            if (String.IsNullOrEmpty(textboxSearch.Text))
                return true;
            else
            {

                string name = (string)((PropertyListViewItemModel)item).Name;
                return (name.IndexOf(textboxSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void textboxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.lvProperties.ItemsSource != null && (this.textboxSearch.Text.Length >= 2 || this.textboxSearch.Text.Length == 0))
            {
                CollectionViewSource.GetDefaultView(this.lvProperties.ItemsSource).Refresh();
            }
        }

        /// <summary>
        /// Handles checking of show all available properties
        /// </summary>
        public void mniShowCorePropertiesChecked()
        {
            Configuration.ShowAllProperties = true;
            UpdateProperties();
            dpFilter.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles unchecking of show all available properties
        /// </summary>
        public void mniShowCorePropertiesUnchecked()
        {
            Configuration.ShowAllProperties = false;
            UpdateProperties();
            dpFilter.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Clear UI
        /// </summary>
        public void Clear()
        {
            ClearProperties();
            this.Element = null;
        }

        /// <summary>
        /// Open properties configuration dialog
        /// </summary>
        public void mniIncludeAllPropertiesClicked()
        {
            var dlg = new PropertyConfigDialog(Configuration.CoreProperties, PropertyType.GetInstance(), "Properties");
            var wnd = (IMainWindow)Application.Current.MainWindow;

            dlg.Owner = Window.GetWindow(this);
            wnd.SetAllowFurtherAction(false);
            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                Configuration.CoreProperties = dlg.ctrlPropertySelect.SelectedList.Select(v => v.Id).ToList();
            }

            UpdateProperties();
            wnd.SetAllowFurtherAction(true);
        }

        /// <summary>
        /// Update show selected properties check state
        /// </summary>
        /// <param name="sender"></param>
        public static void mniShowCorePropertiesLoaded(object sender)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            ((MenuItem)sender).IsChecked = Configuration.ShowAllProperties;
        }

        /// <summary>
        /// Update UI based on current setting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // make sure that filter dock pane is gone.
            this.dpFilter.Visibility = Configuration != null && Configuration.ShowAllProperties ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Copy selected properties to clipboard if ctrl-c pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ApplicationCommands.Copy.Execute(null, lvProperties);
                e.Handled = true;
            }
        }
    }
}
