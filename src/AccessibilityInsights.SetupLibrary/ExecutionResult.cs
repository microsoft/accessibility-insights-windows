// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Results from VersionSwitcher. This will get serialized to disk as a string, then
    /// included with telemetry. Please try to minimize churn to make the telemetry more
    /// manageable.
    /// </summary>
    public enum ExecutionResult
    {
        Unknown,
        ErrorBadCommandLine,
        ErrorMsiDownloadFailed,
        ErrorMsiBadSignature,
        ErrorMsiSizeMismatch,
        ErrorMsiSha512Mismatch,
        ErrorInstallingMsi,
        Success,
    }
}
