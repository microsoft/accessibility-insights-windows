// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Contains state data that is accessible to all conditions while running rules
    /// Useful when one condition saves information to be accessed by another condition
    /// This is primarily used for comparing relations such as
    /// children, siblings, parents, ancestors, and descendants.
    /// </summary>
    class ConditionContext
    {
        public Stack<IA11yElement> ReferenceElements { get; private set; } = new Stack<IA11yElement>();

    } // class
} // namespace
