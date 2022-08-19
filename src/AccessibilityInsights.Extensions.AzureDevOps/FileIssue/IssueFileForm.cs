// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using static System.FormattableString;

namespace AccessibilityInsights.Extensions.AzureDevOps.FileIssue
{
    /// <summary>
    /// Winform based issue filing dialog code.
    /// because of parenting issue, wpf can't be used for IE Control
    /// </summary>
    public partial class IssueFileForm : Form
    {
        enum State
        {
            Initializing,
            NeedsAuthentication,
            Authenticating,
            Authenticated,
            TemplateIsOpen,
            Saving,
            Saved,
        }

        private const int ZOOM_MAX = 1000;
        private const int ZOOM_MIN = 25;
        private const int ZOOM_STEP_SIZE = 25;

        // new work form item template URL
        private Uri Url;

        private State _currentState;

        private bool makeTopMost;

        private Action<int> UpdateZoomLevel;

        private readonly string _userDataFolder;

        /// <summary>
        /// Value to zoom the embedded web browser by
        /// </summary>
        public int ZoomValue { get; set; }

        /// <summary>
        /// Represents the id of the issue filed in this window after ShowDialog() returns
        /// (null if no issue was filed)
        /// </summary>
        public int? IssueId { get; internal set; }

        /// <summary>
        /// Javascript code to run once page is loaded
        /// </summary>
        public string ScriptToRun { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IssueFileForm(Uri url, bool topmost, int zoomLevel, Action<int> updateZoom, string configurationPath)
        {
            InitializeComponent();
            _userDataFolder = Path.Combine(configurationPath, "WebView2");
            this.fileIssueBrowser.CreationProperties = new CoreWebView2CreationProperties
            {
                UserDataFolder = _userDataFolder,
            };
            this.UpdateZoomLevel = updateZoom;
            this.makeTopMost = topmost;
            this.Url = url;
            this.zoomIn.Click += ZoomIn_Click;
            this.zoomOut.Click += ZoomOut_Click;
            this.zoomIn.FlatAppearance.BorderSize = 0;
            this.zoomOut.FlatAppearance.BorderSize = 0;
            this.ZoomValue = zoomLevel;
            this.FormClosed += IssueFileForm_FormClosed;
            ZoomToValue();
        }

        /// <summary>
        /// When the form is closed, set the zoom level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssueFileForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateZoomLevel(ZoomValue);
        }

        /// <summary>
        /// Use URL changes to check whether the issue has been filed or not, close the window if issue has been filed or closed
        /// </summary>
        private void NavigationComplete(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            updateState();
        }

        private State updateState(bool revert = false)
        {
            Uri currentUri = fileIssueBrowser.Source;

            if (currentUri != null)
            {
                switch (_currentState)
                {
                    case State.Initializing:
                        if (currentUri.Host == Url.Host && currentUri.AbsolutePath == Url.AbsolutePath)
                            _currentState = State.TemplateIsOpen;
                        else
                            _currentState = State.NeedsAuthentication;
                        break;
                    case State.NeedsAuthentication:
                        _currentState = State.Authenticating;
                        break;
                    case State.Authenticating:
                        if (currentUri.Host == Url.Host && currentUri.AbsolutePath == Url.AbsolutePath)
                            _currentState = State.Authenticated;
                        break;
                    case State.Authenticated:
                        _currentState = State.Initializing;
                        break;
                    case State.TemplateIsOpen:
                        if (currentUri.Host == Url.Host && currentUri.AbsolutePath != Url.AbsolutePath)
                            _currentState = State.Saving;
                        break;
                    case State.Saving:
                        _currentState = revert ? State.TemplateIsOpen : State.Saved;
                        break;
                }
            }

            return _currentState;
        }

        private void CoreWebView2_HistoryChanged(object sender, object e)
        {
            switch (updateState())
            {
                case State.Saving:
                    var url = fileIssueBrowser.Source.PathAndQuery;
                    var savedUrlSubstrings = new List<String>() { "_queries/edit/", "_workitems/edit/", "_workitems?id=" };
                    int urlIndex = savedUrlSubstrings.FindIndex(str => url.Contains(str));
                    if (urlIndex >= 0)
                    {
                        var matched = savedUrlSubstrings[urlIndex];
                        var endIndex = url.IndexOf(matched, StringComparison.Ordinal) + matched.Length;

                        // URL looks like "_queries/edit/2222222/..." where 2222222 is issue id
                        // or is "_workitems/edit/2222222"
                        // or is "_workitems?id=2222222"
                        url = url.Substring(endIndex);
                        int result;
                        bool worked = int.TryParse(new String(url.TakeWhile(Char.IsDigit).ToArray()), out result);
                        if (worked)
                        {
                            this.IssueId = result;
                        }
                        else
                        {
                            this.IssueId = null;
                        }
                        updateState();
                        this.Close();
                    }
                    else
                    {
                        updateState(revert: true);
                    }
                    break;
                case State.NeedsAuthentication:
                    fileIssueBrowser.Source = new Uri(Url.GetLeftPart(UriPartial.Path));
                    break;
                case State.Authenticated:
                    Navigate(Url);
                    break;
            }
        }

        /// <summary>
        /// Navigates to the given URL
        /// </summary>
        /// <param name="url"></param>
        private void Navigate(Uri url) => fileIssueBrowser.Source = url;

        private async void IssueFileForm_Load(object sender, EventArgs e)
        {
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions()
            {
                AllowSingleSignOnUsingOSPrimaryAccount = true,
            };
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, _userDataFolder, options).ConfigureAwait(true);

            await this.fileIssueBrowser.EnsureCoreWebView2Async(environment).ConfigureAwait(true);
            this.fileIssueBrowser.NavigationCompleted += NavigationComplete;
            this.TopMost = makeTopMost;
            Navigate(this.Url);

            this.fileIssueBrowser.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
        }

        /// <summary>
        /// Zooms the browser's active x instance to the ZoomValue property
        /// can throw com exception if form isn't fully loaded yet, silently ignore
        /// </summary>
        private void ZoomToValue()
        {
            try
            {
                this.fileIssueBrowser.ZoomFactor = ZoomValue / 100.0;
                this.zoomLabel.Text = Invariant($"{ZoomValue}%");
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                e.ReportException();
            }
        }

        /// <summary>
        /// Zoom out by 25% (max 1000)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomOut_Click(object sender, System.EventArgs e)
        {
            AddZoomValue(-ZOOM_STEP_SIZE);
        }

        /// <summary>
        /// Zoom in by 25% (min 25)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomIn_Click(object sender, System.EventArgs e)
        {
            AddZoomValue(ZOOM_STEP_SIZE);
        }

        /// <summary>
        /// Make sure value is between 25 and 1000 and zoom to it
        /// </summary>
        /// <param name="delta"></param>
        private void AddZoomValue(int delta)
        {
            ZoomValue = Math.Max(ZoomValue + delta, ZOOM_MIN);
            ZoomValue = Math.Min(ZoomValue, ZOOM_MAX);
            ZoomToValue();
        }
    }
}
