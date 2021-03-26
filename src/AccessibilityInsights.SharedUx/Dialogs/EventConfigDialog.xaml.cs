// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Settings;
using Axe.Windows.Desktop.Settings;
using System.Linq;
using System.Windows;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for EventConfigWindow.xaml
    /// Need to move this code to MVVM scheme.
    /// for demo, I'm not doing it yet.
    /// </summary>
    public partial class EventConfigWindow : Window
    {

        public static RecorderSetting RecorderSetting
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance().EventConfig;
            }
        }

        public EventConfigWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.controlEvents.SetList(RecorderSetting.Events);
            this.controlProperties.SetList(RecorderSetting.Properties.Where(rs => rs.IsCustom == false).ToList());
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
