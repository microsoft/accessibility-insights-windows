// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;

namespace Axe.Windows.Rules
{
    public class RunResult
    {
        public RuleInfo RuleInfo{ get; set; }
        public IA11yElement element { get; set; }
        public EvaluationCode EvaluationCode { get; set; }
        public string ErrorMessage{ get; set; }
    } // class
} // namespace
