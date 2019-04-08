// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Enums;

namespace Axe.Windows.Rules
{
    interface IRuleFactory
    {
        IRule CreateRule(RuleId id);
    } // interface
} // namespace
