// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    /// <summary>
    /// Used to push and pop context items such as elements from the per-thread context stack.
    /// This allows other conditions to reference saved values in addition to accessing just the properties of the given element in the Condition.Matches function.
    /// </summary>
    class Context
    {
        public static Condition Save(Condition c)
        {
            return new ContextCondition(c, InitializeElementContext, FinalizeElementContext);
        }

        private static void InitializeElementContext(IA11yElement e)
        {
            Condition.Context.Value.ReferenceElements.Push(e);
        }

        private static void FinalizeElementContext(IA11yElement e)
        {
            Condition.Context.Value.ReferenceElements.Pop();
        }
    } // class
} // namespace
