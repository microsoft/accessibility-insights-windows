// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Profile.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.FormattableString;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Methods for integrating with AzureDevOps
    /// </summary>
    public partial class AzureDevOpsIntegration
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        /// <summary>
        /// Single VSO server connection object for reuse
        /// </summary>
        private VssConnection _baseServerConnection;
        private static Object myLock = new Object();

        /// <summary>
        /// When selecting all team projects from AzureDevOps server, do so in chunks
        /// because the API is paged
        /// </summary>
        private const int AzureDevOps_PAGE_SIZE = 1000;

        /// <summary>
        /// Returns true if user has authenticated with VS server URL
        /// </summary>
        public bool ConnectedToAzureDevOps => _baseServerConnection?.HasAuthenticated == true;

        public ExtensionConfiguration Configuration { get; private set; } = new ExtensionConfiguration();

        /// <summary>
        /// Base uri of the AzureDevOps connection
        /// </summary>
        public Uri ConnectedUri => _baseServerConnection?.Uri;

        /// <summary>
        /// AzureDevOps profile fields
        /// </summary>
        public Profile UserProfile { get; private set; }
        public string DisplayName => UserProfile?.DisplayName;
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Avatar => UserProfile?.Avatar.Value;
#pragma warning restore CA1819 // Properties should not return arrays
        public string Email => UserProfile?.EmailAddress;

        private static AzureDevOpsIntegration _instance;

        /// <summary>
        /// Get default instance
        /// </summary>
        /// <returns></returns>
        public static AzureDevOpsIntegration GetCurrentInstance()
        {
            if (_instance == null)
            {
                lock (myLock)
                {
                    if (_instance == null)
                    {
                        _instance = new AzureDevOpsIntegration();
                    }
                }
            }

            return _instance;
        }

        #region IssueFiling
        /// <summary>
        /// Connects to the AzureDevOps server at the given url (e.g. https://myaccount.visualstudio.com)
        ///     If prompt is true, then we may prompt if needed - otherwise, we turn prompting off on credentials
        /// </summary>
        /// <param name="url">AzureDevOps URL to connect to</param>
        /// <param name="prompt">whether user should see any login prompts if needed</param>
        /// <returns></returns>
        public Task ConnectToAzureDevOpsAccount(Uri url, CredentialPromptType prompt = CredentialPromptType.PromptIfNeeded)
        {
            lock (myLock)
            {
                // reset connection state
                Disconnect();
                var credentials = new VssClientCredentials(false);
                credentials.Storage = new VssClientCredentialStorage();
                credentials.PromptType = prompt;
                _baseServerConnection = new VssConnection(url, credentials);
                return _baseServerConnection.ConnectAsync();
            }
        }

        /// <summary>
        /// Removes the stored token associated with the given URL if it exists
        /// </summary>
        /// <param name="url">URL such as https://myaccount.visualstudio.com</param>
        public void FlushToken(Uri url)
        {
            var token = _baseServerConnection?.Credentials.Storage.RetrieveToken(url, VssCredentialsType.Federated);
            if (token != null)
            {
                _baseServerConnection.Credentials.Storage.RemoveToken(url, token);
            }
        }

        /// <summary>
        /// Refreshes profile of current user 
        /// </summary>
        public Task PopulateUserProfile()
        {
            lock (myLock)
            {
                return Task.Run(async () =>
                {
                    if (ConnectedToAzureDevOps)
                    {
                        // We need to use deployment level connection to get profile info
                        var deploymentConn = new VssConnection(new Uri(BASE_VSO_URL), _baseServerConnection.Credentials);
                        ProfileHttpClient client = deploymentConn.GetClient<ProfileHttpClient>();
                        ProfileQueryContext context = new ProfileQueryContext(AttributesScope.Core, CoreProfileAttributes.All, null);
                        this.UserProfile = await client.GetProfileAsync(context).ConfigureAwait(false);
                    }
                });
            }
        }

        /// <summary>
        /// Disconnects from AzureDevOps, resets AzureDevOps instance
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            lock (myLock)
            {
                try
                {
                    _baseServerConnection?.Disconnect();
                }
                catch (ArgumentNullException) { }
                this.UserProfile = null;
            }
        }

        /// <summary>
        /// Returns enumerable of accessible team projects associated
        /// with the currently connected account
        /// </summary>
        /// <returns>Team projects associated with current account or null if disconnected</returns>
        public IEnumerable<Models.TeamProject> GetTeamProjects()
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }

            ProjectHttpClient proClient = _baseServerConnection.GetClient<ProjectHttpClient>();
            var projects = new List<TeamProjectReference>();
            var newElementsReturned = 0;
            // Continue to populate projects list until the number of new returned elements
            //  is less than the number of elements we requested
            do
            {
                var newProjects = proClient.GetProjects(null, AzureDevOps_PAGE_SIZE, projects.Count()).Result;
                newElementsReturned = newProjects.Count();
                projects.AddRange(newProjects);
            }
            while (newElementsReturned >= AzureDevOps_PAGE_SIZE);
            return projects.Select(project => new Models.TeamProject(project.Name, project.Id));
        }

        /// <summary>
        /// Returns enumerable of accessible teams associated
        /// with the given team project
        /// </summary>
        /// <param name="teamProjectId">AzureDevOps team project ID for which to retrieve teams</param>
        /// <returns>teams associated with the given team project, or null if disconnected</returns>
        public IEnumerable<Models.Team> GetTeamsFromProject(Models.TeamProject project)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }

            TeamHttpClient teamClient = _baseServerConnection.GetClient<TeamHttpClient>();
            var teams = new List<WebApiTeamRef>();
            var newElementsReturned = 0;

            // Continue to populate teams list until the number of new returned elements
            //  is less than the number of elements we requested
            do
            {
                var newTeams = teamClient.GetTeamsAsync(project.Id.ToString("D", CultureInfo.InvariantCulture), AzureDevOps_PAGE_SIZE, teams.Count()).Result;
                newElementsReturned = newTeams.Count();
                teams.AddRange(newTeams);
            }
            while (newElementsReturned >= AzureDevOps_PAGE_SIZE);

            return teams.Select(t => new Models.Team(t.Name, t.Id, project));
        }

        #region base AzureDevOps (non-server) connection code

        private const string BASE_VSO_URL = "https://app.vssps.visualstudio.com";
        #endregion

        /// <summary>
        /// Returns the existing issue description for the given issue ID in the currently connected account
        /// </summary>
        /// <param name="issueId">The AzureDevOps issue id in the currently connected account to query</param>
        /// <returns>repro steps for the given issueId or null if the operation fails / if the user is not connected to AzureDevOps</returns>
        public async Task<string> GetExistingIssueDescription(int issueId)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkItemTrackingHttpClient wit = _baseServerConnection.GetClient<WorkItemTrackingHttpClient>();
#pragma warning disable CA2007 // Do not directly await a Task
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            return await wit.GetWorkItemAsync(issueId, new List<string>() { AzureDevOpsField.ReproSteps.ToApiString() })
                .ContinueWith(t => t.IsFaulted ? null : t.Result.Fields[AzureDevOpsField.ReproSteps.ToApiString()].ToString());
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
#pragma warning restore CA2007 // Do not directly await a Task
        }

        /// <summary>
        /// Replaces the existing repro steps on the issue with the given description (should be HTML)
        /// </summary>
        /// <param name="description">new description</param>
        /// <param name="issueId">issue id whose description should be replaced</param>
        /// <returns>Task with completed issue ID or null if user is not connected to AzureDevOps</returns>
        public async Task<int?> ReplaceIssueDescription(string description, int issueId)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkItemTrackingHttpClient wit = _baseServerConnection.GetClient<WorkItemTrackingHttpClient>();
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add(new JsonPatchOperation()
            {
                Operation = Operation.Replace,
                Path = Invariant($"/fields/{AzureDevOpsField.ReproSteps.ToApiString()}"),
                Value = description
            });
#pragma warning disable CA2007 // Do not directly await a Task
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            return await wit.UpdateWorkItemAsync(patchDoc, issueId).ContinueWith(t => t.Result.Id);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
#pragma warning restore CA2007 // Do not directly await a Task
        }

        /// <summary>
        /// Upload attachment of file at the given path to the issue with given issue id
        ///     from "AzureDevOps-dotnet-samples" repo
        /// Also adds comment about snapshot to work item
        /// </summary>
        /// <param name="path">path to file that should be attached</param>
        /// <param name="issueId">issue id to attach file to</param>
        /// <returns>Task with completed issue ID or null if user is not connected to AzureDevOps</returns>
        public async Task<int?> AttachTestResultToIssue(string path, int issueId)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkItemTrackingHttpClient wit = _baseServerConnection.GetClient<WorkItemTrackingHttpClient>();
            AttachmentReference attachment = await wit.CreateAttachmentAsync(new FileStream(path, FileMode.Open), Invariant($"{issueId}.a11ytest")).ConfigureAwait(false);
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add(new JsonPatchOperation()
            {
                Operation = Operation.Test,
                Path = "/rev",
                Value = "1"
            }
            );
            patchDoc.Add(new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/System.History",
                Value = "Attached an Accessibility Insights for Windows test file and screenshot."
            }
            );
            patchDoc.Add(new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/relations/-",
                Value = new
                {
                    rel = "AttachedFile",
                    url = attachment.Url,
                    attributes = new { comment = "Accessibility Insights for Windows test file" }
                }
            }
            );

#pragma warning disable CA2007 // Do not directly await a Task
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            return await wit.UpdateWorkItemAsync(patchDoc, issueId).ContinueWith(t => t.Result.Id);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
#pragma warning restore CA2007 // Do not directly await a Task
        }

        /// <summary>
        /// Uploads the screenshot at the given path to the work item with the given issue id
        /// </summary>
        /// <param name="path">path of screenshot to upload</param>
        /// <returns>Task with URL of the screenshot attachment reference, null if not connected</returns>
        public async Task<string> AttachScreenshotToIssue(string path, int issueId)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkItemTrackingHttpClient wit = _baseServerConnection.GetClient<WorkItemTrackingHttpClient>();
            AttachmentReference attachment = await wit.CreateAttachmentAsync(new FileStream(path, FileMode.Open), Invariant($"{issueId}-pic.png")).ConfigureAwait(false);
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add(new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/relations/-",
                Value = new
                {
                    rel = "AttachedFile",
                    url = attachment.Url,
                    attributes = new { comment = "Screenshot of element" }
                }
            }
            );
            // Return attachment URL once this work item is updated
#pragma warning disable CA2007 // Do not directly await a Task
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            return await wit.UpdateWorkItemAsync(patchDoc, issueId).ContinueWith(t => attachment.Url);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
#pragma warning restore CA2007 // Do not directly await a Task
        }

#pragma warning disable CA1055 // Uri return values should not be strings
        /// <summary>
        /// Returns a link to the already filed issue
        /// </summary>
        /// <param name="issueId">id of issue to get url of</param>
        /// <returns>URL to filed issue, or null if not connected to AzureDevOps</returns>
        public Uri GetExistingIssueUrl(int issueId)
#pragma warning restore CA1055 // Uri return values should not be strings
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkItemTrackingHttpClient wit = _baseServerConnection.GetClient<WorkItemTrackingHttpClient>();
            Task<WorkItem> item = wit.GetWorkItemAsync(issueId, expand: WorkItemExpand.Links);
            var val = item.Result.Links.Links.GetValueOrDefault("html");
            return new Uri((val as ReferenceLink).Href);
        }

        /// <summary>
        /// Returns the default area path of the given team project
        /// </summary>
        /// <param name="conn">connection information to query area-path of</param>
        /// <returns>default area path, or null if operation fails or user is not connected to AzureDevOps</returns>
        public string GetAreaPath(ConnectionInfo conn)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkHttpClient client = _baseServerConnection.GetClient<WorkHttpClient>();
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            return client.GetTeamFieldValuesAsync(new TeamContext(conn.Project.Id, conn.Team?.Id)).ContinueWith(task =>
            {
                // Can fail if there is no area path
                return task.IsFaulted ? null : task.Result?.DefaultValue;
            }).Result;
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
        }

        /// <summary>
        /// Returns the current iteration of the given team context
        /// </summary>
        /// <param name="conn">connection information to query iteration of</param>
        /// <returns>current iteration, or null if operation fails or user is not connected to AzureDevOps</returns>
        public string GetIteration(ConnectionInfo conn)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkHttpClient client = _baseServerConnection.GetClient<WorkHttpClient>();
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            return client.GetTeamIterationsAsync(new TeamContext(conn.Project.Id, conn.Team?.Id), "current").ContinueWith(task =>
            {
                // Can fail if there is no current iteration
                return task.IsFaulted ? null : task.Result.FirstOrDefault().Path;
            }).Result;
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
        }

        /// <summary>
        /// Returns the url of the given team project name
        /// </summary>
        /// <param name="projectName">AzureDevOps project name</param>
        /// <param name="teamName">AzureDevOps team project name</param>
        /// <returns>encoded url to team project (no trailing slash at end), or null if user is not connected to AzureDevOps</returns>
        public Uri GetTeamProjectUrl(string projectName, string teamName)
        {
            if (string.IsNullOrEmpty(projectName) || !ConnectedToAzureDevOps)
            {
                return null;
            }

            UriBuilder builder = new UriBuilder(Uri.UriSchemeHttps,
                this.ConnectedUri.Host + this.ConnectedUri.PathAndQuery.TrimEnd('/'), -1, projectName + 
                (teamName == null ? string.Empty : ("/" + teamName)));
            return builder.Uri;
        }


        public async Task HandleLoginAsync(CredentialPromptType showDialog=CredentialPromptType.DoNotPrompt, Uri serverUri = null)
        {
            if (serverUri == null)
            {
                serverUri = Configuration.SavedConnection.ServerUri;
            }
            try
            {
                if (serverUri != null)
                {
                    await FileIssueHelpers.ConnectAsync(serverUri, showDialog).ConfigureAwait(true);
                    await FileIssueHelpers.PopulateUserProfileAsync().ConfigureAwait(true);
                }
            }
            catch (Exception)
            {
                FileIssueHelpers.FlushToken(serverUri);
            }
        }

        /// <summary>
        /// Returns a template Uri to a issue template with the given project name and reference field mappings
        /// </summary>
        /// <param name="projectName">AzureDevOps project name</param>
        /// <param name="teamName">AzureDevOps team project name</param>
        /// <param name="AzureDevOpsFieldPairs">Key/Value pairs to use when creating the preview</param>
        /// <returns>encoded uri to issue preview (no trailing slash at end), or null if user is not connected to AzureDevOps</returns>
        public Uri CreateIssuePreview(string projectName, string teamName, IReadOnlyDictionary<AzureDevOpsField, string> AzureDevOpsFieldPairs)
        {
            var escaped = from pair in AzureDevOpsFieldPairs
                          select Uri.EscapeDataString($"[{pair.Key.ToApiString()}]") + "=" + Uri.EscapeDataString(pair.Value);

            // Uri.EscapeDataString converts space to %20, but it seems safe for us to instead use a "+" for the contents
            //  of the issue description, which will be interpreted as a space in the browser. This saves us characters, so we
            //  replace all the %20 with "+"

            var finalUrl = GetTeamProjectUrl(projectName, teamName) + "/_workItems/create/Issue?" + String.Join("&", escaped).Replace("%20", "+");
            Uri.TryCreate(finalUrl, UriKind.Absolute, out Uri result);
            return result;
        }
#endregion // IssueFiling
    }
}
