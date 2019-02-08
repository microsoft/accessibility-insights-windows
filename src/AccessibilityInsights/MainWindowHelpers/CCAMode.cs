// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.Actions;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.DesktopUI.Enums;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for HandleCCAStartUpRequest
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Start CCA Mode
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        void StartCCAMode(CCAView view)
        {
            switch (view)
            {
                case CCAView.Automatic:
                    StartCCAMode();
                    break;             
            }
        }

        /// <summary>
        /// Start CCA Automatic Monitor mode
        /// show only single selected item information
        /// </summary>
        private void StartCCAMode()
        {
            if (!(this.CurrentPage == AppPage.CCA && (CCAView)this.CurrentView == CCAView.Automatic))
            {
                this.CurrentPage = AppPage.CCA;
                this.CurrentView = CCAView.Automatic;
                PageTracker.TrackPage(this.CurrentPage, this.CurrentView.ToString());

                // make sure that highlighter is cleared for new selection. 
                HighlightAction.GetDefaultInstance().Clear();

                SetWindowForAutomaticMode();
            }

            // set the state to Capturing view. it will prevent testing for safety.
            this.CurrentView = CCAView.CapturingData;

            var ecId = SelectAction.GetDefaultInstance().GetSelectedElementContextId();
            if (ecId != null)
            {
                // make sure that no more selection is requested.
                DisableElementSelector();

                // Pass the currently selected Element Context
                this.ctrlCurMode.SetElement(ecId.Value);
            }

            SetCurrentViewAndUpdateUI(CCAView.Automatic);
            
            // enable element selector
            EnableElementSelector();

            // if it was open when the switch back button is clicked. 
            HideConfigurationMode();

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Sets the Window size for CCA Automatic Mode and saves previous size config
        /// if desired.
        /// </summary>
        /// <param name="saveSnap">Value indicating if previous window size should be saved.</param>
        private void SetWindowForAutomaticMode()
        {
            this.ctrlCurMode.HideControl();
            this.ctrlCurMode = this.ctrlCCAMode;
            this.ctrlCurMode.ShowControl();

            this.ctrlCCAMode.ctrlContrast.Focus();
        }

        /// <summary>
        /// Get appropriate DataContextMode based on datacontext of selected element context.
        /// </summary>
        /// <returns></returns>
        private static DataContextMode GetDataContextModeForCCA()
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
