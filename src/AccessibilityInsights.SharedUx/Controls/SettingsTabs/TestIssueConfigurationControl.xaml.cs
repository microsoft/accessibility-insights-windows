using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls.SettingsTabs
{
    /// <summary>
    /// Interaction logic for ConfigurationModel.xaml
    /// </summary>
    public partial class TestIssueConfigurationControl : IssueConfigurationControl, INotifyPropertyChanged
    {
        public override Action UpdateSaveButton { get; set; }
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Config { get; set; }
        public TestIssueConfigurationControl(Action UpdateSaveButton)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Ashwin1.IsEnabled = false;
            this.UpdateSaveButton = UpdateSaveButton;
        }

        /// <summary>
        /// Called when save button clicked.
        /// </summary>
        /// <returns>The extension’s new configuration, serialized</returns>
        public override string OnSave()
        {
            return Config;
        }

        /// <summary>
        /// Called when settings page dismissed
        /// </summary>
        public override void OnDismiss()
        {
        }

        private bool canSaveI = false;

        /// <summary>
        /// Can the save button be clicked
        /// </summary>
        public bool canSaveAK
        {
            get { return canSaveI; }
            set
            {
                if (canSaveI != value)
                {
                    canSaveI = value;
                    OnPropertyChanged(nameof(CanSave));  // To notify when the property is changed
                }
            }
        }

        public override bool CanSave => canSaveAK;

        private void Ashwin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            canSaveAK = !canSaveAK;
            UpdateSaveButton();
            //CommandManager.InvalidateRequerySuggested();
        }


    }
}

