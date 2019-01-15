// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.Interfaces.BugReporting
{
    public interface IBugReporting
    {
        /// <summary>
        /// The avatar of the currently logged-in user
        /// </summary>
        IEnumerable<byte> Avatar { get; }

        /// <summary>
        /// The display name of the currently logged-in user
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The email of the currently logged-in user
        /// </summary>
        string Email { get; }

        /// <summary>
        /// True iff a current server connection exists
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="uri">The uri of the server</param>
        /// <param name="prompt">Whether a prompt should be display</param>
        /// <returns>A Task that completes when connection is complete</returns>
        Task ConnectAsync(Uri uri, bool prompt);

        /// <summary>
        /// Flush any cached token for the specified server uri
        /// </summary>
        /// <param name="uri">the uri of the server</param>
        void FlushToken(Uri uri);

        /// <summary>
        /// Populate the profile of the currently logged-in user
        /// </summary>
        /// <returns>A Task that completes when update finishes</returns>
        Task PopulateUserProfileAsync();

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Get the projects that are available to the currently logged-in user
        /// </summary>
        /// <returns>A Task that exposes available projects when complete</returns>
        Task<IEnumerable<IProject>> GetProjectsAsync();

        /// <summary>
        /// Get the description of an existing bug
        /// </summary>
        /// <param name="bugId">The server-side id of the bug to fetch</param>
        /// <returns>A Task that exposes the existing description when complete</returns>
        Task<string> GetExistingBugDescriptionAsync(int bugId);

        /// <summary>
        /// Replace the description of an existing bug
        /// </summary>
        /// <param name="description">The new description</param>
        /// <param name="bugId">The server-side id of the bug to update</param>
        /// <returns>A Task that exposes the status when complete</returns>
        Task<int?> ReplaceBugDescriptionAsync(string description, int bugId);

        /// <summary>
        /// Attach a test results file to the existing bug
        /// </summary>
        /// <param name="path">The path to the results file to attach</param>
        /// <param name="bugId">The server-side id of the bug to update</param>
        /// <returns>A Task that exposes the status when complete</returns>
        Task<int?> AttachTestResultToBugAsync(string path, int bugId);

        /// <summary>
        /// Attach a screenshot image to the existing bug
        /// </summary>
        /// <param name="path">The path to the screenshot image to attach</param>
        /// <param name="bugId">The server-side id of the bug to update</param>
        /// <returns>A Task that exposes the status when complete</returns>
        Task<string> AttachScreenshotToBugAsync(string path, int bugId);

        /// <summary>
        /// Get a Uri to an existing bug
        /// </summary>
        /// <param name="bugId">The server-side id of the bug to query</param>
        /// <returns>A Task that exposes the Uri when complete</returns>
        Task<Uri> GetExistingBugUriAsync(int bugId);

        /// <summary>
        /// Create a preview of the new bug
        /// </summary>
        /// <param name="connectionInfo">The connection to use</param>
        /// <param name="bugInfo">Information that describes the bug to create</param>
        /// <returns>A Task that exposes the Uri to the preview of the new bug</returns>
        Task<Uri> CreateBugPreviewAsync(IConnectionInfo connectionInfo, BugInformation bugInfo);

        /// <summary>
        /// Create an instance of IConnectionInfo with the specified properties
        /// </summary>
        /// <param name="serverUri">The uri to the server</param>
        /// <param name="project">The server project to use--null is supported</param>
        /// <param name="team">The server team to use--null is supported</param>
        /// <returns>An object that implements IConnectionInfo for the specified parameters</returns>
        IConnectionInfo CreateConnectionInfo(Uri serverUri, IProject project, ITeam team);

        /// <summary>
        /// Create an instance of IConnectionCache from a previously saved config string
        /// </summary>
        /// <param name="configString">The previously saved config string. Supports null as a valid value</param>
        /// <returns>An object that implements IConnectionCache</returns>
        IConnectionCache CreateConnectionCache(string configString);

        /// <summary>
        /// Create an instance of IConnectionInfo from a previously saved config string
        /// </summary>
        /// <param name="configString">The previously saved config string. Supports null as a valid value</param>
        /// <returns>An object that implements IConnectionInfo</returns>
        IConnectionInfo CreateConnectionInfo(string configString);
    }
}
