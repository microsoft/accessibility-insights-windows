// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Enums;

using static System.FormattableString;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Provides metadata about a rule.
    /// </summary>
    public class RuleInfo : Attribute
    {
        /// <summary>
        /// Contains a unique identifier for the rule from the RuleId enumeration.
        /// </summary>
        public RuleId ID { get; set; }

        /// <summary>
        /// Contains a short description of the rule.
        /// This is typically displayed in a list of rule results after a scan.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Detailed information on how to resolve a violation reported by the rule.
        /// </summary>
        public string HowToFix { get; set; }

        /// <summary>
        /// Indicates the standards documentation from which the rule was derived.
        /// </summary>
        public A11yCriteriaId Standard { get; set; }

        /// <summary>
        /// In cases where the rule tests one specific UI Automation property,
        /// this contains the UI Automation property ID in question.
        /// This property is used to link elements with rule violations to relevant documentation.
        /// </summary>
        public int PropertyID { get; set; }

        /// <summary>
        /// A description of the conditions under which a rule will be evaluated.
        /// This information is generated programatically when a rule inherits the Rule base class
        /// and is not meant to be changed.
        /// </summary>
        public string Condition { get; set;  }

        /// <summary>
        /// Provides a string summary of the information contained in the RuleInfo object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Invariant($@"ID:   {this.ID}
Description:    {this.Description}
HowToFix:    {this.HowToFix}
Condition:  {this.Condition}");
        }
    } // class
} // namespace
