// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Telemetry;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Trackers;
using Axe.Windows.Core.Exceptions;
using System;
using System.Media;

namespace AccessibilityInsights
{
    /// <summary>
    /// Used to configure TreeNavigation behavior
    /// </summary>
    interface IControlTreeNavigation
    {
        bool IsTreeNavigationAllowed();
    }

    /// <summary>
    /// Used to dispatch accessible tree navigation commands from the UI to the SelectAction class.
    /// Handles navigation failures and provides the required UX.
    /// </summary>
    class TreeNavigator
    {
        readonly IControlTreeNavigation Controller;
        private readonly object _lockObject = new object();

        public Action SelectionChanged;

        public TreeNavigator(IControlTreeNavigation controller)
        {
            this.Controller = controller;
        }

        public void MoveToParent()
        {
            MoveTo(this.MoveToParent);
        }

        private void MoveToParent(TreeTracker treeTracker)
        {
            treeTracker?.MoveToParent();
        }

        public void MoveToFirstChild()
        {
            MoveTo(this.MoveToFirstChild);
        }

        private void MoveToFirstChild(TreeTracker treeTracker)
        {
            treeTracker?.MoveToFirstChild();
        }

        public void MoveToLastChild()
        {
            MoveTo(this.MoveToLastChild);
        }

        private void MoveToLastChild(TreeTracker treeTracker)
        {
            treeTracker?.MoveToLastChild();
        }

        public void MoveToNextSibbling()
        {
            MoveTo(this.MoveToNextSibbling);
        }

        private void MoveToNextSibbling(TreeTracker treeTracker)
        {
            treeTracker?.MoveToNextSibling();
        }

        public void MoveToPreviousSibbling()
        {
            MoveTo(this.MoveToPreviousSibbling);
        }

        private void MoveToPreviousSibbling(TreeTracker treeTracker)
        {
            treeTracker?.MoveToPreviousSibling();
        }

        private delegate void MoveToDelegate(TreeTracker treeTracker);

        /// <summary>
        /// Performs the primary actions of the class: dispatching commands to SelectAction and handling possible navigation failures.
        /// </summary>
        /// <param name="moveTo">
        /// a delegate for calling the desired type of tree navigation.
        /// </param>
        private void MoveTo(MoveToDelegate moveTo)
        {
            lock (_lockObject)
            {
                if (this.Controller == null)
                    return;
                if (!this.Controller.IsTreeNavigationAllowed())
                    return;

                try
                {
                    var selectAction = SelectAction.GetDefaultInstance();
                    moveTo(selectAction?.TreeTracker);
                    this.SelectionChanged?.Invoke();
                }
                catch (TreeNavigationFailedException e)
                {
                    e.ReportException();
                    HandleTreeNavigationFailure();
                }
            } // lock
        }

        private static void HandleTreeNavigationFailure()
        {
            SystemSounds.Asterisk.Play();
        }
    } // class
} // namespace
