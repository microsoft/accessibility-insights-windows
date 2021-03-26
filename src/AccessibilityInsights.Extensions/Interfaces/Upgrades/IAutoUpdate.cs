// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.Interfaces.Upgrades
{
    /// <summary>
    /// Abstract out the auto-update functionality
    /// </summary>
    public interface IAutoUpdate
    {
        /// <summary>
        /// Report the current update option. Async as it may go off-box
        /// </summary>
        Task<AutoUpdateOption> UpdateOptionAsync { get; }

        /// <summary>
        /// Uri to Release notes associated with the updated version.
        /// </summary>
        Uri ReleaseNotesUri { get; }

        /// <summary>
        /// The currently installed version.
        /// </summary>
        Version InstalledVersion { get; }

        /// <summary>
        /// The Release channel that was used when fetching upgrade information
        /// </summary>
        string ReleaseChannel { get; }

        /// <summary>
        /// Trigger an update. Async as it may go off-box
        /// </summary>
        /// <returns>A Task that exposed the result of the update</returns>
        Task<UpdateResult> UpdateAsync();

        /// <summary>
        /// Gets the time taken to initialize the updater.
        /// This will only be valid after initialization is complete.
        /// </summary>
        TimeSpan? GetInitializationTime();

        /// <summary>
        /// Gets the time taken to run the Update operation
        /// </summary>
        TimeSpan? GetUpdateTime();
    }
}
