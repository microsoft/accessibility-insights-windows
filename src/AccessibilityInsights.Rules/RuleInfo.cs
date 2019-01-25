// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Enums;

using static System.FormattableString;

namespace AccessibilityInsights.Rules
{
    public class RuleInfo : Attribute
    {
        public RuleId ID { get; set; }
        public string Description { get; set; }
        public string HowToFix { get; set; }
        public A11yCriteriaId Standard { get; set; }
        public int PropertyID { get; set; }
        public string Condition { get; set;  }

        public override string ToString()
        {
            return Invariant($@"ID:   {this.ID}
Description:    {this.Description}
HowToFix:    {this.HowToFix}
Condition:  {this.Condition}");
        }
    } // class
} // namespace
