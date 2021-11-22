// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Properties;
using AccessibilityInsights.SharedUx.Utilities;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.HelpLinks;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Results;
using Axe.Windows.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// ViewModel class for scanlistview Item
    /// </summary>
    public class ScanListViewItemViewModel:ViewModelBase
    {
        public static IList<ScanListViewItemViewModel> GetScanListViewItemViewModels(A11yElement e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            ScanResults tr = e.ScanResults;

            var list = new List<ScanListViewItemViewModel>();

            foreach(var sr in tr.Items)
            {
                foreach(var rr in sr.Items )
                {
                    list.Add(new ScanListViewItemViewModel(e, rr));
                }
            }

            return list;
        }

        public string Header { get; private set; }

        /// <summary>
        /// Scan status
        /// </summary>
        public ScanStatus Status { get; private set; }
        public HelpUrl HelpUrl { get; private set; }
        public Visibility HelpLinkVisibility { get; private set; }

        /// <summary>
        /// Whether or not the File Bug button should be made visible
        /// </summary>
        public static Visibility FileBugVisibility => HelperMethods.FileBugVisibility;

        /// <summary>
        /// Source of rule, such as "A11yCriteria 4.1.2"
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Error icon to show
        /// </summary>
        public FabricIcon Icon
        {
            get
            {
                if (this.Status == ScanStatus.Fail)
                {
                    return FabricIcon.AlertSolid;
                }
                else if (this.Status == ScanStatus.Pass)
                {
                    return FabricIcon.CompletedSolid;
                }
                else if(this.Status == ScanStatus.ScanNotSupported)
                {
                    return FabricIcon.MapDirections;
                }
                else
                {
                    return FabricIcon.IncidentTriangle;
                }
            }
        }

        public string HowToFixText { get; private set; }

        public string SnippetLink { get; private set; }

        public string AutomationHelpText { get; private set; }

        /// <summary>
        /// RuleResult used to create VM
        /// </summary>
        public RuleResult RR { get; private set; }

        /// <summary>
        /// Element scanned
        /// </summary>
        public A11yElement Element { get; private set; }

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
        /// Display text for this issue
        /// </summary>
        public string IssueDisplayText
        {
            get
            {
                return RR.IssueDisplayText;
            }
            set
            {
                RR.IssueDisplayText = value;
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
                return RR.IssueLink;
            }
            set
            {
                RR.IssueLink = value;
                OnPropertyChanged(nameof(IssueLink));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sr"></param>
        public ScanListViewItemViewModel(A11yElement e, RuleResult sr)
        {
            this.RR = sr ?? throw new ArgumentNullException(nameof(sr));
            this.Element = e;
            this.Header = sr.Description;
            this.Status = sr.Status;
            this.HelpUrl = sr.HelpUrl;
            this.HelpLinkVisibility = this.HelpUrl != null ? Visibility.Visible : Visibility.Collapsed;

            if (this.Status != ScanStatus.Pass && sr.MetaInfo.PropertyId != 0 && StandardLinksHelper.GetDefaultInstance().HasStoredLink(sr.MetaInfo))
            {
                this.SnippetLink = StandardLinksHelper.GetDefaultInstance().GetSnippetQueryUrl(sr.MetaInfo);
            }
            else
            {
                this.SnippetLink = null;
            }

            this.AutomationHelpText = GetAutomationHelpText();

            StringBuilder sb = new StringBuilder();
            foreach (var message in sr.Messages)
            {
                sb.AppendLine(message);
            }
            this.HowToFixText = sb.ToString();
            this.Source = sr.Source;
        }

        private string GetAutomationHelpText()
        {
            return this.HelpUrl != null ? (this.HelpUrl.Type == Axe.Windows.Core.Enums.UrlType.Info ? Resources.ScanListViewItemViewModel_GetAutomationHelpText_This_result_has_the_information_link_button : Resources.ScanListViewItemViewModel_GetAutomationHelpText_This_result_has_fix_snippet_link_button) : null;
        }

        /// <summary>
        /// To string for UIA Name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, 
                Resources.ScanListViewItemViewModel_StatusFormat,
                this.Status, this.Header);
        }

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
                helpUri: this.HelpUrl?.Url?.ToUri(),
                ruleSource: this.Source,
                ruleDescription: this.RR.Description,
                processName: this.Element.GetProcessName(),
                ruleForTelemetry: this.RR.Rule.ToString(),
                uiFramework: this.Element.GetUIFramework(),
                windowTitle: this.Element.GetOriginAncestor(Axe.Windows.Core.Types.ControlType.UIA_WindowControlTypeId).Glimpse,
                elementPath: string.Join("<br/>", this.Element.GetPathFromOriginAncestor().Select(el => el.Glimpse)),
                testMessages: string.Join("<br/>", this.RR.Messages),
                internalGuid: Guid.NewGuid(),
                issueType: IssueType.SingleFailure
            );
        }

        #region HelpLink Command
        private ICommand _commandHelpLink;

        public ICommand CommandHelpLink
        {
            get
            {
                return _commandHelpLink ?? (_commandHelpLink = new CommandHandler(() => InvokeHelpLink(), this.HelpUrl != null));
            }
        }

        public void InvokeHelpLink()
        {
            Process.Start(new ProcessStartInfo(this.HelpUrl.Url));

            var dic = new Dictionary<string, string>();
            dic.Add("HelpLink", this.HelpUrl.Url);
            dic.Add("UrlType", this.HelpUrl.Type.ToString());
        }
        #endregion

        #region SnippetLink Command
        private ICommand _commandSnippetLink;

        /// <summary>
        /// Command for Snippet Link
        /// this button is visible and enabled only when Status is failed or uncertain.
        /// </summary>
        public ICommand CommandSnippetLink
        {
            get
            {
                return _commandSnippetLink ?? (_commandSnippetLink = new CommandHandler(() => InvokeSnippetLink(), this.Status != ScanStatus.Pass));
            }
        }

        public void InvokeSnippetLink()
        {
            Process.Start(new ProcessStartInfo(this.SnippetLink));
        }
        #endregion

    }
}
