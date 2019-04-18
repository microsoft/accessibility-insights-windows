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
        /// <summary>
        /// The given element did not meet the conditions for the rule and was not evaluated.
        /// </summary>
        NotApplicable,

        /// <summary>
        /// The given element met the success criteria  of the rule evaluation.
        /// </summary>
        Pass,

        /// <summary>
        /// The given element did not meet the success criteria  of the rule evaluation.
        /// </summary>
        Error,

        /// <summary>
        /// The rule is unable to make a determination, and the element should be investigated.
        /// </summary>
        Open,

        /// <summary>
        /// The rule's description is informational and does not indicate success or failure.
        /// </summary>
        Note,

        /// <summary>
        /// The given element did not meet the success criteria  of the rule evaluation,
        /// but the cause is known to be difficult for developers to fix
        /// (as in issues caused by the platform itself)
        /// or impact to users has been determined to be low.
        /// </summary>
        Warning,

        /// <summary>
        /// An error such as an exception was encountered while evaluating the rule.
        /// </summary>
        RuleExecutionError
    }
}
