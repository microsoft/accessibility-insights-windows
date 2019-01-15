// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;

using static System.FormattableString;

namespace AccessibilityInsights.Core.Fingerprint
{
    /// <summary>
    /// Production implementation of IFingerprint
    /// </summary>
    internal class ScanResultFingerprint : IFingerprint
    {
        private int? _hashValue;

        public IReadOnlyList<FingerprintContribution> Contributions { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="element">The element being fingerprinted</param>
        /// <param name="ruleId">The RuleIds value associated with this rule</param>
        /// <param name="status">The status of this rule for this element</param>
        public ScanResultFingerprint(A11yElement element, RuleId ruleId, ScanStatus status)
        {
            element.ArgumentIsNotNull(nameof(element));

            List<FingerprintContribution> contributions = new List<FingerprintContribution>
            {
                new FingerprintContribution("RuleId", ruleId.ToString()),
                new FingerprintContribution("Level", status.ToResultLevelString()),
            };

            AddRuleSpecificContributions(contributions, ruleId, element);

            A11yElement parent = element.Parent;
            A11yElement grandparent = parent?.Parent;
            int ancestorLevel = 0;
            while (true)
            {
                bool ignoreNameProperty = (ancestorLevel > 0) && (grandparent == null);

                AddRuleAgnosticContributions(contributions, element, ancestorLevel++, ignoreNameProperty);

                if (grandparent == null)
                    break;

                element = parent;
                parent = grandparent;
                grandparent = parent?.Parent;
            }

            // Sort the list to help with comparisons
            contributions.Sort();
            Contributions = contributions;
        }

        /// <summary>
        /// Add contributions for rule-specific properties
        /// </summary>
        /// <param name="contributions">The set of contributions to update</param>
        /// <param name="ruleId">determines which properties to add</param>
        /// <param name="element">Element under consideration</param>
        internal static void AddRuleSpecificContributions(IList<FingerprintContribution> contributions, RuleId ruleId, A11yElement element)
        {
            switch (ruleId)
            {
                case RuleId.IsKeyboardFocusable:
                case RuleId.IsKeyboardFocusableBasedOnPatterns:
                    contributions.Add(new FingerprintContribution("IsKeyboardFocusable", element.IsKeyboardFocusable.ToString(CultureInfo.InvariantCulture)));
                    break;

                case RuleId.IsControlElementPropertyCorrect:
                    contributions.Add(new FingerprintContribution("IsControlElement", element.IsControlElement.ToString(CultureInfo.InvariantCulture)));
                    break;

                case RuleId.IsContentElementPropertyCorrect:
                    contributions.Add(new FingerprintContribution("IsContentElement", element.IsContentElement.ToString(CultureInfo.InvariantCulture)));
                    break;
            }
        }

        /// <summary>
        /// Add the rule-agnostic fingerprint contributions for this element
        /// </summary>
        /// <param name="contributions">The set of contributions to update</param>
        /// <param name="element">Element under consideration</param>
        /// <param name="levelBaseName">The "base" to use when specifying contribution keys</param>
        /// <param name="ignoreNameProperty">If true, exclude the "Name" property from contributions</param>
        private static void AddRuleAgnosticContributions(IList<FingerprintContribution> contributions, A11yElement element, int level,
            bool ignoreNameProperty)
        {
            string levelBaseName = (level > 0) ?
                Invariant($"Ancestor{level}.") :
                string.Empty;

            AddValidContribution(contributions, levelBaseName, "AcceleratorKey", element.AcceleratorKey);
            AddValidContribution(contributions, levelBaseName, "AccessKey", element.AccessKey);
            AddValidContribution(contributions, levelBaseName, "AutomationId", element.AutomationId);
            AddValidContribution(contributions, levelBaseName, "ClassName", element.GetClassName());
            AddValidContribution(contributions, levelBaseName, "ControlType", ControlType.GetInstance().GetNameById(element.ControlTypeId));
            AddValidContribution(contributions, levelBaseName, "FrameworkId", element.GetUIFramework());
            AddValidContribution(contributions, levelBaseName, "LocalizedControlType", element.LocalizedControlType);
            AddValidContribution(contributions, levelBaseName, "Name", ignoreNameProperty ? null : element.Name);
        }

        private static void AddValidContribution(IList<FingerprintContribution> contributions, string levelBaseName, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            contributions.Add(new FingerprintContribution(levelBaseName + key, value));
        }

        /// <summary>
        /// Get the hash code for this object
        /// </summary>
        /// <returns>The computed hash code</returns>
        public override int GetHashCode()
        {
            EnsureHashCode();
            return _hashValue.Value;
        }

        /// <summary>
        /// Ensures that we have a computed hash code
        /// </summary>
        private void EnsureHashCode()
        {
            if (_hashValue.HasValue)
                return;

            unchecked
            {
                int contributionCount = 0;
                // Adapted from https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
                int hash = 17;
                foreach (FingerprintContribution contribution in Contributions)
                {
                    hash = hash * 31 + contribution.GetHashCode();
                    contributionCount++;
                }

                _hashValue = hash;
            }
        }

        /// <summary>
        /// Implementation of IComparable
        /// </summary>
        /// <param name="other">The other object to consider for comparison</param>
        /// <returns>-1: this is before other
        ///           0: this is in same position as other
        ///           1: this is after other</returns>
        public int CompareTo(IFingerprint other)
        {
            other.ArgumentIsNotNull(nameof(other));

            ScanResultFingerprint typedOther = other as ScanResultFingerprint;

            if (typedOther == null)
                throw new InvalidOperationException("This operation is only supported on ScanResultFingerprint objects");

            return TypedCompareTo(typedOther);
        }

        /// <summary>
        /// Strongly typed version of CompareTo. Used by both CompareTo and Equals
        /// </summary>
        /// <param name="other">The other object to consider for comparison</param>
        /// <returns>-1: this is before other
        ///           0: this is in same position as other
        ///           1: this is after other</returns>
        private int TypedCompareTo(ScanResultFingerprint other)
        {
            int hash = GetHashCode();
            int otherHash = other.GetHashCode();

            if (Contributions.Count == other.Contributions.Count)
                return hash.CompareTo(otherHash);

            return Contributions.Count.CompareTo(other.Contributions.Count);
        }

        /// <summary>
        /// Implementation of IEquatable
        /// </summary>
        /// <param name="other">The other IFingerprint object to compare against</param>
        /// <returns>true if 2 objects are fully equivalent</returns>
        public bool Equals(IFingerprint other)
        {
            if (other == null)
                return false;

            // Our check only works for ScanResultFingerprint objects
            ScanResultFingerprint typedOther = other as ScanResultFingerprint;
            if (typedOther == null)
                return false;

            if (TypedCompareTo(typedOther) != 0)
                return false;

            // Do the expensive check last!
            for (int loop = 0; loop < Contributions.Count; loop++)
            {
                if (!Contributions[loop].Equals(other.Contributions[loop]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
