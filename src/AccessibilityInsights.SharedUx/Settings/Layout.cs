// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;

namespace AccessibilityInsights.SharedUx.Settings
{
    /// <summary>
    /// Layout contains data related to window layouts, such as size and location.
    /// It includes methods to generate default Layouts and to interact with
    /// stored layout files.
    /// </summary>
    public class MainWindowLayout : ConfigurationBase
    {
        /// <summary>
        /// Window height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Window width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Width of left column.
        /// </summary>
        public double ColumnSnapWidth { get; set; }

        /// <summary>
        /// Height of RowProperties.
        /// </summary>
        public double RowPropertiesHeight { get; set; }

        /// <summary>
        /// Height of RowPatterns.
        /// </summary>
        public double RowPatternsHeight { get; set; }

        /// <summary>
        /// Height of RowTest.
        /// </summary>
        public double RowTestHeight { get; set; }

        /// <summary>
        /// Window state.
        /// </summary>
        public WindowState WinState { get; set; }

#pragma warning disable CA1024 // This should not be a property
        /// <summary>
        /// Returns a Layout with default values for a Live Mode window.
        /// </summary>
        /// <param name="top">Top position for window</param>
        /// <param name="left">Left position for window</param>
        /// <returns></returns>
        public static MainWindowLayout GetLayoutLive()
        {
            return new MainWindowLayout()
            {
                Width = 460,
                Height = 500,
                ColumnSnapWidth = 410,
                RowPropertiesHeight = 46 * 7,
                RowPatternsHeight = 46 * 3,
                RowTestHeight = 0,
                WinState = WindowState.Normal
            };
        }
#pragma warning restore CA1024 // This should not be a property

#pragma warning disable CA1024 // This should not be a property
        /// <summary>
        /// Returns a Layout with default values for a Snapshot Mode window.
        /// </summary>
        /// <param name="top">Top position for window</param>
        /// <param name="left">Left position for window</param>
        /// <returns></returns>
        public static MainWindowLayout GetLayoutSnapshot()
        {
            return new MainWindowLayout()
            {
                Width = 870,
                Height = 720,
                ColumnSnapWidth = 410,
                RowPropertiesHeight = 46 * 7,
                RowPatternsHeight = 46 * 3,
                RowTestHeight = 260,
                WinState = WindowState.Normal
            };
        }
#pragma warning restore CA1024 // This should not be a property
    }
}
