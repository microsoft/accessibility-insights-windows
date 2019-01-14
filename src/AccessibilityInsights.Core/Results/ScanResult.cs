// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.HelpLinks;
using AccessibilityInsights.Core.Misc;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Core.Results
{
    /// <summary>
    /// Class for an individual scan result
    /// it contains multiple instances of "ScanResult" 
    /// </summary>
    public class ScanResult
    {
        /// <summary>
        /// Aggregate Status
        /// </summary>
        public ScanStatus Status
        {
            get
            {
                ScanStatus status = ScanStatus.NoResult;

                if (this.Items != null)
                {
                    var tss = from i in Items
                              select i.Status;

                    status = tss.GetAggregatedScanStatus();
                }

                return status;
            }
        }

        /// <summary>
        /// Name of the Rule
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Source of the rule (ex, A11yCriteria 4.1.2)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// URL to a helpful information
        /// it could be a suggested fix or other things such as A11y guidance.
        /// </summary>
        public HelpUrl HelpUrl { get; set; }

        /// <summary>
        /// Keep the Meta Info Scan Result
        /// </summary>
        public ScanMetaInfo MetaInfo { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// Rule Results
        /// </summary>
        public List<RuleResult> Items { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Constructor for Scan result
        /// </summary>
        /// <param name="description"></param>
        /// <param name="e">A11yElement</param>
        /// <param name="propertyId"> if the value is 0, it means that it is not using property for scan</param>
        /// <param name="status"></param>
        public ScanResult(string description, string source, IA11yElement e, int propertyId = 0)
        {
            this.Description = description;
            this.Source = source;
            this.MetaInfo = propertyId != 0 ? new ScanMetaInfo(e, propertyId) : new ScanMetaInfo(e);
            this.Items = new List<RuleResult>();
        }

        /// <summary>
        /// Constructor for Deserialization
        /// Do not use for other purpose.
        /// </summary>
        public ScanResult()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public RuleResult GetRuleResultInstance(RuleId id, string desc)
        {
            var rr = new RuleResult(id, desc, this.Source, this.HelpUrl, this.MetaInfo.Clone());

            this.Items.Add(rr);

            return rr;
        }
    }
}
