// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// AppLayout represents the window information (size and location) for Live and Snapshot Modes.
    /// Specific data is stored in two Layouts; AppLayout houses these objects and provides JSON loading
    /// and storing functions.
    /// </summary>
    public class AppLayout : ConfigurationBase
    {
        /// <summary>
        /// Current version of configuration
        /// </summary>
        public const string CurrentVersion = "1.0.5";

        /// <summary>
        /// Layout details for Snapshot Mode.
        /// </summary>
        public MainWindowLayout LayoutSnapshot { get; set; }

        /// <summary>
        /// Layout details for Live Mode.
        /// </summary>
        public MainWindowLayout LayoutLive { get; set; }

        /// <summary>
        /// Layout details for Live Mode.
        /// </summary>
        public MainWindowLayout LayoutEvent { get; set; }

        /// <summary>
        /// Left coordinate of window.
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// Top coordinate of window.
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Window height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Window width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Window state.
        /// </summary>
        public WindowState WinState { get; set; }

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        public AppLayout() { }

        /// <summary>
        /// Constructor for AppLayout. Uses window position and generates Layouts with default values.
        /// </summary>
        /// <param name="top">Top coordinate of window.</param>
        /// <param name="left">Left coordinate of window.</param>
        public AppLayout(double top, double left)
            : base(CurrentVersion)
        {
            Top = top;
            Left = left;
            Width = 870;
            Height = 720;
            WinState = WindowState.Normal;
            LayoutLive = MainWindowLayout.GetLayoutLive();
            LayoutSnapshot = MainWindowLayout.GetLayoutSnapshot();
            LayoutEvent = MainWindowLayout.GetLayoutSnapshot();
        }

        /// <summary>
        /// Updates app layout from previous version if necessary
        /// </summary>
        /// <param name="path">Location of settings file</param>
        /// <param name="top">Current top position of window</param>
        /// <param name="left">Current left position of window</param>
        /// <returns></returns>
        public void LoadLayoutIfPrevVersion(double top, double left)
        {
            if (this.Version == null || string.CompareOrdinal(this.Version, AppLayout.CurrentVersion) != 0)
            {
                // for now we enforce the layout to be always be default if the version mismatch.
                // it will reduce the unnecessary crashes or noize. but it may lose the previous layout info.
                // we may bear the lose of previous layout for now.
                this.Top = top;
                this.Left = left;
                this.Width = 870;
                this.Height = 720;
                WinState = WindowState.Normal;
                this.LayoutSnapshot = MainWindowLayout.GetLayoutSnapshot();
                this.LayoutEvent = MainWindowLayout.GetLayoutSnapshot();
                this.LayoutLive = MainWindowLayout.GetLayoutLive();
                this.Version = CurrentVersion;
            }
        }
    }
}

