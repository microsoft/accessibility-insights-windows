// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.Helpers;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.FormattableString;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Methods for integrating with AzureDevOps
    /// </summary>
    internal partial class AzureDevOpsIntegration : IDevOpsIntegration
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        /// <summary>
        /// Single VSO server connection object for reuse
        /// </summary>
        private VssConnection _baseServerConnection;
        private object _lockObject = new object();

        /// <summary>
        /// When selecting all team projects from AzureDevOps server, do so in chunks
        /// because the API is paged
        /// </summary>
        private const int AzureDevOps_PAGE_SIZE = 1000;

        /// <summary>
        /// Returns true if user has authenticated with VS server URL
        /// </summary>
        public bool ConnectedToAzureDevOps => _baseServerConnection?.HasAuthenticated == true;

        public ExtensionConfiguration Configuration { get; } = new ExtensionConfiguration();

        /// <summary>
        /// Base uri of the AzureDevOps connection
        /// </summary>
        internal Uri ConnectedUri => _baseServerConnection?.Uri;

        /// <summary>
        /// AzureDevOps profile fields
        /// </summary>
        internal Profile UserProfile { get; private set; }

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.DisplayName"/>
        /// </summary>
        public string DisplayName => UserProfile?.DisplayName;

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.Avatar"/>
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Avatar => UserProfile?.Avatar.Value;
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.Email"/>
        /// </summary>
        public string Email => UserProfile?.EmailAddress;

        #region IssueFiling
        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.ConnectToAzureDevOpsAccount(Uri, CredentialPromptType)"/>
        /// </summary>
        public Task ConnectToAzureDevOpsAccount(Uri url, CredentialPromptType prompt = CredentialPromptType.PromptIfNeeded)
        {
            lock (_lockObject)
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
        /// Implements <see cref="IDevOpsIntegration.FlushToken(Uri)"/>
        /// </summary>
        public void FlushToken(Uri url)
        {
            var token = _baseServerConnection?.Credentials.Storage.RetrieveToken(url, VssCredentialsType.Federated);
            if (token != null)
            {
                _baseServerConnection.Credentials.Storage.RemoveToken(url, token);
            }
        }

        public async Task<bool> CheckIfAbleToGetProjects()
        {
            try
            {
                ProjectHttpClient proClient = _baseServerConnection.GetClient<ProjectHttpClient>();
                var project = await proClient.GetProjects(null, 1).ConfigureAwait(false);
                return project.Count >= 0;
            }
            catch (VssException)
            {
                return false;
            }
        }

        /// Implements <see cref="IDevOpsIntegration.PopulateUserProfile"/>
        public Task PopulateUserProfile()
        {
            lock (_lockObject)
            {
                return Task.Run(async () =>
                {
                    if (ConnectedToAzureDevOps)
                    {
                        ProfileHttpClient client = _baseServerConnection.GetClient<ProfileHttpClient>();
                        ProfileQueryContext context = new ProfileQueryContext(AttributesScope.Core, CoreProfileAttributes.All, null);
                        this.UserProfile = await client.GetProfileAsync(context).ConfigureAwait(false);
                    }
                });
            }
        }

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.Disconnect()"/>
        /// </summary>
        public void Disconnect()
        {
            lock (_lockObject)
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
        /// Implements <see cref="IDevOpsIntegration.GetTeamProjects()"/>
        /// </summary>
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
                var newProjects = proClient.GetProjects(null, AzureDevOps_PAGE_SIZE, projects.Count).Result;
                newElementsReturned = newProjects.Count;
                projects.AddRange(newProjects);
            }
            while (newElementsReturned >= AzureDevOps_PAGE_SIZE);
            return projects.Select(project => new Models.TeamProject(project.Name, project.Id));
        }

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.GetTeamsFromProject(Models.TeamProject)"/>
        /// </summary>
        public IEnumerable<Models.AdoTeam> GetTeamsFromProject(Models.TeamProject project)
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
                var newTeams = teamClient.GetTeamsAsync(project.Id.ToString("D", CultureInfo.InvariantCulture), null, AzureDevOps_PAGE_SIZE, teams.Count).Result;
                newElementsReturned = newTeams.Count;
                teams.AddRange(newTeams);
            }
            while (newElementsReturned >= AzureDevOps_PAGE_SIZE);

            return teams.Select(t => new Models.AdoTeam(t.Name, t.Id, project));
        }

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.GetExistingIssueDescription(int)"/>
        /// </summary>
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
        /// Implements <see cref="IDevOpsIntegration.ReplaceIssueDescription(string, int)"/>
        /// </summary>
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
        /// Implements <see cref="IDevOpsIntegration.AttachTestResultToIssue(string, int)"/>
        /// </summary>
        public async Task<int?> AttachTestResultToIssue(string path, int issueId)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkItemTrackingHttpClient wit = _baseServerConnection.GetClient<WorkItemTrackingHttpClient>();
            AttachmentReference attachment;
            using (FileStream outputStream = new FileStream(path, FileMode.Open))
            {
                attachment = await wit.CreateAttachmentAsync(outputStream, null, Invariant($"{issueId}.a11ytest"), "Simple", null).ConfigureAwait(false);
            }
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
        /// Implements <see cref="IDevOpsIntegration.AttachScreenshotToIssue(string, int)"/>
        /// </summary>
        public async Task<string> AttachScreenshotToIssue(string path, int issueId)
        {
            if (!ConnectedToAzureDevOps)
            {
                return null;
            }
            WorkItemTrackingHttpClient wit = _baseServerConnection.GetClient<WorkItemTrackingHttpClient>();
            AttachmentReference attachment;
            using (FileStream outputStream = new FileStream(path, FileMode.Open))
            {
                attachment = await wit.CreateAttachmentAsync(outputStream, null, Invariant($"{issueId}-pic.png"), "Simple", null).ConfigureAwait(false);
            }
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

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.GetExistingIssueUrl(int)"/>
        /// </summary>
        public Uri GetExistingIssueUrl(int issueId)
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
        /// Implements <see cref="IDevOpsIntegration.GetAreaPath(ConnectionInfo)"/>
        /// </summary>
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
        /// Implements <see cref="IDevOpsIntegration.GetIteration(ConnectionInfo)"/>
        /// </summary>
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
        /// Implements <see cref="IDevOpsIntegration.GetTeamProjectUri(string, string)"
        /// </summary>
        public Uri GetTeamProjectUri(string projectName, string teamName)
        {
            if (string.IsNullOrEmpty(projectName) || !ConnectedToAzureDevOps)
            {
                return null;
            }

            return GetTeamProjectUriInternal(projectName, teamName, this.ConnectedUri);
        }

        internal static Uri GetTeamProjectUriInternal(string projectName, string teamName, Uri connectedUri)
        {
            UriBuilder builder = new UriBuilder(Uri.UriSchemeHttps,
                connectedUri.Host + connectedUri.PathAndQuery.TrimEnd('/'), -1, projectName +
                (teamName == null ? string.Empty : ("/" + teamName)));
            return builder.Uri;
        }

        /// <summary>
        /// Implements <see cref="IDevOpsIntegration.HandleLoginAsync(CredentialPromptType, Uri)"/>
        /// </summary>
        public async Task HandleLoginAsync(CredentialPromptType showDialog=CredentialPromptType.DoNotPrompt, Uri serverUri = null)
        {
            serverUri = serverUri ?? Configuration.SavedConnection.ServerUri;

            if (serverUri == null)
                return;

            // If the main window is always on top, then an error occurs where
            //  the login dialog is not a child of the main window, so we temporarily
            //  turn topmost off and turn it back on after logging in
            bool oldTopmost = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                oldTopmost = Application.Current.MainWindow.Topmost;
                Application.Current.MainWindow.Topmost = false;
            });

            try
            {
                await ConnectToAzureDevOpsAccount(serverUri, showDialog).ConfigureAwait(true);
                await PopulateUserProfile().ConfigureAwait(true);
                var canGetProjects = await CheckIfAbleToGetProjects().ConfigureAwait(true);
                if (!canGetProjects)
                {
                    throw new Exception("unable to get projects");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                FlushToken(serverUri);
                Disconnect();
            }
#pragma warning restore CA1031 // Do not catch general exception types

            Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Topmost = oldTopmost);
        }

        /// <summary>
        /// Implements <see cref="cref=IDevOpsIntegration.CreateIssuePreview"/>
        /// </summary>
        public Uri CreateIssuePreview(string projectName, string teamName, IReadOnlyDictionary<AzureDevOpsField, string> AzureDevOpsFieldPairs)
        {
            var escaped = from pair in AzureDevOpsFieldPairs
                          select EscapeForUrl($"[{pair.Key.ToApiString()}]") + "=" + EscapeForUrl(pair.Value);

            // Uri.EscapeDataString converts space to %20, but it seems safe for us to instead use a "+" for the contents
            //  of the issue description, which will be interpreted as a space in the browser. This saves us characters, so we
            //  replace all the %20 with "+"

            var finalUrl = GetTeamProjectUri(projectName, teamName) + "/_workItems/create/Bug?" + String.Join("&", escaped).Replace("%20", "+");
            Uri.TryCreate(finalUrl, UriKind.Absolute, out Uri result);
            return result;
        }

        /// <summary>
        /// Substitutions for characters when creating the bug URL
        /// </summary>
        private static IReadOnlyDictionary<char, char> CharacterSubstitutions = new Dictionary<char, char>()
        {
            { '\u00b7', '-' }, // middle dot (183) to dash (45)
        };

        /// <summary>
        /// replaces characters if substitution defined and
        /// returns an escaped representation of the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string EscapeForUrl(string str)
        {
            // characters such as the middle dot can
            // cause issues during navigation, so we only allow
            // a basic set of characters when building the URL
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (CharacterSubstitutions.TryGetValue(c, out char replaced))
                {
                    sb.Append(replaced);
                }
                else if (c <= 127)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('?');
                }
            }

            return Uri.EscapeDataString(sb.ToString());
        }
        #endregion // IssueFiling
    }
}
