// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Contains the result of the evaluation by a rule of a specific element.
    /// </summary>
    public class RunResult
    {
        /// <summary>
        /// Metadata about the rule that was run.
        /// </summary>
        public RuleInfo RuleInfo{ get; set; }

        /// <summary>
        /// The element on which the rule was run.
        /// </summary>
        public IA11yElement element { get; set; }

        /// <summary>
        /// The result of the evaluation by the rule.
        /// </summary>
        public EvaluationCode EvaluationCode { get; set; }

        /// <summary>
        /// If <see cref="EvaluationCode"/> contains the value <see cref="EvaluationCode.RuleExecutionError"/>,
        /// this property may contain a description of the error.
        /// </summary>
        public string ErrorMessage{ get; set; }
    } // class
} // namespace
