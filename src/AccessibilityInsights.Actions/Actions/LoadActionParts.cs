// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Desktop.Settings;
using System.Drawing;

namespace AccessibilityInsights.Actions
{
    /// <summary>
    /// Class to contain the three objects stored in a snapshot zip file
    /// </summary>
    public class LoadActionParts
    {
        /// <summary>
        /// Stored element
        /// </summary>
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Stored screenshot
        /// </summary>
        public Bitmap Bmp { get; private set; }

        /// <summary>
        /// Synthesized screenshot
        /// </summary>
        public Bitmap SynthesizedBmp { get; }

        /// <summary>
        /// Metadata
        /// </summary>
        public SnapshotMetaInfo MetaInfo { get; private set; }

        /// <summary>
        /// Consctructor
        /// </summary>
        /// <param name="el">The element that was selected when the file was saved</param>
        /// <param name="bmp">Actual screenshot</param>
        /// <param name="synthesizedBmp">Synthesized ("yellow box") bitmap</param>
        /// <param name="settings">Metadata about the snapshot</param>
        public LoadActionParts(A11yElement el, Bitmap bmp, Bitmap synthesizedBmp, SnapshotMetaInfo settings)
        {
            this.Element = el;
            this.Bmp = bmp;
            this.SynthesizedBmp = synthesizedBmp;
            this.MetaInfo = settings;
        }
    }
}
