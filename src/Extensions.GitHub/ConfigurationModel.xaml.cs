using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// Interaction logic for ConfigurationModel.xaml
    /// </summary>
    public partial class ConfigurationModel : IssueConfigurationControl
    {
        public ConfigurationModel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when save button clicked.
        /// </summary>
        /// <returns>The extension’s new configuration, serialized</returns>
        public override string OnSave()
        {
            return "";
        }

        /// <summary>
        /// Called when settings page dismissed
        /// </summary>
        public override void OnDismiss()
        {

        }
    }
}
