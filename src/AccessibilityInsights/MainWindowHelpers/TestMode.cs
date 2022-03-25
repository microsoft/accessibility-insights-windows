// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Telemetry;
using Axe.Windows.Actions;
using Axe.Windows.Desktop.Settings;
using System;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for HandleTestStartUpRequest
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Start Test Mode
        /// </summary>
        /// <returns></returns>
        void StartTestMode(TestView view)
        {
            if (view != TestView.NoSelection)
            {
                if (SelectAction.GetDefaultInstance().IsPaused)
                {
                    ClearOverlayDriver.BringMainWindowOfPOIElementToFront();
                }
                HandlePauseButtonToggle(true);
            }
            switch (view)
            {
                case TestView.NoSelection:
                    StartTestNoSelection();
                    break;
                case TestView.AutomatedTestResults:
                    StartTestAutomatedChecksView();
                    break;
                case TestView.TabStop:
                    break;
                case TestView.ElementDetails:
                    StartElementDetailView();
                    break;
                case TestView.ElementHowToFix:
                    StartElementHowToFixView();
                    break;
            }
        }

        /// <summary>
        /// Start snapshot mode.
        /// </summary>
        private void StartElementDetailView()
        {
            var ecId = SelectAction.GetDefaultInstance().SelectedElementContextId;
            if (ecId != null)
            {
                this.CurrentPage = AppPage.Test;
                this.CurrentView = TestView.CapturingData;
                DisableElementSelector();

                var tp = GetDataAction.GetProcessAndUIFrameworkOfElementContext(ecId.Value);

                PageTracker.TrackPage(this.CurrentPage, this.CurrentView.ToString(), tp.Item2);

                this.ctrlCurMode.HideControl();
                this.ctrlCurMode = this.ctrlSnapMode;
                this.ctrlSnapMode.DataContextMode = GetDataContextModeForTest();
                this.ctrlCurMode.ShowControl();

#pragma warning disable CS4014
                // NOTE: We aren't awaiting this async call, so if you
                // touch it, consider if you need to add the await
                ctrlSnapMode.SetElement(ecId.Value);
#pragma warning restore CS4014
            }
            else
            {
                this.AllowFurtherAction = false;
                MessageDialog.Show(Properties.Resources.StartElementDetailViewNoElementIsSelectedMessage);
                this.AllowFurtherAction = true;
            }
        }

        /// <summary>
        /// Switches to snapshot mode and selects appropriate element
        /// </summary>
        /// <param name="followUp">An action to perform after tests have been run</param>
        private void StartElementHowToFixView(Action followUp = null)
        {
            var ecId = SelectAction.GetDefaultInstance().SelectedElementContextId;

            this.CurrentPage = AppPage.Test;
            this.CurrentView = TestView.ElementHowToFix;

            var tp = GetDataAction.GetProcessAndUIFrameworkOfElementContext(ecId.Value);
            PageTracker.TrackPage(this.CurrentPage, this.CurrentView.ToString(), tp.Item2);

            this.ctrlCurMode.HideControl();
            this.ctrlCurMode = this.ctrlSnapMode;
            this.ctrlSnapMode.DataContextMode = GetDataContextModeForTest();
            this.ctrlCurMode.ShowControl();

            DisableElementSelector();
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            ctrlSnapMode.SetElement(ecId.Value).ContinueWith(
                t =>
                {
                    followUp?.Invoke();
                });
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler

        }

        /// <summary>
        /// Start test mode when no selection has been made and tests have not been run
        /// </summary>
        private void StartTestNoSelection()
        {
            ctrlCurMode.HideControl();
            ctrlCurMode = ctrlTestMode;
            ctrlCurMode.ShowControl();

            this.CurrentPage = AppPage.Test;
            this.CurrentView = TestView.NoSelection;
            this.ctrlTestMode.SetForNoSelection();
        }

        /// <summary>
        /// Start fastpass mode.
        /// </summary>
        private void StartTestAutomatedChecksView()
        {
            var ec = SelectAction.GetDefaultInstance().SelectedElementContextId;

            if (ec != null)
            {
                DisableElementSelector();

                ctrlCurMode.HideControl();
                // make sure that we honor loaded data. but in this case, some of manual test should be disabled.
                ctrlTestMode.DataContextMode = GetDataContextModeForTest();
                ctrlCurMode = ctrlTestMode;
                ctrlCurMode.ShowControl();

                // set mode to data capturing
                this.CurrentPage = AppPage.Test;
                this.CurrentView = TestView.CapturingData;

#pragma warning disable CS4014
                // NOTE: We aren't awaiting this async call, so if you
                // touch it, consider if you need to add the await
                this.ctrlTestMode.SetElement(ec.Value);
#pragma warning restore CS4014

                var tp = GetDataAction.GetProcessAndUIFrameworkOfElementContext(ec.Value);
                PageTracker.TrackPage(this.CurrentPage, this.CurrentView.ToString(), tp.Item2);
            }
            else
            {
                this.AllowFurtherAction = false;
                MessageDialog.Show(Properties.Resources.StartElementDetailViewNoElementIsSelectedMessage);
                this.AllowFurtherAction = true;
            }
        }

        /// <summary>
        /// Allow to change view value from mode control and update title.
        /// </summary>
        /// <param name="view"></param>
        internal void SetCurrentViewAndUpdateUI(dynamic view)
        {
            var ec = SelectAction.GetDefaultInstance().SelectedElementContextId;

            this.CurrentView = view;
            if (ec != null)
            {
                var tp = GetDataAction.GetProcessAndUIFrameworkOfElementContext(ec.Value);
                PageTracker.TrackPage(this.CurrentPage, this.CurrentView.ToString(), tp.Item2);
            }

            UpdateTitleString();
            UpdateMainCommandButtons();
        }

        /// <summary>
        /// Start snapshot mode with loading data
        /// </summary>
        private void StartLoadingSnapshot(string path, int? selectedElementId = null)
        {
            DisableElementSelector();

            var v = SelectAction.GetDefaultInstance().SelectLoadedData(path, selectedElementId);

            Logger.PublishTelemetryEvent(TelemetryEventFactory.ForLoadDataFile(v.Item2.Mode.ToString()));

            if (v.Item2.Mode == A11yFileMode.Test && !selectedElementId.HasValue)
            {
                StartTestAutomatedChecksView();
            }
            else if (v.Item2.Mode == A11yFileMode.Contrast)
            {
                StartTestAutomatedChecksView(); // we got rid of the CC test tab.
            }
            else // A11yFileMode.Inspect
            {
                this.ctrlCurMode.HideControl();
                this.ctrlCurMode = this.ctrlSnapMode;
                this.ctrlSnapMode.DataContextMode = GetDataContextModeForTest();
                this.CurrentPage = AppPage.Test;
                this.CurrentView = TestView.ElementDetails;
                this.ctrlCurMode.ShowControl();
#pragma warning disable CS4014
                // NOTE: We aren't awaiting this async call, so if you
                // touch it, consider if you need to add the await
                this.ctrlSnapMode.SetElement(v.Item1);
#pragma warning restore CS4014
            }

            PageTracker.TrackPage(this.CurrentPage, this.CurrentView.ToString());
        }
    }
}
