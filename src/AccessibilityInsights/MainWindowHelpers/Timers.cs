// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Enums;
using AccessibilityInsights.SharedUx.Telemetry;
using Axe.Windows.Actions;
using Axe.Windows.Core.Misc;
using System.Globalization;
using System.Timers;
using System.Windows;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for Selection/auto select timers
    /// </summary>
#pragma warning disable CA1001
    // Types that own disposable fields should be disposable
    public partial class MainWindow
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        Timer timerAutoSnap;
        Timer timerSelector;

        int autoSnapCounter;
        const int IntervalTimerAutoSnap = 1000; // milliseconds
        const int InternalTimerSelector = 200; // milliseconds

        /// <summary>
        /// Initialize timer for Element Selector
        /// </summary>
        private void InitTimerSelector()
        {
            // set up mouse Timer
            this.timerSelector = new Timer(InternalTimerSelector);
            this.timerSelector.Elapsed += new ElapsedEventHandler(this.OntimerSelectorElapsedEvent);
            this.timerSelector.Enabled = true;
            this.timerSelector.AutoReset = false;// disable autoreset to do reset in timer handler
        }

        /// <summary>
        /// Selector timer event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OntimerSelectorElapsedEvent(object sender, ElapsedEventArgs e)
        {
            if (this.timerSelector != null)
            {
                if (AllowFurtherAction)
                {
                    Dispatcher.Invoke(delegate ()
                    {
                        lock (_lockObject)
                        {
                            if (this.CurrentPage != AppPage.Exit && AllowFurtherAction)
                            {
                                var sa = SelectAction.GetDefaultInstance();
                                var cecId = sa.SelectedElementContextId;
                                var cec = cecId.HasValue ? GetDataAction.GetElementContext(cecId.Value) : null;
                                if (sa.Select())
                                {
                                    var ec = GetDataAction.GetElementContext(sa.SelectedElementContextId.Value);

                                    if ((cec == null || cec.Element.IsSameUIElement(ec.Element) == false) && ec.Element.IsRootElement() == false)
                                    {
                                        HandleTargetSelectionChanged();
                                    }
                                    if (!sa.IsPaused)
                                    {
                                        cec?.Dispose();
                                        cec = null;
                                    }
                                }
                            }
                        }
                    });
                }

                lock (_lockObject)
                {
                    EnableTimerSafely(timerSelector, true); // make sure that it is disabled.
                }
            }
            else
            {
                lock (_lockObject)
                {
                    EnableTimerSafely(timerSelector, false); // make sure that it is disabled.
                }
            }
        }

        /// <summary>
        /// Initialize Auto Snap timer
        /// </summary>
        private void InitTimerAutoSnap()
        {
            // set up AutoSnap Timer
            this.timerAutoSnap = new Timer(IntervalTimerAutoSnap);
            this.timerAutoSnap.Elapsed += new ElapsedEventHandler(this.OntimerAutoSnapElapsedEvent);
            this.timerAutoSnap.Enabled = false;
            this.timerAutoSnap.AutoReset = false;// disable autoreset to do reset in timer handler
        }

        /// <summary>
        /// Starts auto snap timer for provided number of seconds
        /// </summary>
        /// <param name="count">Length of timer in seconds</param>
        public void StartTimerAutoSnap(int count)
        {
            if (count > 0)
            {
                this.ctrlLiveMode.ccDisplayCount.Count = count;
                this.ctrlLiveMode.ccDisplayCount.Visibility = Visibility.Visible;
                this.autoSnapCounter = count;
                this.timerAutoSnap?.Start();

                if (count != 5)
                {
                    Logger.PublishTelemetryEvent(TelemetryAction.Mainwindow_Timer_Started,
                        TelemetryProperty.Seconds, count.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// Timer handler for AutoSnap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OntimerAutoSnapElapsedEvent(object sender, ElapsedEventArgs e)
        {
            if (this.timerAutoSnap != null)
            {
                Dispatcher.Invoke(delegate ()
                {
                    lock (_lockObject)
                    {
                        if (this.CurrentPage != AppPage.Exit)
                        {
                            this.autoSnapCounter--;
                            if (this.autoSnapCounter == 0)
                            {
                                this.ctrlLiveMode.ccDisplayCount.Visibility = Visibility.Collapsed;
                                HandleModeChangeRequestByTimer();
                            }
                            else
                            {
                                this.ctrlLiveMode.ccDisplayCount.Count = autoSnapCounter;
                                EnableTimerSafely(this.timerAutoSnap, true); // make sure that it is disabled.
                            }
                        }
                    }
                });
            }
            else
            {
                EnableTimerSafely(this.timerAutoSnap, false); // make sure that it is disabled.
            }
        }

        /// <summary>
        /// Safely enabling timer
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="enabled"></param>
        private void EnableTimerSafely(Timer timer, bool enabled)
        {
            if (this.isClosed == false)
            {
                timer.Enabled = enabled;
            }
        }
    }
}
