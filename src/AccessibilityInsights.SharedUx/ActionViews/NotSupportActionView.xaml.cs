// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AccessibilityInsights.SharedUx.ActionViews
{
    /// <summary>
    /// Interaction logic for NotSupportActionView.xaml
    /// </summary>
    public partial class NotSupportActionView : UserControl
    {
        private readonly BaseActionViewModel ActionViewModel;

        private const int BoldFontWeight = 700;

        public NotSupportActionView(BaseActionViewModel a)
        {
            InitializeComponent();
            this.ActionViewModel = a ?? throw new ArgumentNullException(nameof(a));
            SetRequestActionMessage();
        }

        private void SetRequestActionMessage()
        {
            var formatString = requestActionMessage.Text;
            string actionID = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ActionViewModel.PatternName, ActionViewModel.Name);
            string url = "https://github.com/microsoft/accessibility-insights-windows/issues";

            // Split format string of form "a {0} b {1} c" into ["a ", "{0}", " b ", "{1}", " c"]
            Regex formatStringRegex = new Regex(@"(\{\d})");
            string[] formatStringParts = formatStringRegex.Split(formatString, 3);

            var messageRuns = formatStringParts.Select<string, FrameworkContentElement>(part =>
            {
                switch (part)
                {
                    case "{0}": return new Run { Text = actionID, FontWeight = FontWeight.FromOpenTypeWeight(BoldFontWeight) };
                    case "{1}": 
                        var hyperLink = new Hyperlink(new Run { Text = url, }) { NavigateUri = new Uri(url) };
                        hyperLink.RequestNavigate += Hyperlink_RequestNavigate;
                        return hyperLink;
                    default: return new Run { Text = part };
                }
            });
            requestActionMessage.Inlines.Clear();
            requestActionMessage.Inlines.AddRange(messageRuns);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(Properties.Resources.hlLink_RequestNavigateException);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
