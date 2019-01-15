// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;

namespace AccessibilityInsights.Rules
{
    class PatternCondition: Condition
    {
        private readonly int PatternID = 0;

        private ValidateProperty Validate { get; set; }

        public delegate bool ValidateProperty(IA11yElement e);

        public PatternCondition(int patternID, ValidateProperty validate = null)
        {
            if (patternID == 0) throw new ArgumentException(nameof(patternID));

            this.PatternID = patternID;
            this.Validate = validate;
        }

        public override bool Matches(IA11yElement e)
        {
            if (e == null) throw new ArgumentException();

            var pattern = e.GetPattern(this.PatternID);

            return pattern != null && (this.Validate == null || Validate(e));
        }

        public override string ToString()
        {
            var patternName = PatternType.GetInstance().GetNameById(this.PatternID);
            return $"has {patternName} pattern";
        }
    } // class
} // namespace
