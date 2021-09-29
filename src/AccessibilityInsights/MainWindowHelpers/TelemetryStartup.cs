﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using System.Windows;

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
                if (TelemetryController.DoesGroupPolicyAllowTelemetry)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    ctrlDialogContainer.ShowDialog(new TelemetryApproveContainedDialog());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }
    }
}
