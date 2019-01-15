// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.InteropServices;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Exceptions;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Desktop.UIAutomation;
using UIAutomationClient;

namespace AccessibilityInsights.Actions.Trackers
{
    /// <summary>
    /// Implements UIA tree navigation logic. Currently used by the SelectAction class.
    /// The class is named to match the MouseTracker and FocusTracker classes, also used by SelectAction.
    /// This class attempts to find the requested nearby element. If successful, the SelectAction class is notified.
    /// If unsuccessful, the TreeNavigationFailedException is thrown.
    /// </summary>
    public class TreeTracker : BaseTracker
    {
        readonly SelectAction SelectAction = null;
        internal TreeViewMode TreeViewMode { get; set; } = TreeViewMode.Raw;

        public TreeTracker(Action<A11yElement> action, SelectAction selectAction)
            : base(action)
        {
            this.SelectAction = selectAction;
        }

        public override void Start()
        { }

        public override void Stop()
        {
            base.Stop();
        }

        public void MoveToParent()
        {
            MoveTo(this.GetParent);
        }

        private IUIAutomationElement GetParent(IUIAutomationTreeWalker treeWalker, IUIAutomationElement element)
        {
            if (element == null) return null;

            return treeWalker?.GetParentElement(element);
        }

        public void MoveToFirstChild()
        {
            MoveTo(this.GetFirstChild);
        }

        private IUIAutomationElement GetFirstChild(IUIAutomationTreeWalker treeWalker, IUIAutomationElement element)
        {
            if (element == null) return null;

            return treeWalker?.GetFirstChildElement(element);
        }

        public void MoveToLastChild()
        {
            MoveTo(this.GetLastChild);
        }

        private IUIAutomationElement GetLastChild(IUIAutomationTreeWalker treeWalker, IUIAutomationElement element)
        {
            if (element == null) return null;

            return treeWalker?.GetLastChildElement(element);
        }

        public void MoveToNextSibbling()
        {
            MoveTo(this.GetNexSibbling);
        }

        private IUIAutomationElement GetNexSibbling(IUIAutomationTreeWalker treeWalker, IUIAutomationElement element)
        {
            if (element == null) return null;

            return treeWalker?.GetNextSiblingElement(element);
        }

        public void MoveToPreviousSibbling()
        {
            MoveTo(this.GetPreviousSibbling);
        }

        private IUIAutomationElement GetPreviousSibbling(IUIAutomationTreeWalker treeWalker, IUIAutomationElement element)
        {
            if (element == null) return null;

            return treeWalker?.GetPreviousSiblingElement(element);
        }

        private delegate IUIAutomationElement GetElementDelegate(IUIAutomationTreeWalker treeWalker, IUIAutomationElement element);

        /// <summary>
        /// Call this function to find a nearby element and update SelectAction if successful.
        /// This function is the starting point for the navigation logic of this class.
        /// </summary>
        /// <param name="getElementMethod">
        /// a delegate used to find the next nearby element.
        /// </param>
        private void MoveTo(GetElementDelegate getElementMethod)
        {
            lock (this)
            {
                _MoveTo(getElementMethod);
            }
        }

        /// <summary>
        /// Do not call this function directly. Instead, call MoveTo().
        /// </summary>
        /// <param name="getElementMethod"></param>
        private void _MoveTo(GetElementDelegate getElementMethod)
        {
            var element = GetNearbyElement(getElementMethod);
            if (element == null) throw new TreeNavigationFailedException();

            var desktopElement = new DesktopElement(element, true, false);
            desktopElement.PopulateMinimumPropertiesForSelection();
            if (desktopElement.IsRootElement() == false)
            {
                this.SelectAction?.SetCandidateElement(desktopElement);
                this.SelectAction?.Select();
            }
            else
            {
                // if it is desktop, release it. 
                desktopElement.Dispose();
                throw new TreeNavigationFailedException();
            }
        }

        private IUIAutomationElement GetNearbyElement(GetElementDelegate getNextElement)
        {
            var currentElement = GetCurrentElement();
            if (currentElement == null) return null;

            var treeWalker = A11yAutomation.GetTreeWalker(this.TreeViewMode);

            var retVal = getNextElement?.Invoke(treeWalker, currentElement);

            // make sure that we skip an element from current process while walking tree.
            // this code should be hit only at App level. but for sure. 
            if(DesktopElement.IsFromCurrentProcess(retVal))
            {
                var tmp = retVal;

                retVal = getNextElement?.Invoke(treeWalker, retVal);

                // since element is not in use, release. 
                Marshal.ReleaseComObject(tmp);
            }

            Marshal.ReleaseComObject(treeWalker);

            return retVal;
        }

        private IUIAutomationElement GetCurrentElement()
        {
            return this.SelectAction?.POIElementContext?.Element?.PlatformObject as IUIAutomationElement;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    } // class
} // namespace
