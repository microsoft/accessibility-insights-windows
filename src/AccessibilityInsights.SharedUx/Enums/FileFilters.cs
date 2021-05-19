// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.SharedUx.Enums
{
    /// <summary>
    /// file filters
    /// </summary>
    public static class FileFilters
    {
        public const string A11yFileFilter = "AccessibilityInsights files (*.a11ytest,*.a11yevent)|*.a11ytest;*.a11yevent|A11y Test files (*.a11ytest)|*.a11ytest|A11y Events files (*.a11yevent)|*.a11yevent|All files (*.*)|*.*";
        public const string TestFileFilter = "AccessibilityInsights Test files (*.a11ytest)|*.a11ytest|All files (*.*)|*.*";
        public const string EventsFileFilter = "AccessibilityInsights Events files (*.a11yevent)|*.a11yevent|All files (*.*)|*.*";
        public const string TestExtension = ".a11ytest";
        public const string EventsExtension = ".a11yevent";
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
