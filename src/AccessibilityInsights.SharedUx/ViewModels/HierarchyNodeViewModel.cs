// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Properties;
using AccessibilityInsights.SharedUx.Utilities;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Results;
using Axe.Windows.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// HierarchyTreeItem class
    /// ViewModel for A11yElement in Hierarchy tree
    /// </summary>
    public class HierarchyNodeViewModel: ViewModelBase
    {
        const int NormalIconSizeBack = 14; // size for non composite icon
        const int TreeIconSizeBack = 16;   // size for tree in composite icon
        const int NormalIconSizeFront = 9; // size for error state in composite icon

        /// <summary>
        /// Show uncertain results Icon
        /// </summary>
        private readonly bool ShowUncertain;

        /// <summary>
        /// Element which is represented via this ViewModel
        /// </summary>
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Child ViewModels
        /// </summary>
        public IList<HierarchyNodeViewModel> Children { get; private set; }

#pragma warning disable CA1819 // Properties should not return arrays
        /// <summary>
        /// Counts of aggregate scan statuses
        ///     result[ScanStatusEnum] -> number of descendents with that scan status (including this element)
        /// </summary>
        public int[] AggregateStatusCounts { get; private set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// indicate whether new Snapshot is needed if asked for expand.
        /// </summary>
        public bool NeedSnapshot { get; }

        /// <summary>
        /// Error icon to show
        /// </summary>
        public FabricIcon? IconBack { get; private set; }

        /// <summary>
        /// Icon to overlay.
        /// </summary>
        public FabricIcon? IconFront { get; private set; }

        /// <summary>
        /// Background Icon Size
        /// </summary>
        public int IconSizeBack { get; private set; } = NormalIconSizeBack;

        /// <summary>
        /// front Icon size
        /// </summary>
        public int IconSizeFront { get; private set; } = NormalIconSizeFront;

        /// <summary>
        /// Whether or not the File Bug button should be made visible
        /// </summary>
        public static Visibility FileBugVisibility => HelperMethods.FileBugVisibility;

        private Visibility _iconVisibility = Visibility.Collapsed;
        /// <summary>
        /// Visibility of error icon
        /// </summary>
        public Visibility IconVisibility
        {
            get
            {
                return this._iconVisibility;
            }

            private set
            {
                this._iconVisibility = value;
                OnPropertyChanged(nameof(IconVisibility));
            }
        }

        /// <summary>
        /// Display text for the issue
        /// </summary>
        public string IssueDisplayText
        {
            get
            {
                return Element.IssueDisplayText;
            }
            set
            {
                Element.IssueDisplayText = value;
                OnPropertyChanged(nameof(IssueDisplayText));
                OnPropertyChanged(nameof(BugVisibility));
            }
        }

        /// <summary>
        /// Used to store the issue link.
        /// </summary>
        public Uri IssueLink { get; set; }

        /// <summary>
        /// Visibility of bug icon when node selected
        /// </summary>
        public Visibility BugVisibility
        {
            get
            {
                return string.IsNullOrEmpty(IssueDisplayText) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// AutomationProperties.Name
        /// </summary>
        public string AutomationName { get; private set; }

        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get
            {
                return this._visibility;
            }

            private set
            {
                this._visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        /// <summary>
        /// to indicate whether the node should be hilighted or not
        /// </summary>
        private bool _ishilighted;
        public bool IsHilighted
        {
            get
            {
                return this._ishilighted;
            }

            private set
            {
                this._ishilighted = value;
                OnPropertyChanged(nameof(IsHilighted));
            }
        }

        /// <summary>
        /// Is node selected
        /// </summary>
        private bool _isselected;
        public bool IsSelected
        {
            get
            {
                return this._isselected;
            }

            set
            {
                this._isselected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// Is node selected
        /// </summary>
        private bool _isexpanded;
        public bool IsExpanded
        {
            get
            {
                return this._isexpanded;
            }

            set
            {
                this._isexpanded = value;

                this.IconVisibility = (value && this.IconBack != FabricIcon.DOM && this.IconBack != default(FabricIcon)) || (!value && this.IconBack != default(FabricIcon)) ? Visibility.Visible : Visibility.Collapsed;

                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        /// <summary>
        /// Header for tree node item
        /// </summary>
        public string Header { get; private set; }

        private readonly string SearchContext;

        /// <summary>
        /// These are searchable string properties
        /// Not all string properties are included to improve performance
        /// </summary>
        private static int[] CommonStringProperties =
        {
            PropertyType.UIA_LocalizedControlTypePropertyId,
            PropertyType.UIA_NamePropertyId,
            PropertyType.UIA_AutomationIdPropertyId,
            PropertyType.UIA_ClassNamePropertyId,
            PropertyType.UIA_HelpTextPropertyId,
            PropertyType.UIA_FrameworkIdPropertyId,
            PropertyType.UIA_ItemStatusPropertyId,
            PropertyType.UIA_ItemTypePropertyId,
            PropertyType.UIA_ValuePattern_ValuePropertyId,
            PropertyType.UIA_AriaRolePropertyId,
            PropertyType.UIA_AriaPropertiesPropertyId,
            PropertyType.UIA_LocalizedLandmarkTypePropertyId,
            PropertyType.UIA_FullDescriptionPropertyId,
        };

        /// <summary>
        /// Is node for live mode
        /// </summary>
        public bool IsLiveMode { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ce">Current Element</param>
        /// <param name="se">Selected Element</param>
        /// <param name="showUncertain">Show uncertain result icon or not. </param>
        /// <param name="needSnapshotMenu">default is true</param>
        /// <param name="isLiveMode">Is node for live mode</param>
        public HierarchyNodeViewModel(A11yElement ce, bool showUncertain, bool isLiveMode)
        {
            this.ShowUncertain = showUncertain;
            this.Element = ce ?? throw new ArgumentNullException(nameof(ce));

            if (this.Element.UniqueId == 0)
            {
                this.IsExpanded = true;
                this.IsSelected = true;
                this.NeedSnapshot = false;
            }
            else if (this.Element.UniqueId > 0)
            {
                this.IsExpanded = false;
                this.IsSelected = false;
                this.NeedSnapshot = false;
            }
            else
            {
                this.IsExpanded = true;
                this.IsSelected = false;
                this.NeedSnapshot = true;
            }

            if (ce.Children != null)
            {
                this.Children = (from c in ce.Children
                                 select new HierarchyNodeViewModel(c, showUncertain, isLiveMode)).ToList();
            }

            if (isLiveMode == false)
            {
                var childrenAggregateCounts = (from c in this.Children
                                               select c.AggregateStatusCounts);

                // Sum together counts from immediate children & increment current element's status
                this.AggregateStatusCounts = childrenAggregateCounts.Aggregate(new int[Enum.GetNames(typeof(ScanStatus)).Length],
                                                        (countsA, countsB) => countsA.Zip(countsB, (x, y) => x + y).ToArray());
                this.AggregateStatusCounts[(int)this.Element.TestStatus]++;
            }

            this.Header = ce.Glimpse;
            this.SearchContext = CreateSearchContext(ce);
            this.IsLiveMode = isLiveMode;
            UpdateIconInfoAndAutomationName(isLiveMode);
        }

        /// <summary>
        /// Update Icon information and AutomationProperties.Name
        /// since they share similar logic, combine calculation to improve perf.
        /// </summary>
        /// <param name="isLiveMode">Is node in live mode</param>
        private void UpdateIconInfoAndAutomationName(bool isLiveMode)
        {
            this.IconVisibility = Visibility.Collapsed;
            string automationNameFormat = Resources.HierarchyNodeViewModel_AutomationNameDefaultFormat;

            if (!isLiveMode)
            {
                if (this.Element.TestStatus == ScanStatus.Fail)
                {
                    this.IconBack = FabricIcon.AlertSolid;
                    this.IconSizeBack = NormalIconSizeBack;
                    this.IconVisibility = Visibility.Visible;
                    automationNameFormat = Resources.HierarchyNodeViewModel_AutomationNameFailedResultsFormat;
                }
                else if(this.Element.TestStatus == ScanStatus.ScanNotSupported)
                {
                    this.IconBack = FabricIcon.MapDirections;
                    this.IconSizeBack = NormalIconSizeBack;
                    this.IconVisibility = Visibility.Visible;
                    automationNameFormat = Resources.HierarchyNodeViewModel_AutomationNameNotSupportResultsFormat;
                }
                else if (this.Element.TestStatus == ScanStatus.Uncertain && this.ShowUncertain)
                {
                    this.IconBack = FabricIcon.IncidentTriangle;
                    this.IconSizeBack = NormalIconSizeBack;
                    this.IconVisibility = Visibility.Visible;
                    automationNameFormat = Resources.HierarchyNodeViewModel_AutomationNameUncertainResultsFormat;
                }
                else
                {
                    // no result or all pass, we need to use aggregated data.
                    this.IconBack = FabricIcon.DOM;
                    this.IconSizeBack = TreeIconSizeBack;

                    if (this.AggregateStatusCounts[(int)ScanStatus.Fail] > 0)
                    {
                        this.IconFront = FabricIcon.AlertSolid;
                        automationNameFormat = Resources.HierarchyNodeViewModel_AutomationNameFailedResultsInDescendentsFormat;
                    }
                    else if (this.AggregateStatusCounts[(int)ScanStatus.Uncertain] > 0 && this.ShowUncertain)
                    {
                        this.IconFront = FabricIcon.IncidentTriangle;
                        automationNameFormat = Resources.HierarchyNodeViewModel_AutomationNameUncertainResultsInDescendentsFormat;
                    }
                    else
                    {
                        this.IconBack = default(FabricIcon);
                        automationNameFormat = Resources.HierarchyNodeViewModel_AutomationNameNoFailedOrUncertainResultsInDescendentsFormat;
                    }

                    this.IconVisibility = this.IsExpanded == false && this.IconBack != default(FabricIcon) ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            this.AutomationName = string.Format(CultureInfo.InvariantCulture, automationNameFormat,
                this.Element.Glimpse);
        }

        public bool ApplyFilter(string filter)
        {
            bool isfiltered = true;

            if (this.Children != null && this.Children.Count != 0)
            {
                var cnt = (from c in this.Children
                           let f = c.ApplyFilter(filter)
                           where f == false
                           select c).Count();
                if (cnt != 0)
                {
                    isfiltered = false;
                }
            }

            if (string.IsNullOrEmpty(filter)
                || (this.SearchContext != null
                && this.SearchContext.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1))
            {
                isfiltered = false;
                this.IsHilighted = true;
            }

            if(isfiltered)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }

            return isfiltered;
        }

        public void SelectFirstVisibleLeafNode()
        {
            SelectFirstVisibleLeafNodeWorker();
        }

        private bool SelectFirstVisibleLeafNodeWorker()
        {
            if (this.Children != null)
            {
                foreach (var child in this.Children)
                {
                    if (child.SelectFirstVisibleLeafNodeWorker())
                        return true;
                } // for each child
            }

            if (this.IsVisible)
            {
                this.IsSelected = true;
                return true;
            }

            return false;
        }

        private bool IsVisible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        /// <summary>
        /// Expand tree
        /// if expandchildren is true, make children expanded too
        /// it will expand all descendants
        /// </summary>
        /// <param name="expandchildren"></param>
        internal void Expand(bool expandchildren = false)
        {
            this.IsExpanded = true;

            if(expandchildren && this.Children.Count != 0)
            {
                foreach(var c in this.Children)
                {
                    c.Expand(true);
                }
            }
        }

        internal void Clear()
        {
            if (this.Children != null)
            {
                foreach (var c in this.Children)
                {
                    c.Clear();
                }

                this.Children.Clear();
            }

            this.Element = null;
        }

        private static string CreateSearchContext(A11yElement e)
        {
            if (e == null)
                return null;

            var properties = CreateSearchPropertiesString(e);
            var patterns = CreateSearchPatternsString(e);
            var controlTypeName = ControlType.GetInstance().GetNameById(e.ControlTypeId);

            return string.Join(" ", properties, patterns, controlTypeName);
        }

        private static string CreateSearchPropertiesString(A11yElement e)
        {
            if (e == null)
                return null;

            var properties = from p in CommonStringProperties
                             let value = e.GetPropertyOrDefault<string>(p)
                             select value;

            if (properties == null)
                return null;

            return string.Join(" ", properties);
        }

        private static string CreateSearchPatternsString(A11yElement e)
        {
            if (e == null)
                return null;

            var patterns = e.Patterns?.Select(p => PatternType.GetInstance().GetNameById(p.Id));
            if (patterns == null)
                return null;

            return string.Join(" ", patterns);
        }
    }
}
