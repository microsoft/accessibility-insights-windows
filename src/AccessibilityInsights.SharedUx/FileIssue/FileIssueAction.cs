// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Telemetry;
using Axe.Windows.Actions;
using System;
using System.Drawing;
using System.IO;

namespace AccessibilityInsights.SharedUx.FileIssue
{
    /// <summary>
    /// Class with static functions used for filing bugs
    /// </summary>
    public static class FileIssueAction
    {
        /// <summary>
        /// File an issue. Telemetry wrapper.
        /// </summary>
        /// <param name="issueInformation">Issue information to pass on to the reporter</param>
        /// <returns></returns>
        public static IIssueResult FileIssueAsync(IssueInformation issueInformation)
        {
            if (!IssueReporter.IsEnabled)
                return null;

            try
            {
                IIssueResult issueResult = IssueReporter.FileIssueAsync(issueInformation);
                var telemetryEvent = TelemetryEventFactory.ForIssueFilingCompleted(issueResult, issueInformation);
                Logger.PublishTelemetryEvent(telemetryEvent);
                return issueResult;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                return null;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Attaches screenshot and results file to existing issue
        /// Default case - resulting test file will open in A11yFileMode.Inspect mode,
        ///     no additional loading parameters needed
        /// </summary>
        /// <param name="issueInformation"> Issue information object that needs to be populated with attachments</param>
        /// <param name="ecId">Element context id</param>
        /// <param name="rect">Bounding rect of element for screenshot</param>
        /// <param name="elId">Element unique id</param>
        /// <returns>Success or failure</returns>
        public static void AttachIssueData(IssueInformation issueInformation, Guid ecId, Rectangle? rect, int? elId)
        {
            if (issueInformation == null)
                throw new ArgumentNullException(nameof(issueInformation));

            // Save snapshot locally in prep for uploading attachment
            var snapshotFileName = GetTempFileName(FileFilters.TestExtension);

            // when the file is open, it will be open in Inspect view, not Test view.
            SaveAction.SaveSnapshotZip(snapshotFileName, ecId, elId, Axe.Windows.Desktop.Settings.A11yFileMode.Inspect);
            issueInformation.Screenshot = GetScreenShotForIssueDescription(ecId, rect);
            issueInformation.TestFileName = snapshotFileName;
        }

        /// <summary>
        /// Returns the path to a newly created temporary directory
        /// </summary>
        /// <returns></returns>
        private static string GetTempDir()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }

        /// <summary>
        /// Highlights the given rectangle on a clone of the given data context's
        /// inner bitmap and returns it
        ///
        /// If the given rectangle is null (might happen if bounding rectangle doesn't exist),
        ///     then the original bitmap is returned
        /// </summary>
        /// <param name="ecId">Element context id</param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static Bitmap GetScreenShotForIssueDescription(Guid ecId, Rectangle? rect)
        {
            var dc = GetDataAction.GetElementDataContext(ecId);
            if (dc.Screenshot != null)
            {
                Bitmap newImg = new Bitmap(dc.Screenshot);

                if (rect.HasValue)
                {
                    Rectangle valueRect = rect.Value;
                    using (var graphics = Graphics.FromImage(newImg))
                    using (Pen pen = new Pen(Color.Red, 5))
                    {
                        // Use element
                        var el = GetDataAction.GetA11yElementInDataContext(ecId, dc.ScreenshotElementId);
                        var outerRect = el.BoundingRectangle;

                        valueRect.X -= outerRect.X;
                        valueRect.Y -= outerRect.Y;
                        graphics.DrawRectangle(pen, valueRect);
                    }
                }
                return newImg;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a temp file with the given extension and returns its path
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string GetTempFileName(string extension)
        {
            return Path.Combine(GetTempDir(), Path.GetRandomFileName() + extension);
        }
    }
}
