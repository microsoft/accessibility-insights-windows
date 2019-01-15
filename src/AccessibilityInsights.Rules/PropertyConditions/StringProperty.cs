// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text.RegularExpressions;
using AccessibilityInsights.Core.Bases;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    class StringProperty
    {
        public Condition Null;
        public Condition NotNull;
        public Condition Empty;
        public Condition NotEmpty;
        public Condition NullOrEmpty;
        public Condition NotNullOrEmpty;
        public Condition WhiteSpace;
        public Condition NotWhiteSpace;
        public Condition IncludesSpecialCharacters;
        public Condition ExcludesSpecialCharacters;
        public ValueCondition<int> Length;
        private Func<IA11yElement, string> GetStringPropertyValue;

        // special character RegEx
        private static string SpecialCharacters = "\\s*[\u0387\u16EB\u2022\u2024\u2027\u2219\u22C5\u2E31\u2E33\u30FB\uA78F\uE946]+\\s*";
        private static Regex SpecialCharacterRegEx = new Regex(SpecialCharacters, RegexOptions.IgnoreCase);

        public StringProperty(Func<IA11yElement, string> valueGetter)
        {
            GetStringPropertyValue = valueGetter;
            this.Null = CreateNullCondition();
            this.NotNull = ~Null;
            this.Empty = CreateEmptyCondition();
            this.NotEmpty = ~Empty;
            this.NullOrEmpty = Null | Empty;
            this.NotNullOrEmpty = NotNull & NotEmpty;
            this.WhiteSpace = CreateWhitespaceCondition();
            this.NotWhiteSpace = ~WhiteSpace;
            this.IncludesSpecialCharacters = CreateIncludesSpecialCharactersCondition();
            this.ExcludesSpecialCharacters = ~IncludesSpecialCharacters;
            this.Length = CreateLengthCondition();
        }

        private Condition CreateNullCondition()
        {
            var condition = Condition.Create(e => GetStringPropertyValue(e) == null);
            return condition;
        }

        private Condition CreateEmptyCondition()
        {
            var condition = Condition.Create(e => GetStringPropertyValue(e)?.Length <= 0);
            return condition;
        }

        private Condition CreateWhitespaceCondition()
        {
            var condition = Condition.Create(e => GetStringPropertyValue(e)?.Trim().Length <= 0);
            return condition;
        }

        private Condition CreateIncludesSpecialCharactersCondition()
        {
            var condition = Condition.Create(e =>
            {
                var matches = SpecialCharacterRegEx.Matches(GetStringPropertyValue(e));
                return matches.Count > 0;
            });

            return condition;
        }

        private ValueCondition<int> CreateLengthCondition()
        {
            return new ValueCondition<int>(e =>
            {
                if (e == null) return 0;

                string s = GetStringPropertyValue(e);
                if (s == null) return 0;

                return s.Length;
            }, string.Empty);
        }

        public Condition Is(string s)
        {
            return Condition.Create(e => GetStringPropertyValue(e) == s);
        }

        public Condition IsNoCase(string s)
        {
            return Condition.Create(e => string.Equals(GetStringPropertyValue(e), s, StringComparison.OrdinalIgnoreCase));
        }

        public Condition IsEqualTo(StringProperty that)
        {
            if (that == null) throw new ArgumentNullException(nameof(that));

            return Condition.Create(e => this.IsEqualTo(e, that));
        }

        public Condition IsNotEqualTo(StringProperty that)
        {
            return ~IsEqualTo(that);
        }

        private bool IsEqualTo(IA11yElement e, StringProperty that)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (that == null) throw new ArgumentNullException(nameof(that));

            string s1 = this.GetStringPropertyValue(e);
            if (String.IsNullOrWhiteSpace(s1)) return false;

            string s2 = that.GetStringPropertyValue(e);
            if (String.IsNullOrWhiteSpace(s2)) return false;

            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    } // class
} // namespace
