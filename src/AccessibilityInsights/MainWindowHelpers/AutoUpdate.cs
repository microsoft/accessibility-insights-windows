// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Dialogs;
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.Misc;
using AccessibilityInsights.Resources;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for Auto Update
    /// </summary>
    public partial class MainWindow
    {
        // Remember our update option that we determined on boot
        private AutoUpdateOption _updateOption = AutoUpdateOption.Unknown;

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
                    Logger.PublishTelemetryEvent(TelemetryEventFactory.ForGetUpgradeOption(autoUpdate.GetInitializationTime(),
                        updateOptionStopwatch.Elapsed, updateOption.ToString(), timeOutOccurred));
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                    System.Diagnostics.Trace.WriteLine($"Unable to send telemetry at {e.Message}");
                }
#pragma warning restore CA1031 // Do not catch general exception types
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

            return autoUpdateOption == AutoUpdateOption.OptionalUpgrade
                || autoUpdateOption == AutoUpdateOption.RequiredUpgrade;
        }

        /// <summary>
        /// Create the upgrade dialog based on releaseNotesUri and updateOption
        /// </summary>
        private UpdateDialog CreateUpgradeDialog(Uri releaseNotesUri, AutoUpdateOption updateOption)
        {
            UpdateDialog dialog = new UpdateDialog(releaseNotesUri);
            if (updateOption == AutoUpdateOption.RequiredUpgrade)
            {
                dialog.btnUpdateLater.Visibility = Visibility.Collapsed;
                dialog.txtUpdateNotice.Text = Properties.Resources.MainWindow_ShowUpgradeDialog_An_update_is_required;
            }

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
        private async void DownLoadInstaller(IAutoUpdate autoUpdate)
        {
            UpdateResult updateResult = UpdateResult.Unknown;

            // If the window is topmost, the UAC prompt from the version switcher will appear behind the main window.
            // To prevent this, save the previous topmost state, ensure that the main window is not topmost when the
            // UAC prompt will display, then restore the previous topmost state.
            bool previousTopmostSetting = Topmost;

            try
            {
                ctrlProgressRing.Activate();
                Topmost = false;
                updateResult = await autoUpdate.UpdateAsync().ConfigureAwait(true);

                Logger.PublishTelemetryEvent(TelemetryEventFactory.ForUpgradeDoInstallation(
                    autoUpdate.GetUpdateTime(), updateResult.ToString()));
                if (updateResult == UpdateResult.Success)
                {
                    this.Close();
                    return;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
            };
#pragma warning restore CA1031 // Do not catch general exception types

            Topmost = previousTopmostSetting;
            ctrlProgressRing.Deactivate();
            Logger.PublishTelemetryEvent(TelemetryEventFactory.ForUpgradeInstallationError(updateResult.ToString()));

            string message = _updateOption == AutoUpdateOption.RequiredUpgrade
                ? GetMessageForRequiredUpdateFailure() : GetMessageForOptionalUpdateFailure();

            MessageDialog.Show(message);

            if (_updateOption == AutoUpdateOption.RequiredUpgrade)
            {
                this.Close();
                return;
            }
        }

        private void ShowUpgradeDialog()
        {
            // Make the upgrade decision
            var autoUpdate = Container.GetDefaultInstance().AutoUpdate;
            if (!ShouldUserUpgrade(autoUpdate, out _updateOption))
            {
                return;
            }

            // Create the upgrade dialog
            UpdateDialog dialog = CreateUpgradeDialog(autoUpdate.ReleaseNotesUri, _updateOption);

            bool? dialogResult = dialog.ShowDialog();
            // user clicked update now (true)
            if (dialogResult.HasValue && dialogResult.Value)
            {
                try
                {
                    DownLoadInstaller(autoUpdate);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            else
            {
                // user clicked later (false)
                Logger.PublishTelemetryEvent(TelemetryEventFactory.ForUpgradeDismissed(autoUpdate.InstalledVersion));
            }

            this.modeGrid.Opacity = 1;
            this.AllowFurtherAction = true;
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
                    UpdateVersionString();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    ex.ReportException();
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }
}
