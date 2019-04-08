// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;

namespace Axe.Windows.Rules.PropertyConditions
{
    /// <summary>
    /// Contains commonly used conditions for testing against the SelectionItem pattern of IA11yElement.
    /// </summary>
    static class SelectionItemPattern
    {
        // pattern property values
        public const string IsSelectedProperty = "IsSelected";

        public static Condition Null = Condition.Create(IsNull);
        public static Condition NotNull = ~Null;
        public static Condition IsSelected = Condition.Create(GetIsSelected);

        private static bool IsNull(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var selectionPattern = e.GetPattern(PatternType.UIA_SelectionPatternId);
            return selectionPattern == null;
        }

        private static bool GetIsSelected(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var pattern = e.GetPattern(PatternType.UIA_SelectionItemPatternId);
            if (pattern == null) return false;

            return pattern.GetValue<bool>(IsSelectedProperty);
        }
    } // class
} // namespace
