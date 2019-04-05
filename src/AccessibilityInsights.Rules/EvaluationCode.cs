// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Axe.Windows.Rules
{
    /// <summary>
    /// Enumeration representing the results of a rule evaluation
    /// Reflects the SARIF 2.0 Result.Level property
    /// </summary>
    public enum EvaluationCode
    {
        NotApplicable, // given element did not meet the conditions for the rule
        Pass,
        Error, // fail
        Open, // not enough information to make a determination
        Note, // informational
        Warning,
        RuleExecutionError
    }
}
