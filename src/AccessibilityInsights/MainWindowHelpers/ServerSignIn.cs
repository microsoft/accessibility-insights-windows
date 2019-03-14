// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.FileBug;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Net;
using System.Globalization;

namespace AccessibilityInsights
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public partial class MainWindow
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        /// <summary>
        /// Returns whether a website is available within 5 seconds
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static Task<bool> CheckWebsiteAvailability(Uri url)
        {
            return Task.Run(() =>
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    request.Timeout = 5000;
                    request.AllowAutoRedirect = false;
                    request.Method = WebRequestMethods.Http.Head;
                    var response = request.GetResponse();
                }
                catch (UriFormatException)
                {
                    return false;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.Timeout || e.Status == WebExceptionStatus.NameResolutionFailure)
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        ///// <summary>
        ///// Attempt to connect to the service at the given server URL.
        ///// Populates user profile if connection is made correctly. 
        ///// If connection fails, a message dialog will be shown to the user
        ///// if promptIfNeeded is true.
        ///// </summary>
        ///// <param name="serverUrl">url like https://accountname.visualstudio.com to connect to</param>
        ///// <param name="promptIfNeeded">if true, may show account picker & error dialog. Otherwise, no dialogs are shown to user</param>
        ///// <param name="callback">Action to execute after attempt to connect before reallowing further action</param>
        //public async void HandleLoginRequest(Uri serverUrl = null, bool promptIfNeeded = true, Action callback = null)
        //{
        //    SetAllowFurtherAction(false);

        //    // If the main window is always on top, then an error occurs where
        //    //  the login dialog is not a child of the main window, so we temporarily
        //    //  turn topmost off and turn it back on after logging in
        //    bool oldTopmost = this.Topmost;
        //    this.Topmost = false;

        //    if (serverUrl != null && await CheckWebsiteAvailability(serverUrl).ConfigureAwait(true) == false)
        //    {
        //        NotifyLoginUnsuccessful(promptIfNeeded);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            if (serverUrl != null)
        //            {
        //                //await BugReporter.ConnectAsync(serverUrl, promptIfNeeded).ConfigureAwait(true);
        //                //await BugReporter.PopulateUserProfileAsync().ConfigureAwait(true);
        //                UpdateMainWindowLoginFields();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ReportException();
        //            NotifyLoginUnsuccessful(promptIfNeeded);
        //            //BugReporter.FlushToken(serverUrl);
        //            HandleLogoutRequest();
        //        }
        //    }

        //    this.Topmost = oldTopmost;
        //    callback?.Invoke();

        //    Logger.PublishTelemetryEvent(TelemetryAction.Mainwindow_Login_Attempted, TelemetryProperty.PromptIfNeeded, promptIfNeeded.ToString(CultureInfo.InvariantCulture));
        //    SetAllowFurtherAction(true);
        //}

        /// <summary>
        /// Show user dialog with error and log telemetry for failure
        /// </summary>
        /// <param name="bringUpDialog">whether the dialog should be shown</param>
        private static void NotifyLoginUnsuccessful(bool bringUpDialog = true)
        {
            if (bringUpDialog)
            {
                MessageDialog.Show(Properties.Resources.NotifyLoginUnsuccessfulDialogMessage);
            }
        }

        /// <summary>
        /// Update sign in profile, avatar, email, etc - call BugReporter.PopulateUserProfileAsync first
        /// </summary>
        private void UpdateMainWindowLoginFields()
        {
            // Main window UI changes
            vmAvatar.ByteData = BugReporter.Logo;

            imgAvatar.Visibility = Visibility.Visible;
            this.newAccountGrid.Visibility = Visibility.Collapsed;
            string txt = string.Format(CultureInfo.InvariantCulture, Properties.Resources.UpdateMainWindowLoginFieldsSignedInAs, BugReporter.DisplayName);
            AutomationProperties.SetName(btnAccountConfig, txt);
            btnAccountConfig.ToolTip = txt;
        }

        ///// <summary>
        ///// Disconnect from the current server connection
        ///// and update main window UI
        ///// </summary>
        ///// <param name="callback"></param>
        //public void HandleLogoutRequest(Action callback = null)
        //{
        //    //BugReporter.Disconnect();
        //    //// Main window UI changes
        //    //vmAvatar.ByteData = null;
        //    //imgAvatar.Visibility = Visibility.Collapsed;
        //    //this.newAccountGrid.Visibility = Visibility.Visible;
        //    callback?.Invoke();
        //    AutomationProperties.SetName(btnAccountConfig, Properties.Resources.HandleLogoutRequestSignIn);
        //    //btnAccountConfig.ToolTip = Properties.Resources.HandleLogoutRequestSignIn;
        //}
    }
}
