// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    /// <summary>
    /// Interface to allow mocking of AzureDevOps functionality for unit testing
    /// </summary>
    internal interface IDevOpsIntegration
    {
        /// <summary>
        /// The display name of the currently logged in user
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The avatar of the currently logged in user
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        byte[] Avatar { get; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// The email of the currently logged in user
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Returns true if user has authenticated with VS server URL
        /// </summary>
        bool ConnectedToAzureDevOps { get; }

        /// <summary>
        /// Stored configuration information (shared between config dialog and server connection)
        /// </summary>
        ExtensionConfiguration Configuration { get; }

        /// <summary>
        /// Connects to the AzureDevOps server at the given url (e.g. https://myaccount.visualstudio.com)
        ///     If prompt is true, then we may prompt if needed - otherwise, we turn prompting off on credentials
        /// </summary>
        /// <param name="url">AzureDevOps URL to connect to</param>
        /// <param name="prompt">whether user should see any login prompts if needed</param>
        /// <returns></returns>
        Task ConnectToAzureDevOpsAccount(Uri url, CredentialPromptType prompt = CredentialPromptType.PromptIfNeeded);

        /// <summary>
        /// Removes the stored token associated with the given URL if it exists
        /// </summary>
        /// <param name="url">URL such as https://myaccount.visualstudio.com</param>
        void FlushToken(Uri url);

        /// <summary>
        /// Refreshes profile of current user
        /// </summary>
        Task PopulateUserProfile();

        Task<bool> CheckIfAbleToGetProjects();

        /// <summary>
        /// Disconnects from AzureDevOps, resets AzureDevOps instance
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Returns enumerable of accessible team projects associated
        /// with the currently connected account
        /// </summary>
        /// <returns>Team projects associated with current account or null if disconnected</returns>
        IEnumerable<Models.TeamProject> GetTeamProjects();

        /// <summary>
        /// Returns enumerable of accessible teams associated
        /// with the given team project
        /// </summary>
        /// <param name="teamProjectId">AzureDevOps team project ID for which to retrieve teams</param>
        /// <returns>teams associated with the given team project, or null if disconnected</returns>
        IEnumerable<Models.AdoTeam> GetTeamsFromProject(Models.TeamProject project);

        /// <summary>
        /// Returns the existing issue description for the given issue ID in the currently connected account
        /// </summary>
        /// <param name="issueId">The AzureDevOps issue id in the currently connected account to query</param>
        /// <returns>repro steps for the given issueId or null if the operation fails / if the user is not connected to AzureDevOps</returns>
        Task<string> GetExistingIssueDescription(int issueId);

        /// <summary>
        /// Replaces the existing repro steps on the issue with the given description (should be HTML)
        /// </summary>
        /// <param name="description">new description</param>
        /// <param name="issueId">issue id whose description should be replaced</param>
        /// <returns>Task with completed issue ID or null if user is not connected to AzureDevOps</returns>
        Task<int?> ReplaceIssueDescription(string description, int issueId);

        /// <summary>
        /// Upload attachment of file at the given path to the issue with given issue id
        /// Also adds comment about snapshot to work item
        /// </summary>
        /// <param name="path">path to file that should be attached</param>
        /// <param name="issueId">issue id to attach file to</param>
        /// <returns>Task with completed issue ID or null if user is not connected to AzureDevOps</returns>
        Task<int?> AttachTestResultToIssue(string path, int issueId);

        /// <summary>
        /// Uploads the screenshot at the given path to the work item with the given issue id
        /// </summary>
        /// <param name="path">path of screenshot to upload</param>
        /// <returns>Task with URL of the screenshot attachment reference, null if not connected</returns>
        Task<string> AttachScreenshotToIssue(string path, int issueId);

        /// <summary>
        /// Returns a link to the already filed issue
        /// </summary>
        /// <param name="issueId">id of issue to get url of</param>
        /// <returns>URL to filed issue, or null if not connected to AzureDevOps</returns>
        Uri GetExistingIssueUrl(int issueId);

        /// <summary>
        /// Returns the default area path of the given team project
        /// </summary>
        /// <param name="conn">connection information to query area-path of</param>
        /// <returns>default area path, or null if operation fails or user is not connected to AzureDevOps</returns>
        string GetAreaPath(ConnectionInfo conn);

        /// <summary>
        /// Returns the current iteration of the given team context
        /// </summary>
        /// <param name="conn">connection information to query iteration of</param>
        /// <returns>current iteration, or null if operation fails or user is not connected to AzureDevOps</returns>
        string GetIteration(ConnectionInfo conn);

        /// <summary>
        /// Returns the uri of the given team project name
        /// </summary>
        /// <param name="projectName">AzureDevOps project name</param>
        /// <param name="teamName">AzureDevOps team project name</param>
        /// <returns>encoded url to team project (no trailing slash at end), or null if user is not connected to AzureDevOps</returns>
        Uri GetTeamProjectUri(string projectName, string teamName);

        /// <summary>
        /// Asynchronously handle the login prompt (if needed)
        /// </summary>
        /// <param name="showDialog">Controls whether or not a prompt may be shown</param>
        /// <param name="serverUri">The Uri of the server to connect to</param>
        /// <returns>A Task to let the caller know when the task is complete</returns>
        Task HandleLoginAsync(CredentialPromptType showDialog = CredentialPromptType.DoNotPrompt, Uri serverUri = null);

        /// <summary>
        /// Returns a template Uri to a issue template with the given project name and reference field mappings
        /// </summary>
        /// <param name="projectName">AzureDevOps project name</param>
        /// <param name="teamName">AzureDevOps team project name</param>
        /// <param name="AzureDevOpsFieldPairs">Key/Value pairs to use when creating the preview</param>
        /// <returns>encoded uri to issue preview (no trailing slash at end), or null if user is not connected to AzureDevOps</returns>
        Uri CreateIssuePreview(string projectName, string teamName, IReadOnlyDictionary<AzureDevOpsField, string> AzureDevOpsFieldPairs);
    }
}
