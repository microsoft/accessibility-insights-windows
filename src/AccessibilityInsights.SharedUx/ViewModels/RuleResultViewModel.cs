// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Utilities;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Results;
using Axe.Windows.Desktop.Utility;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Class RuleResultViewModel
    /// this is for FastPass scan result display
    /// </summary>
    public class RuleResultViewModel : ViewModelBase, IIssueFilingSource
    {
        /// <summary>
        /// Element
        /// </summary>
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Description of ScanResults group
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Source of rule, such as "A11yCriteria 4.1.2"
        /// </summary>
        public string Source { get; }

#pragma warning disable CA1056 // Uri properties should not be strings
        /// <summary>
        /// Link to more information
        /// </summary>
        public string URL { get; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        /// Combine title and URL
        /// </summary>
        public Tuple<String, String> TitleURL
        {
            get
            {
                return new Tuple<string, string>(Description, URL);
            }
        }

        /// <summary>
        /// Bug id of this element
        /// </summary>
        public string IssueDisplayText
        {
            get
            {
                return RuleResult.IssueDisplayText;
            }
            set
            {
                RuleResult.IssueDisplayText = value;
                OnPropertyChanged(nameof(IssueDisplayText));
            }
        }

        /// <summary>
        /// Used to store the issue link.
        /// </summary>
        public Uri IssueLink
        {
            get
            {
                return RuleResult.IssueLink;
            }
            set
            {
                RuleResult.IssueLink = value;
                OnPropertyChanged(nameof(IssueLink));
            }
        }

        /// <summary>
        /// Properties formatted as name=value
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// URL to snippet query
        /// </summary>
        public string SnippetLink { get; }

        /// <summary>
        /// Name of Glyph for Fabric Icon
        /// </summary>
        public FabricIcon GlyphName { get; }

        /// <summary>
        /// reference to rule result object
        /// </summary>
        public RuleResult RuleResult { get; }

        private System.Windows.Visibility loadingVisibility;
        /// <summary>
        /// Attachment loading
        /// </summary>
        public System.Windows.Visibility LoadingVisibility
        {
            get
            {
                return loadingVisibility;
            }
            set
            {
                loadingVisibility = value;
                OnPropertyChanged(nameof(LoadingVisibility));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e">A11yElement</param>
        /// <param name="rr">Scan result</param>
        public RuleResultViewModel(A11yElement e, RuleResult rr)
        {
            if (rr == null)
                throw new ArgumentNullException(nameof(rr));

            this.Element = e ?? throw new ArgumentNullException(nameof(e));

            var p = e.Properties.ById(rr.MetaInfo.PropertyId);
            if (p != null)
            {
                this.Properties = String.Format(CultureInfo.InvariantCulture, "{0}={1}", p.Name, p.TextValue);
            }

            if (StandardLinksHelper.GetDefaultInstance().HasStoredLink(rr.MetaInfo))
            {
                this.SnippetLink = StandardLinksHelper.GetDefaultInstance().GetSnippetQueryUrl(rr.MetaInfo);
            }
            this.LoadingVisibility = System.Windows.Visibility.Collapsed;
            this.Description = rr.Description;
            this.URL = rr.HelpUrl.Url;
            this.Source = rr.Source;
            this.LoadingVisibility = System.Windows.Visibility.Collapsed;
            this.RuleResult = rr;
            // show website icon for NotSupported case
            this.GlyphName = rr.Status == ScanStatus.ScanNotSupported ? FabricIcon.MapDirections : FabricIcon.AlertSolid;
        }

        /// <summary>
        /// String for narrator
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Element.Glimpse;
        }

        /// <summary>
        /// Whether or not the File Bug button should be made visible
        /// </summary>
        public static Visibility FileBugVisibility => HelperMethods.FileBugVisibility;

        /// <summary>
        /// Returns a BugInformation about the bug from the given view model.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public IssueInformation GetIssueInformation()
        {
            return new IssueInformation(
                glimpse: this.Element.Glimpse,
                howToFixLink: this.SnippetLink.ToUri(),
                helpUri: this.URL.ToUri(),
                ruleSource: this.Source,
                ruleDescription: this.Description,
                ruleForTelemetry: this.RuleResult.Rule.ToString(),
                uiFramework: this.RuleResult.MetaInfo.UIFramework,
                processName: this.Element.GetProcessName(),
                windowTitle: this.Element.GetOriginAncestor(Axe.Windows.Core.Types.ControlType.UIA_WindowControlTypeId).Glimpse,
                elementPath: string.Join("<br/>", this.Element.GetPathFromOriginAncestor().Select(el => el.Glimpse)),
                testMessages: string.Join("<br/>", this.RuleResult.Messages),
                internalGuid: Guid.NewGuid(),
                issueType: IssueType.SingleFailure
            );
        }
    }
}
