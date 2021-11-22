// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.SharedUx.Enums
{
    /// <summary>
    /// file filters
    /// </summary>
    public static class FileFilters
    {
        public static readonly string A11yFileFilter = Properties.Resources.FileFilter_AllFilesFilter;
        public static readonly string TestFileFilter = Properties.Resources.FileFilter_TestFilesFilter;
        public static readonly string EventsFileFilter = Properties.Resources.FileFilter_EventFilesFilter;
        public static readonly string TestExtension = Properties.Resources.FileFilter_TestFilesExtension;
        public static readonly string EventsExtension = Properties.Resources.FileFilter_EventFilesExtension;
    }

    /// <summary>
    /// AccessibilityInsights file types
    /// </summary>
    public enum FileType
    {
        TestResults,
        EventRecord,
    }
}
