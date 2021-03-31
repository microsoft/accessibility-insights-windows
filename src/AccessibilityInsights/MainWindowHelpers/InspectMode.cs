// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Highlighting;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Enums;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for HandleTestStartUpRequest
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Start Inspect Mode
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        void StartInspectMode(InspectView view)
        {
            switch (view)
            {
                case InspectView.Live:
                    StartLiveMode();
                    break;
            }
        }

        /// <summary>
        /// Start Live Monitor mode
        /// show only single selected item information
        /// </summary>
        private void StartLiveMode()
        {
            if (!(this.CurrentPage == AppPage.Inspect && (InspectView)this.CurrentView == InspectView.Live))
            {
                this.CurrentPage = AppPage.Inspect;
                this.CurrentView = InspectView.Live;
                PageTracker.TrackPage(this.CurrentPage, this.CurrentView.ToString());

                // make sure that highlighter is cleared for new selection.
                HollowHighlightDriver.GetDefaultInstance().Clear();

                SetWindowForLiveMode();
            }

            // set the state to Capturing view. it will prevent testing for safety.
            this.CurrentView = InspectView.CapturingData;

            var ecId = SelectAction.GetDefaultInstance().SelectedElementContextId;
            if (ecId != null)
            {
                // make sure that no more selection is requested.
                DisableElementSelector();

                // Pass the currently selected Element Context
                this.ctrlCurMode.SetElement(ecId.Value);
            }
            else
            {
                // make sure that state is set to Live since View is not changed via SetElement.
                SetCurrentViewAndUpdateUI(InspectView.Live);
                EnableElementSelector();
            }
        }

        /// <summary>
        /// Sets the Window size for Live Mode and saves previous size config
        /// if desired.
        /// </summary>
        /// <param name="saveSnap">Value indicating if previous window size should be saved.</param>
        private void SetWindowForLiveMode()
        {
            this.ctrlCurMode.HideControl();
            this.ctrlCurMode = this.ctrlLiveMode;
            this.ctrlCurMode.ShowControl();

            this.ctrlLiveMode.ctrlTabs.Focus();
        }

        /// <summary>
        /// Get appropriate DataContextMode based on datacontext of selected element context.
        /// </summary>
        /// <returns></returns>
        private static DataContextMode GetDataContextModeForTest()
        {
            var mode = GetDataAction.GetDataContextMode();

            switch(mode)
            {
                case DataContextMode.Live:
                    return DataContextMode.Test;
            }

            return mode;
        }
    }
}
