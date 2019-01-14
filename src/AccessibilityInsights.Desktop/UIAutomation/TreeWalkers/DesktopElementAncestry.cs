// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UIAutomationClient;

namespace AccessibilityInsights.Desktop.UIAutomation.TreeWalkers
{
    /// <summary>
    /// class DesktopElementAncestry
    /// Get the ancestry hierarchy
    /// </summary>
    public class DesktopElementAncestry
    {
        /// <summary>
        /// Oldest in Ancestry
        /// </summary>
        public A11yElement First { get; private set; }

        /// <summary>
        /// Last in Ancestry
        /// </summary>
        public A11yElement Last { get; private set; }

        public List<A11yElement> Items { get; private set; }

        private readonly IUIAutomationTreeWalker TreeWalker;

        public TreeViewMode TreeWalkerMode { get; }

        /// <summary>
        /// Parent elements' SetMembers value
        /// </summary>
        bool SetMembers { get; set; }

        /// <summary>
        /// Id for next element 
        /// it will be used in Tree Walker.
        /// </summary>
        public int NextId { get; }

        /// <summary>
        /// Constructor DesktopElementAncestry
        /// Get Ancestry Tree elements up to Desktop(at best)
        /// </summary>
        /// <param name="walker"></param>
        /// <param name="e"></param>
        public DesktopElementAncestry(TreeViewMode mode, A11yElement e, bool setMem = false)
        {
            this.TreeWalker = A11yAutomation.GetTreeWalker(mode);
            this.TreeWalkerMode = mode;
            this.Items = new List<A11yElement>();
            this.SetMembers = setMem;
            SetParent(e,-1);

            if (Items.Count != 0)
            {
                this.First = Items.Last();
                this.Last = Items.First();
                if (this.Last.IsRootElement() == false)
                {
                    this.Last.Children.Clear();
                    this.NextId = PopulateSiblingTreeNodes(this.Last, e);
                }
                else
                {
                    this.NextId = 1;
                }
            }

            Marshal.ReleaseComObject(this.TreeWalker);
        }

        /// <summary>
        /// Set Parent to build ancestry tree
        /// </summary>
        /// <param name="e"></param>
        /// <param name="uniqueId"></param>
        private void SetParent(A11yElement e, int uniqueId)
        {
            if (e == null || e.PlatformObject == null || e.IsRootElement()) return;

            try
            {
                var puia = this.TreeWalker.GetParentElement((IUIAutomationElement)e.PlatformObject);
                DesktopElement parent = null;

                if (puia != null)
                {
                    parent = new DesktopElement(puia, true, SetMembers);
                    parent.PopulateMinimumPropertiesForSelection();

                    // we need to avoid infinite loop of self reference as parent. 
                    // it is a probably a bug in UIA or the target app. 
                    if (e.IsSameUIElement(parent) == false)
                    {
                        parent.IsAncestorOfSelected = true;
                        parent.Children.Add(e);
                        e.Parent = parent;
                        this.Items.Add(parent);
                        parent.UniqueId = uniqueId;

                        SetParent(parent, uniqueId - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                // ignore to show the best efforts.
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// Populate siblings
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="poiNode"></param>
        /// <param name="startId"></param>
        private int PopulateSiblingTreeNodes(A11yElement parentNode, A11yElement poiNode)
        {
            int childId = 1;

            IUIAutomationTreeWalker walker = this.TreeWalker;
            IUIAutomationElement child = null;

            if ((IUIAutomationElement)parentNode.PlatformObject != null)
            {
                try
                {
                    child = walker.GetFirstChildElement((IUIAutomationElement)parentNode.PlatformObject);
                }
                catch (Exception ex)
                {
                    child = null;
                    System.Diagnostics.Trace.WriteLine("Tree walker exception: " + ex);
                }

                while (child != null)
                {
                    var childNode = new DesktopElement(child, true, false);
                    childNode.PopulateMinimumPropertiesForSelection();

                    if (childNode.IsSameUIElement(poiNode) == false)
                    {
                        childNode.UniqueId = childId++;
                        childNode.Parent = parentNode;
                        childNode.TreeWalkerMode = this.TreeWalkerMode;
                        this.Items.Add(childNode);
                    }
                    else
                    {
                        childNode = poiNode as DesktopElement;
                    }

                    parentNode.Children.Add(childNode);

                    try
                    {
                        child = walker.GetNextSiblingElement(child);
                    }
                    catch (Exception ex)
                    {
                        child = null;
                        System.Diagnostics.Trace.WriteLine("Tree walker exception: " + ex);
                    }
                }
            }

            return childId;
        }

    }
}
