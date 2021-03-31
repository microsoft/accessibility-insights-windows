// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Globalization;
using System.Runtime.InteropServices;

namespace AccessibilityInsights.Extensions.AzureDevOps.FileIssue
{
    /// <summary>
    /// Bridge between web browser and WPF window
    /// - methods can be accessed from javascript, e.g. "window.external.Log('test')"
    /// </summary>
    [ComVisible(true)]
    public class ScriptInterface
    {
        private IssueFileForm Owner { get; set; }
        public ScriptInterface(IssueFileForm owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Set issue filing dialog's issue id when issue filed
        /// </summary>
        /// <param name="issueId"></param>
        public void IssueFiled(string issueId)
        {
            int? parsed = int.Parse(issueId, CultureInfo.InvariantCulture);
            Owner.IssueId = parsed >= 0 ? parsed : null;
        }

        /// <summary>
        /// Close the issue filing dialog when the web browser closes
        /// </summary>
        public void CloseWindow() => Owner.Close();

        /// <summary>
        /// Log message to console
        /// </summary>
        /// <param name="s"></param>
#pragma warning disable CA1822 // Exempted since it is required by COM interaction.
        public void Log(string s) => System.Diagnostics.Debug.WriteLine(s);
#pragma warning restore CA1822
    }
}
