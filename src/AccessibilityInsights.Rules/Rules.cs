// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;

using static System.FormattableString;

namespace AccessibilityInsights.Rules
{
    public static class Rules
    {
        private static readonly RuleProvider Provider = new RuleProvider(new RuleFactory());

        // the following is rarely used because it only provides information about the rules
        // because its purpose is just to get info about the rules
        private static readonly Lazy<IReadOnlyDictionary<RuleId, RuleInfo>> AllInfo = new Lazy<IReadOnlyDictionary<RuleId, RuleInfo>>(CreateRuleInfo);
        public static IReadOnlyDictionary<RuleId, RuleInfo> All => AllInfo.Value;

        private static IReadOnlyDictionary<RuleId, RuleInfo> CreateRuleInfo()
        {
            var dictionary = new Dictionary<RuleId, RuleInfo>();
            foreach (var rule in Provider.All)
                dictionary.Add(rule.Info.ID, rule.Info);

            return dictionary;
        }

        public static RunResult RunRuleByID(RuleId id, IA11yElement element)
        {
            var rule = Provider.GetRule(id);
            if (rule == null)
            {
                return new RunResult
                {
                    EvaluationCode = EvaluationCode.RuleExecutionError,
                    ErrorMessage = Invariant($"No rule matching the ID '{id}' was found.")
                };
            }

            var retVal = Rules.RunRule(rule, element);

            return retVal;
        }

        public static IEnumerable<RunResult> RunAll(IA11yElement element)
        {
            var results = new List<RunResult>();

            foreach (var rule in Provider.All)
            {
                var result = RunRule(rule, element);
                results.Add(result);
            } // for all rules

            return results;
        }

        private static RunResult RunRule(IRule rule, IA11yElement element)
        {
            try
            {
                return RunRuleWorker(rule, element);
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsLogging())
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                return new RunResult
                {
                    RuleInfo = rule.Info,
                    EvaluationCode = EvaluationCode.RuleExecutionError,
                    ErrorMessage = ex.ToString()
                };
            } // catch
        }

        private static RunResult RunRuleWorker(IRule rule, IA11yElement element)
        {
            var result = new RunResult
            {
                RuleInfo = rule.Info,
                element = element,
                EvaluationCode = EvaluationCode.NotApplicable
            };

            if (!ShouldRun(rule.Condition, element))
                return result;

            result.EvaluationCode = rule.Evaluate(element);

            return result;
        }

        private static bool ShouldRun(Condition condition, IA11yElement element)
        {
            if (condition == null) return true;

            return condition.Matches(element);
        }
    } // class
} // namespace
