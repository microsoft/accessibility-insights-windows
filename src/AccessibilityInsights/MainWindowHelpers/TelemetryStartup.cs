// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using AccessibilityInsights.SharedUx.Settings;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for Telemetry Startup Dialog
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Checks if telemetry startup dialog needs to be diplayed
        /// </summary>
        private void ShowTelemetryDialog()
        {
            if (ConfigurationManager.GetDefaultInstance().AppConfig.ShowTelemetryDialog)
            {
                ctrlTelemetryDialog.ShowControl();
            }
            else
            {
                ctrlTelemetryDialog.HideControl();
            }
        }
    }
}
