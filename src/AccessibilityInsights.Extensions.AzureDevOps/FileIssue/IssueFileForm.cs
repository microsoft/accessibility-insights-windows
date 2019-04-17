// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using static System.FormattableString;

namespace AccessibilityInsights.Extensions.AzureDevOps.FileIssue
{
    /// <summary>
    /// Winform based issue filing dialog code. 
    /// because of parenting issue, wpf can't be used for IE Control
    /// </summary>
    public partial class IssueFileForm: Form
    {
        private const int ZOOM_MAX = 1000;
        private const int ZOOM_MIN = 25;
        private const int ZOOM_STEP_SIZE = 25;

        // new work form item template URL
        private Uri Url;

        private bool makeTopMost;

        private Action<int> UpdateZoomLevel;

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
        public IssueFileForm(Uri url, bool topmost, int zoomLevel, Action<int> updateZoom)
        {
            InitializeComponent();
            this.UpdateZoomLevel = updateZoom;
            this.makeTopMost = topmost;
            this.Url = url;
            this.zoomIn.Click += ZoomIn_Click;
            this.zoomOut.Click += ZoomOut_Click;
            this.zoomIn.FlatAppearance.BorderSize = 0;
            this.zoomOut.FlatAppearance.BorderSize = 0;
            this.ZoomValue = zoomLevel;
            this.FormClosed += IssueFileForm_FormClosed;
            this.fileIssueBrowser.ScriptErrorsSuppressed = true; // Hides script errors AND other dialog boxes.
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            ZoomToValue();

            var url = e.Url.PathAndQuery;
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
                this.Close();
            }
        }

        /// <summary>
        /// Navigates to the given url
        /// </summary>
        /// <param name="url"></param>
        private void Navigate(Uri url) => fileIssueBrowser.Navigate(url);

        private void IssueFileForm_Load(object sender, EventArgs e)
        {
            this.fileIssueBrowser.ObjectForScripting = new ScriptInterface(this);
            this.fileIssueBrowser.DocumentCompleted += (s, ea) => this.fileIssueBrowser.Document.InvokeScript("eval", new object[] { ScriptToRun });
            fileIssueBrowser.Navigated += Browser_Navigated;
            Navigate(this.Url);
            this.TopMost = makeTopMost;
        }

        /// <summary>
        /// Zooms the browser's active x instance to the ZoomValue property
        /// can throw com exception if form isn't fully loaded yet, silently ignore
        /// </summary>
        private void ZoomToValue()
        {
            try
            {
                var browser = this.fileIssueBrowser.ActiveXInstance as SHDocVw.InternetExplorer;
                browser.ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ZoomValue, IntPtr.Zero);
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
