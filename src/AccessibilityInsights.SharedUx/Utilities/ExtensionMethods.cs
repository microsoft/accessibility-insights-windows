// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Properties;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using AccessibilityInsights.Win32;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Results;
using Axe.Windows.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.FormattableString;

using MediaColor = System.Windows.Media.Color;
using WindowsPoint = System.Windows.Point;

namespace AccessibilityInsights.SharedUx.Utilities
{
    /// <summary>
    /// Static class for extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get the suggested file name based on Filetype.
        /// format would be
        /// A11yTestFile mm-dd-yy [0-9].a11ytest
        /// try up to 100 and if there still is existing one, exit with 100.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filetype"> type of file</param>
        /// <returns></returns>
        public static string GetSuggestedFileName(this string path, FileType filetype)
        {
            var dt = DateTime.Now.ToString("MM-dd-yy", CultureInfo.InvariantCulture);
            string namebase = Invariant($"{filetype} {dt}");
            string fn = null;

            for (int idx = 0; idx <= 100; idx++)
            {
                fn = GetTestResultFileNameWithId(namebase, idx, filetype);

                if (File.Exists(Path.Combine(path, fn)) == false)
                {
                    break;
                }
            }

            return fn;
        }

        internal static bool IsTextAllowed(this string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        /// <summary>
        /// Get the file name with index id
        /// if Id is 0, it is not appended.
        /// </summary>
        /// <param name="namebase"></param>
        /// <param name="idx">index</param>
        /// <param name="filetype">File type</param>
        /// <returns></returns>
        private static string GetTestResultFileNameWithId(string namebase, int idx, FileType filetype)
        {
            StringBuilder sb = new StringBuilder(namebase);

            if (idx != 0)
            {
                sb.Append(Invariant($" {idx}"));
            }

            switch (filetype)
            {
                case FileType.TestResults:
                    sb.Append(FileFilters.TestExtension);
                    break;
                default:
                    sb.Append(FileFilters.EventsExtension);
                    break;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Copy string to clipboard with error handling.
        /// </summary>
        /// <param name="sb"></param>
        public static void CopyStringToClipboard(this StringBuilder sb)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            try
            {
                Clipboard.SetText(sb.ToString());
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(Resources.ExtensionMethods_CopyStringToClipboard_Error_copying_to_clipboard + ex.Message);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Get Root Node Hierarchy ViewModel based on currently populated data.
        /// </summary>
        /// <param name="dc">Datacontext to create nodes from</param>
        /// <param name="showAncestry">Should ancestry be shown</param>
        /// <param name="showUncertain">Should uncertain scans be shown</param>
        /// <param name="isLiveMode">Is node for live mode</param>
        public static HierarchyNodeViewModel GetRootNodeHierarchyViewModel(this ElementDataContext dc, bool showAncestry, bool showUncertain, bool isLiveMode)
        {
            if (dc?.RootElment?.Properties != null && dc.RootElment.Properties.Count != 0 && dc.Element != null)
            {
                // if need to show ancestry, start from root node
                // if not show ancestry, but element has no parent, return element itself.
                return new HierarchyNodeViewModel(showAncestry ? dc.RootElment : dc.Element.Parent ?? dc.Element, showUncertain, isLiveMode);
            }

            return null;
        }

        /// <summary>
        /// Get ScanResultsViewModel List
        /// Any results with Fail or ScanNotSupported state are picked up for this.
        /// </summary>
        /// <returns></returns>
        public static IList<RuleResultViewModel> GetRuleResultsViewModelList(this ElementDataContext sc)
        {
            if (sc == null)
                throw new ArgumentNullException(nameof(sc));

            List<Tuple<RuleResult, A11yElement>> list = new List<Tuple<RuleResult, A11yElement>>();

            foreach (var e in sc.Elements.Values)
            {
                if (e.ScanResults != null && e.ScanResults.Items != null && e.ScanResults.Items.Count != 0)
                {
                    foreach (var sr in e.ScanResults.Items)
                    {
                        var fsrs = from rr in sr.Items
                                   where rr.Status == ScanStatus.Fail || rr.Status == ScanStatus.ScanNotSupported
                                   select new Tuple<RuleResult, A11yElement>(rr, e);
                        list.AddRange(fsrs);
                    }
                }
            }

            if (list.Count != 0)
            {
                var gs = from l in list
                         orderby l.Item1.Description, l.Item2.Glimpse
                         select new RuleResultViewModel(l.Item2, l.Item1);

                return gs.ToList();
            }

            return null;
        }

        /// <summary>
        /// Returns the top left point of the given window
        ///     .Top and .Left are not updated when a window is maximized,
        ///     so this method finds the actual values of top / left
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static WindowsPoint GetTopLeftPoint(this Window window)
        {
            WindowsPoint topLeft = new WindowsPoint();
            // .Top & .Left are not updated when window is maximized, so need to
            //  recover the position of the window in this case
            var left = typeof(Window).GetField("_actualLeft", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var top = typeof(Window).GetField("_actualTop", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            topLeft.X = (double)left.GetValue(window);
            topLeft.Y = (double)top.GetValue(window);
            return topLeft;
        }

        /// <summary>
        /// Returns a BugInformation with information from the given A11yElement
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IssueInformation GetIssueInformation(this A11yElement element, IssueType issueType)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return new IssueInformation(
                glimpse: element.Glimpse,
                processName: element.GetProcessName(),
                windowTitle: element.GetOriginAncestor(Axe.Windows.Core.Types.ControlType.UIA_WindowControlTypeId).Glimpse,
                elementPath: string.Join("<br/>", element.GetPathFromOriginAncestor().Select(el => el.Glimpse)),
                internalGuid: Guid.NewGuid(),
                issueType: issueType);
        }

        /// <summary>
        /// Gets the header UIElement for a given datagrid and column
        /// </summary>
        /// <param name="root">Datagrid in which to search</param>
        /// <param name="column">Column whose header is to be found</param>
        /// <param name="root">Element in which to search</param>
        /// <returns></returns>
        public static DataGridColumnHeader GetDGColumnHeader(this DataGrid dg, DataGridColumn column, DependencyObject root = null)
        {
            if (root == null)
                root = dg;

            var count = VisualTreeHelper.GetChildrenCount(root);
            for (int x = 0; x < count; x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, x);
                DataGridColumnHeader header = child as DataGridColumnHeader;

                if (header?.Column == column)
                {
                    return header;
                }

                header = dg.GetDGColumnHeader(column, child);

                if (header != null)
                {
                    return header;
                }
            }

            return null;
        }

        /// <summary>
        /// Resizes a column a given amount. Ensures that column size is not set below 0
        /// </summary>
        /// <param name="diff">Amount to resize column</param>
        public static void ResizeColumn(this ColumnDefinition col, double diff)
        {
            if (col == null)
                throw new ArgumentNullException(nameof(col));

            var newWidth = col.Width.Value + diff;

            if (newWidth > 0)
            {
                col.Width = new GridLength(newWidth);
            }
        }

        internal static MediaColor ToMediaColor(this System.Drawing.Color color)
        {
            return MediaColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Gets source for bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        internal static BitmapSource ConvertToSource(this Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            var hbmp = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                NativeMethods.DeleteObject(hbmp);
            }
        }

        /// <summary>
        /// Check whether p is fully visible based on current screen size
        /// </summary>
        internal static bool IsVisibleLocation(this Rectangle p)
        {
            return (from s in System.Windows.Forms.Screen.AllScreens
                    where s.Bounds.Contains(p)
                    select s).Any();
        }

        /// <summary>
        /// Finds object up parent hierarchy of specified type
        /// </summary>
        internal static DependencyObject GetParentElem<T>(this DependencyObject obj)
        {
            try
            {
                var par = VisualTreeHelper.GetParent(obj);

                if (par is T)
                {
                    return par;
                }
                else
                {
                    return GetParentElem<T>(par);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                return null;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        internal static ToggleState ConvertToToggleState(bool? value)
        {
            switch (value)
            {
                case (true): return ToggleState.On;
                case (false): return ToggleState.Off;
                default: return ToggleState.Indeterminate;
            }
        }
    }
}
