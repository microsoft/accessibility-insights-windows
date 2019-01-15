// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Globalization;
using System.Runtime.InteropServices;

namespace AccessibilityInsights.SharedUx.FileBug
{
    /// <summary>
    /// Bridge between web browser and WPF window
    /// - methods can be accessed from javascript, e.g. "window.external.Log('test')"
    /// </summary>
    [ComVisible(true)]
    public class ScriptInterface
    {
        private BugFileForm Owner { get; set; }
        public ScriptInterface(BugFileForm owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Set bug filing dialog's bug id when bug filed
        /// </summary>
        /// <param name="bugId"></param>
        public void BugFiled(string bugId)
        {
            int? parsed = int.Parse(bugId, CultureInfo.InvariantCulture);
            Owner.BugId = parsed >= 0 ? parsed : null;
        }

        /// <summary>
        /// Close the bug filing dialog when the web browser closes
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
