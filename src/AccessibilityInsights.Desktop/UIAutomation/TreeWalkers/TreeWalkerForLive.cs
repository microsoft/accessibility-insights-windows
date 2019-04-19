// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Axe.Windows.Core.Bases;
using Axe.Windows.Telemetry;
using UIAutomationClient;
using System.Linq;
using Axe.Windows.Core.Enums;
using System.Runtime.InteropServices;

namespace Axe.Windows.Desktop.UIAutomation.TreeWalkers
{
    /// <summary>
    /// Wrapper for UIAutomation Tree Walker for Live mode. 
    /// it is based on 2nd edition of TreeWalker
    /// </summary>
    public class TreeWalkerForLive
    {
        /// <summary>
        /// List to keep all elements in tree walking(Ancestors, self and children)
        /// </summary>
        public List<A11yElement> Elements { get; private set; }

        /// <summary>
        /// Top root node in whole hierarchy
        /// </summary>
        public A11yElement RootElement { get; private set; }

        TreeViewMode WalkerMode;

        /// <summary>
        /// Last walk time spane
        /// </summary>
        public TimeSpan LastWalkTime { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="tf"></param>
        /// <param name="showAncestry"></param>
        public TreeWalkerForLive()
        {
            this.Elements = new List<A11yElement>();
        }

        /// <summary>
        /// Refresh tree node data with all children at once. 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mode"></param>
        /// <param name="showAncestry"></param>
        public void GetTreeHierarchy(A11yElement e, TreeViewMode mode)
        {
            var begin = DateTime.Now;
            this.WalkerMode = mode;

            //Set parent of Root explicitly for testing.
            A11yElement parent = null;
            var ancestry = new DesktopElementAncestry(this.WalkerMode, e);
            parent = ancestry.Last;

            this.RootElement = ancestry.First;

            // clear children
            e.Children?.ForEach(c => c.Dispose());
            e.Children?.Clear();

            // populate selected element relationship and add it to list. 
            e.Parent = ancestry.Last;
            e.TreeWalkerMode = this.WalkerMode; // set tree walker mode. 
            e.UniqueId = 0; // it is the selected element which should be id 0.
            this.Elements.Add(e);

            PopulateChildrenTreeNode(e, ancestry.NextId);

            // populate decendent Elements first in parallel
            this.Elements.AsParallel().ForAll(el =>
            {
                el.PopulateMinimumPropertiesForSelection();
            });

            // Add ancestry into Elements list.
            this.Elements.AddRange(ancestry.Items);

            this.LastWalkTime = DateTime.Now - begin;
        }

        /// <summary>
        /// Populate tree by retrieving all children at once.
        /// </summary>
        /// <param name="rootNode"></param>
        private void PopulateChildrenTreeNode(A11yElement rootNode, int startId)
        {
            int childId = startId;

            IUIAutomationTreeWalker walker = A11yAutomation.GetTreeWalker(this.WalkerMode);
            IUIAutomationElement child = (IUIAutomationElement)rootNode.PlatformObject;

            if (child != null)
            {
                try
                {
                    child = walker.GetFirstChildElement(child);
                }
                catch (Exception ex)
                {
                    ex.ReportException();
                    child = null;
                    System.Diagnostics.Trace.WriteLine("Tree walker exception: " + ex);
                }

                while (child != null)
                {
                    // Create child without populating basic property. it will be set all at once in parallel.
                    var childNode = new DesktopElement(child, true, false)
                    {
                        UniqueId = childId++
                    };

                    rootNode.Children.Add(childNode);

                    childNode.Parent = rootNode;
                    childNode.TreeWalkerMode = this.WalkerMode;

                    this.Elements.Add(childNode);
                    try
                    {
                        child = walker.GetNextSiblingElement(child);
                    }
                    catch (Exception ex)
                    {
                        ex.ReportException();
                        child = null;
                        System.Diagnostics.Trace.WriteLine("Tree walker exception: " + ex);
                    }
                }
            }

            Marshal.ReleaseComObject(walker);
        }
    }
}
