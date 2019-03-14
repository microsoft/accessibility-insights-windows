// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.HelpLinks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Core.Results
{
    /// <summary>
    /// Class RuleResult
    /// it is a class to containt the result of each individual rules in a Scan. 
    /// </summary>
    public class RuleResult
    {
#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// Messages through Rule checking stages
        /// for JSON serialization, allow set()
        /// </summary>
        public List<string> Messages { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
        /// <summary>
        /// Status of Rule check
        /// </summary>
        public ScanStatus Status { get; set; } = ScanStatus.Pass;

        [JsonConverter(typeof(StringEnumConverter))]
        public RuleId Rule { get; set; }

        /// <summary>
        /// Name of the Rule
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Source of the rule (ex, A11yCriteria 4.1.2)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Meta info for query
        /// </summary>
        public ScanMetaInfo MetaInfo { get; set; }

        /// <summary>
        /// URL to a helpful information
        /// it could be a suggested fix or other things such as A11y guidance.
        /// </summary>
        public HelpUrl HelpUrl { get; set; }

        /// <summary>
        /// Returns the bug id, null if no bug id has been associated
        /// </summary>
        public string IssueDisplayText { get; set; }

        public Uri IssueLink { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="desc"></param>
        /// <param name="source"></param>
        /// <param name="url"></param>
        /// <param name="meta"></param>
        internal RuleResult(RuleId id, string desc, string source, HelpUrl url, ScanMetaInfo meta)
        {
            this.Rule = id;
            this.Description = desc;
            this.Source = source;
            this.Messages = new List<string>();
            this.MetaInfo = meta;
            this.HelpUrl = url;
        }

        /// <summary>
        /// Constructor for Deserialization
        /// Do not use for other purpose.
        /// </summary>
        public RuleResult()
        {

        }

        /// <summary>
        /// Add Test status
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public void SetStatus(ScanStatus status, string message)
        {
            if (this.Status < status)
            {
                this.Status = status;
            }
            AddMessage(message);
        }

        /// <summary>
        /// Add Message
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.Messages.Add(message);
            }
        }
    }
}
