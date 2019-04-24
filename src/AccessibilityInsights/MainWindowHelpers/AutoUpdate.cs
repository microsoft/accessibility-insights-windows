// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Dialogs;
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.Resources;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for Auto Update
    /// </summary>
    public partial class MainWindow
    {
        private static void PublishTelemetryEventAsync(IAutoUpdate autoUpdate, Stopwatch updateOptionStopwatch, bool timeOutOccurred)
        {
            Task.Run(() =>
            {
                try
                {
                    var updateOption = autoUpdate.UpdateOptionAsync.Result;
                    if (timeOutOccurred)
                    {
                        updateOptionStopwatch.Stop();
                    }
                    Logger.PublishTelemetryEvent(TelemetryAction.Upgrade_GetUpgradeOption, new Dictionary<TelemetryProperty, string>
                    {
                        { TelemetryProperty.UpdateInitializationTime, GetTimeSpanTelemetryString(autoUpdate.GetInitializationTime())},
                        { TelemetryProperty.UpdateOptionWaitTime, GetTimeSpanTelemetryString(updateOptionStopwatch.Elapsed)},
                        { TelemetryProperty.UpdateOption, updateOption.ToString()},
                        { TelemetryProperty.UpdateTimedOut, timeOutOccurred.ToString(CultureInfo.InvariantCulture)}
                    });
                }
                catch (Exception e)
                {
                    e.ReportException();
                    System.Diagnostics.Trace.WriteLine($"Unable to send telemetry at {e.Message}");
                }
            });
        }

        /// <summary>
        /// Retrieve the upgrade decision status, if yes, use autoUpdateOption to update, otherwise, no need to upgrade the app
        /// </summary>
        private static bool ShouldUserUpgrade(IAutoUpdate autoUpdate, out AutoUpdateOption autoUpdateOption)
        {
            if (autoUpdate == null)
            {
                autoUpdateOption = AutoUpdateOption.Unknown;
                return false;
            }

            AutoUpdateOption updateOption;
            Stopwatch updateOptionStopwatch = Stopwatch.StartNew();
            bool timeOutOccurred = false;
            try
            {
                var t = autoUpdate.UpdateOptionAsync;
                if (!t.Wait(2000))
                {
                    timeOutOccurred = true;
                    autoUpdateOption = AutoUpdateOption.Unknown;
                    return false;
                }
                updateOptionStopwatch.Stop();
                updateOption = t.Result;
            }
            catch (AggregateException e)
            {
                e.ReportException();
                updateOption = AutoUpdateOption.Unknown;
            }
            finally
            {
                PublishTelemetryEventAsync(autoUpdate, updateOptionStopwatch, timeOutOccurred);
            }

            autoUpdateOption = updateOption;
            if (autoUpdateOption == AutoUpdateOption.Current || autoUpdateOption == AutoUpdateOption.Unknown)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create the upgrade dialog based on releaseNotesUri and updateOption
        /// </summary>
        private UpdateDialog CreateUpgradeDialog(Uri releaseNotesUri, AutoUpdateOption updateOption)
        {
            UpdateDialog dialog = new UpdateDialog(releaseNotesUri);
            dialog.btnUpdateLater.IsEnabled = (updateOption != AutoUpdateOption.RequiredUpgrade);
            dialog.txtUpdateNotice.Text = (updateOption == AutoUpdateOption.RequiredUpgrade) ? Properties.Resources.MainWindow_ShowUpgradeDialog_An_update_is_required : Properties.Resources.MainWindow_ShowUpgradeDialog_An_update_is_available;

            // center horizontally - does not work when maximized on secondary screen
            Point topLeft = this.WindowState == WindowState.Maximized ?
                            this.GetTopLeftPoint() : new Point(this.Left, this.Top);

            dialog.Top = topLeft.Y + this.borderTitleBar.ActualHeight + 8;
            dialog.Left = topLeft.X + this.ictMainMenu.ActualWidth;
            dialog.Width = this.ActualWidth - this.ictMainMenu.ActualWidth;
            dialog.Owner = this;

            // Block until message bar disappears
            this.modeGrid.Opacity = 0.5;
            this.AllowFurtherAction = false;

            return dialog;
        }

        /// <summary>
        /// Download the upgrade installer and close the app depending on if it is successful or not
        /// </summary>
        private async void DownLoadInstaller(IAutoUpdate autoUpdate, AutoUpdateOption updateOption)
        {
            UpdateResult result = UpdateResult.Unknown;

            // The UAC prompt from the version switcher will appear behind the main window
            // if it is topmost, so we store, change, and restore the value in this method.
            bool oldTopMost = Topmost;

            try
            {
                ctrlProgressRing.Activate();
                Topmost = false;
                result = await autoUpdate.UpdateAsync().ConfigureAwait(true);

                Logger.PublishTelemetryEvent(TelemetryAction.Upgrade_DoInstallation, new Dictionary<TelemetryProperty, string>
                    {
                        { TelemetryProperty.UpdateInstallerUpdateTime, GetTimeSpanTelemetryString(autoUpdate.GetUpdateTime())},
                        { TelemetryProperty.UpdateResult, updateOption.ToString()},
                    });
                if (result == UpdateResult.Success)
                {
                    this.Close();
                    return;
                }
            }
            catch (Exception e)
            {
                e.ReportException();
            };

            Topmost = oldTopMost;
            ctrlProgressRing.Deactivate();
            Logger.PublishTelemetryEvent(TelemetryAction.Upgrade_InstallationError, TelemetryProperty.Error, result.ToString());

            string message = updateOption == AutoUpdateOption.RequiredUpgrade
                ? GetMessageForRequiredUpdateFailure() : GetMessageForOptionalUpdateFailure();

            MessageDialog.Show(message);

            if (updateOption == AutoUpdateOption.RequiredUpgrade)
            {
                this.Close();
                return;
            }
        }

        private void ShowUpgradeDialog()
        {
            // Make the upgrade decision
            var autoUpdate = Container.GetDefaultInstance().AutoUpdate;
            AutoUpdateOption updateOption;
            if (!ShouldUserUpgrade(autoUpdate, out updateOption))
            {
                return;
            }

            // Create the upgrade dialog
            UpdateDialog dialog = CreateUpgradeDialog(autoUpdate.ReleaseNotesUri, updateOption);

            bool? dialogResult = dialog.ShowDialog();
            // user clicked update now (true)
            if (dialogResult.HasValue && dialogResult.Value)
            {
                try
                {
                    DownLoadInstaller(autoUpdate, updateOption);
                }
                catch (Exception e)
                {
                    e.ReportException();
                }
            }
            else
            {
                // user clicked later (false)
                Logger.PublishTelemetryEvent(TelemetryAction.Upgrade_Update_Dismiss, TelemetryProperty.MSIVersion, autoUpdate.InstalledVersion.ToString());
            }

            this.modeGrid.Opacity = 1;
            this.AllowFurtherAction = true;
        }

        private static string GetTimeSpanTelemetryString(TimeSpan? timeSpan)
        {
            // Because return values are used for telemetry, they do not need to be localized
            return timeSpan.HasValue ? timeSpan.Value.ToString() : "unknown";
        }

        private static string GetMessageForOptionalUpdateFailure()
        {
            return Messages.OptionalUpdateFailedUnknown;
        }

        private static string GetMessageForRequiredUpdateFailure()
        {
            return Messages.RequiredUpdateFailedUnknown;
        }

        /// <summary>
        /// Checks for new updates and shows update control if appropriate
        ///   finds path to signed folder via *.drop file, see Axe.Windows.Core.Installer.UpgradeHelper for more info
        /// </summary>
        private void CheckForUpdates()
        {
            if (!IgnoreUpdates)
            {
                try
                {
                    this.ShowUpgradeDialog();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("AccessibilityInsights upgrade exception: " + ex.ToString());
                }
            }
        }
    }
}
