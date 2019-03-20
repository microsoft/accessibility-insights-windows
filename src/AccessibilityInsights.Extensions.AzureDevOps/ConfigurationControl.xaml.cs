// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    /// <summary>
    /// Interaction logic for ConfigurationControl.xaml
    /// </summary>
    public partial class ConfigurationControl : IssueConfigurationControl
    {
        private static AzureDevOpsIntegration AzureDevOps = AzureDevOpsIntegration.GetCurrentInstance();

        public ConfigurationControl()
        {
            InitializeComponent();
        }

        private bool canSave = false;

        public override bool CanSave => canSave;

        public override void OnDismiss() { }

        public override string OnSave()
        {
            return AzureDevOps.Configuration.GetSerializedConfig();
        }

        /// <summary>
        /// Represents different states for whether user has connected to server yet
        /// </summary>
        private enum States
        {
            NoServer,       // First screen with "next"
            EditingServer,  // Second screen with treeview
            HasServer       // Third screen with selected team
        };

        /// <summary>
        /// Represents whether user should be allowed to click "disconnect" or "refresh"
        ///     - used to block while loading treeview
        /// </summary>
        private bool InteractionAllowed { get; set; } = true;


        /// <summary>
        /// Avatar view model
        /// </summary>
        public ByteArrayViewModel vmAvatar { get; private set; } = new ByteArrayViewModel();

        /// <summary>
        /// Current user display name
        /// </summary>
        public static string DisplayName => AzureDevOps.DisplayName;

        /// <summary>
        /// Current user email
        /// </summary>
        public static string Email => AzureDevOps.Email;

        /// <summary>
        /// List of team projects
        /// </summary>
        public BindingList<TeamProjectViewModel> projects { get; private set; } = new BindingList<TeamProjectViewModel>();
        public override Action UpdateSaveButton { get; set; }

        #region configuration updating code
        /// <summary>
        /// Updates the MRU list of servers from the configuration
        /// Depending on whether we are logged in, we update the server combo box selection to
        /// the MRU server or to the currently connected server
        /// </summary>
        /// <param name="configuration"></param>
        public void UpdateFromConfig(ExtensionConfiguration configuration)
        {
            if (InteractionAllowed) // in case team projects were already being loaded (todo: cancel old task)
            {
                this.ServerComboBox.ItemsSource = configuration.CachedConnections?.GetCachedConnections().ToList();
                ConnectionInfo connectionInfo = AzureDevOps.ConnectedToAzureDevOps ?
                    AzureDevOps.Configuration.SavedConnection :
                    configuration.CachedConnections?.GetMostRecentConnection();
                this.ServerComboBox.Text = connectionInfo?.ServerUri == null ? string.Empty : connectionInfo.ServerUri.AbsoluteUri;
                InitializeView();
            }
        }

        /// <summary>
        /// Adds the currently selected connection to the configuration so it is persisted
        /// in the MRU cache as well as the auto-startup connection
        /// </summary>
        /// <param name="configuration"></param>
        public void UpdateConfigFromSelections(ExtensionConfiguration configuration)
        {
            var connection = GetConnectionFromTreeView();
            if (connection != null)
            {
                configuration.CachedConnections?.AddToCache(connection);
                configuration.SavedConnection = connection;
            }
        }

        /// <summary>
        /// For this control we want SaveAndClose to be enabled if any team project
        /// is selected, regardless of whether it differs from the current configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public bool IsConfigurationChanged()
        {
            var connection = GetConnectionFromTreeView();
            return connection?.IsPopulated == true;
        }
        #endregion

        #region state change button handlers
        /// <summary>
        /// Logs the user in and moves to editing server screen.
        /// Forces a configuration change to the saved connection so the server URL is set, 
        /// but the team project and team are null
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (InteractionAllowed)
            {
                if (Uri.IsWellFormedUriString(ServerComboBox.Text, UriKind.Absolute))
                {
                    var serverUri = ToUri(ServerComboBox.Text);

                    // block clicking "next" until login request is done
                    ToggleLoading(true);
                    await AzureDevOps.HandleLoginAsync(CredentialPromptType.PromptIfNeeded, serverUri).ConfigureAwait(false);

                    if (AzureDevOps.ConnectedToAzureDevOps)
                    {
                        AzureDevOps.Configuration.SavedConnection = FileIssueHelpers.CreateConnectionInfo(serverUri, null, null);
                        ChangeStates(States.EditingServer);
                    }
                    else
                    {
                        ToggleLoading(false);
                        ServerComboBox.Focus();
                    }
                }
                else
                {
                    //MessageDialog.Show("URL format is not valid. Example URL: https://accountname.visualstudio.com");
                }
            }
        }

        /// <summary>
        /// Convert a Url string to a Uri, with proper handling of null inputs
        /// </summary>
        /// <param name="stringValue">The url (potentially empty or null) to convert</param>
        /// <returns>The Uri if the url is neither empty nor null</returns>
        private static Uri ToUri(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return null;

            return new Uri(stringValue);
        }

        /// <summary>
        /// Changes to second screen where user can select a team
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            this.tbTeamProjectSearch.Clear();
            ChangeStates(States.EditingServer);
            canSave = true;
            Dispatcher.Invoke(UpdateSaveButton);
        }

        /// <summary>
        /// Logs the user out and moves to selecting a new server screen.
        /// Forces a configuration change to the saved connection so it is null. 
        /// This means we will not try to automatically connect when we next start up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Don't allow disconnect until project teams have loaded
            if (InteractionAllowed)
            {
                AzureDevOps.Disconnect();

                ClearTreeviewSelection();
                ChangeStates(States.NoServer);
                AzureDevOps.Configuration.SavedConnection = null;
            }

            canSave = false;
            Dispatcher.Invoke(UpdateSaveButton);
        }

        /// <summary>
        /// Refresh the project teams, equivalent to reloading this state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Don't allow refresh until project teams have loaded
            if (InteractionAllowed)
            {
                this.tbTeamProjectSearch.Clear();
                ClearTreeviewSelection();
                ChangeStates(States.EditingServer);
            }
        }
        #endregion

        /// <summary>
        /// Switch screens in this connection config view
        /// </summary>
        /// <param name="state"></param>
        private async void ChangeStates(States state)
        {
            Dispatcher.Invoke(() => { 
                vmAvatar.ByteData = AzureDevOps.Avatar;
                displayNameField.Text = AzureDevOps.DisplayName;
                emailField.Text = AzureDevOps.Email;
            });
            if (state == States.NoServer)
                {
                Dispatcher.Invoke(() =>
                {
                    selectServerGrid.Visibility = Visibility.Visible;
                    editProjectGrid.Visibility = Visibility.Collapsed;
                    teamSelectedGrid.Visibility = Visibility.Collapsed;
                });
                }
                else if (state == States.EditingServer)
                {
                Dispatcher.Invoke(() =>
                {
                    editProjectGrid.Visibility = Visibility.Visible;
                    selectTeamGrid.Visibility = Visibility.Visible;
                    teamSelectedGrid.Visibility = Visibility.Collapsed;
                    selectServerGrid.Visibility = Visibility.Collapsed;
                });
                    // Load the team project list, show progress animation
                    try
                    {
                        ToggleLoading(true);
                        projects.Clear();
                        var newProjectList = await UpdateTeamProjects().ConfigureAwait(true); // need to come back to original UI thread. 
                        newProjectList.ForEach(p => projects.Add(p));
                        ToggleLoading(false);
                    Dispatcher.Invoke(() => serverTreeview.ItemsSource = projects);
                    }
                    catch (Exception)
                    {
                        //MessageDialog.Show("Error when retrieving team projects");
                        ToggleLoading(false);
                        disconnectButton_Click(null, null);
                    }

                    FireAsyncContentLoadedEvent(AsyncContentLoadedState.Completed);
                }
                else if (state == States.HasServer)
                {
                Dispatcher.Invoke(() =>
                {
                    editProjectGrid.Visibility = Visibility.Visible;
                    teamSelectedGrid.Visibility = Visibility.Visible;
                    selectTeamGrid.Visibility = Visibility.Collapsed;
                    selectServerGrid.Visibility = Visibility.Collapsed;
                    selectedTeamText.Text = AzureDevOps.Configuration.SavedConnection.ToString();
                });
                }
        }

        /// <summary>
        /// Routes to the correct state given whether the user has logged into the server or not.
        /// If the user is connected to the server but they haven't chosen a team project / team yet, we 
        /// allow them to select their team project / team without having to re-connect.
        public void InitializeView()
        {
            // TODO - move "chose team project yet" state into server integration
            var connection = AzureDevOps.Configuration.SavedConnection;
            if (!AzureDevOps.ConnectedToAzureDevOps)
            {
                ChangeStates(States.NoServer);
            }
            else if (connection?.IsPopulated == true)
            {
                ChangeStates(States.HasServer);
            }
            else
            {
                ChangeStates(States.EditingServer);
            }
        }

        /// <summary>
        /// Notify that the content has loaded
        /// </summary>
        private void FireAsyncContentLoadedEvent(AsyncContentLoadedState state)
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.AsyncContentLoaded))
            {
                UserControlAutomationPeer peer = UIElementAutomationPeer.FromElement(this) as UserControlAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAsyncContentLoadedEvent(new AsyncContentLoadedEventArgs(state, state == AsyncContentLoadedState.Beginning ? 0 : 100));
                }
            }
        }

        /// <summary>
        /// Clears the selection in the treeview, useful when disconnecting
        /// </summary>
        private void ClearTreeviewSelection()
        {
            foreach (var item in serverTreeview.Items)
            {
                // Necessary because the treeview is virtualized for perf reasons
                if (serverTreeview.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem tvItem)
                {
                    tvItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Returns a connection info object from the selected fields in the treeview
        /// - also sets the date of last usage
        /// </summary>
        /// <returns></returns>
        private ConnectionInfo GetConnectionFromTreeView()
        {
            if (this.serverTreeview.SelectedItem == null)
            {
                return null;
            }
            var item = this.serverTreeview.SelectedItem;
            var vm = item as TeamProjectViewModel;
            var team = vm.Team;
            var project = vm.Project;

            if (team != null)
            {
                project = team.ParentProject;
            }
            else if (project != null)
            {
                team = null;
            }

            ConnectionInfo connection = FileIssueHelpers.CreateConnectionInfo(new Uri(this.ServerComboBox.Text), project, team);
            connection.SetLastUsage(DateTime.Now);
            return connection;
        }

        /// <summary>
        /// Toggle whether the progress circle is visible and if user can click on "disconnect", etc
        /// </summary>
        /// <param name="starting">whether to start blocking (true) or stop blocking (false)</param>
        private void ToggleLoading(bool starting)
        {
            InteractionAllowed = !starting;
            //   this.ctrlProgressRing.IsActive = starting;
            Dispatcher.Invoke(() => this.IsEnabled = InteractionAllowed);
        }

        /// <summary>
        /// Repopulates the team projects and children teams
        ///     returns a started Task so it can be awaited on
        /// </summary>
        private static Task<List<TeamProjectViewModel>> UpdateTeamProjects()
        {
            Task<List<TeamProjectViewModel>> t = Task.Run<List<TeamProjectViewModel>>(() =>
            {
                List<TeamProjectViewModel> result = new List<TeamProjectViewModel>();
                try
                {
                    var projects = FileIssueHelpers.GetProjectsAsync().Result;
                    foreach (var project in projects.OrderBy(project => project.Name))
                    {
                        var vm = new TeamProjectViewModel(project, new List<TeamProjectViewModel>());
                        result.Add(vm);
                    }
                    PopulateTreeviewWithTeams(result);
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            });

            return t;
        }

        /// <summary>
        /// Populates each team project in the given list with specific teams, 
        ///     e.g. somewhere underneath VSOnline
        /// The teams are added to a project in sorted order
        /// </summary>
        private static void PopulateTreeviewWithTeams(List<TeamProjectViewModel> projectList)
        {
            object lockObject = new object();
            List<Exception> caughtExceptions = new List<Exception>();

            projectList.AsParallel().ForAll(vm =>
            {
                try
                {
                    var teams = (vm.Project.GetTeamsAsync().Result)?.ToList();
                    if (teams != null && teams.Any())
                    {
                        teams.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
                        var newVMList = new List<TeamProjectViewModel>();
                        teams.ForEach(team => newVMList.Add(new TeamProjectViewModel(team, null)));
                        vm.Children = newVMList;
                    }
                }
                catch (Exception e)
                {
                    lock (lockObject)
                    {
                        caughtExceptions.Add(e);
                    }
                }
            });

            if (caughtExceptions.Any())
            {
                throw new AggregateException("Error populating Projects", caughtExceptions);
            }
        }

        /// <summary>
        /// Update the save button when the selected treeview item changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverTreeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            canSave = true;

            // update status
            Dispatcher.Invoke(UpdateSaveButton);
        }

        /// <summary>
        /// If user presses enter on combobox, have same behavior as clicking "next"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.btnNext.IsEnabled && InteractionAllowed)
            {
                e.Handled = true;
                NextButton_Click(null, null);
            }
        }

        /// <summary>
        /// Change the expanded and visibility properties on the view model and its children 
        /// based on whether they pass the given filter
        /// </summary>
        /// <param name="node"></param>
        /// <param name="filter"></param>
        private void ModifyVisibility(TeamProjectViewModel node, Func<TeamProjectViewModel, bool> filter)
        {
            var matched = filter(node);
            node.Children.ForEach(c => ModifyVisibility(c, filter));
            node.Expanded = node.Children.Any(c => c.Visibility == Visibility.Visible);
            bool shouldBeVisible = matched || node.Expanded;
            node.Visibility = shouldBeVisible ? Visibility.Visible : Visibility.Collapsed;
            // if a TreeViewItem previously selected by the user is now 
            // hidden due to our search filter, we should deselect it.
            if (shouldBeVisible == false)
            {
                node.Selected = false;
            }

        }

        /// <summary>
        /// Filters the search results based on the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox).Text;
            foreach (TeamProjectViewModel vm in serverTreeview.Items)
            {
                ModifyVisibility(vm, (inputVM) =>
                {
                    return
                        (inputVM.Project != null && inputVM.Project.Name.ToUpperInvariant().Contains(searchText.ToUpperInvariant()) ||
                        (inputVM.Team != null && inputVM.Team.Name.ToUpperInvariant().Contains(searchText.ToUpperInvariant())));
                });
            }
        }
    }
}
