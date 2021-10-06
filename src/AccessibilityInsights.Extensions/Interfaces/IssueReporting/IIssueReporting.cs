// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.Interfaces.IssueReporting
{
    public interface IIssueReporting
    {
        /// <summary>
        /// The display name of the issue reporting service
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Used to uniquely key the extension’s settings
        /// </summary>
        Guid StableIdentifier { get; }

        /// <summary>
        /// True iff the service needs no further configuration/login to file an issue
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// True if the service can handle file attachments. Tells AI-Win whether or not to provide
        /// a screenshot and snapshot file
        /// </summary>
        bool CanAttachFiles { get; }

        /// <summary>
        /// The logo to display for the extension
        /// </summary>
        ReporterFabricIcon Logo { get; }

        /// <summary>
        /// Tooltip/accessible text for logo
        /// </summary>
        string LogoText { get; }

        /// <summary>
        /// Method to restore the extension’s configuration, presumably on app startup
        /// </summary>
        /// <returns>A Task that completes when the config is ready</returns>
        Task RestoreConfigurationAsync(string serializedConfig);

        /// <summary>
        /// Method to try to fetch the current serialized settings
        /// </summary>
        /// <returns>true (and sets settings) if supported</returns>
        bool TryGetCurrentSerializedSettings(out string settings);

        /// <summary>
        /// Passes along the configurationPath
        /// </summary>
        void SetConfigurationPath(string configurationPath);

        /// <summary>
        /// Control to let user configure/login to issue reporting service.
        /// UpdateSaveButton action needs to be called when the extension is ready to save.
        /// </summary>
        IssueConfigurationControl RetrieveConfigurationControl(Action UpdateSaveButton);

        /// <summary>
        /// Files an issue
        /// </summary>
        /// <param name="issueInfo">Information that describes the issue to create</param>
        /// <returns>Optionally, an issue result with details about this filed issue</returns>
        Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo);
    }
}
