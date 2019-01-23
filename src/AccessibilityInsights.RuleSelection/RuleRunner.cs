// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.HelpLinks;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.Rules;

using static System.FormattableString;

namespace AccessibilityInsights.RuleSelection
{
    /// <summary>
    /// Runs rules from AccessibilityInsights.Rules and adapts the results to the RuleResults format
    /// </summary>
    public static class RuleRunner
    {
        public static void Run(A11yElement e)
        {
            try
            {
                RunUnsafe(e);
            }
            catch (System.Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        private static void RunUnsafe(A11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            if (e.ScanResults == null)
                e.ScanResults = new ScanResults();

            Run(e.ScanResults, e);
        }

        private static void Run(ScanResults results, A11yElement e)
        {
            var runResults = AccessibilityInsights.Rules.Rules.RunAll(e);
            foreach (var r in runResults)
            {
                if (r.EvaluationCode == EvaluationCode.NotApplicable) continue;

                var scanResult = ConvertRunResultToScanResult(r);
                results.AddScanResult(scanResult);
            } // for each
        }

        private static ScanResult ConvertRunResultToScanResult(RunResult runResult)
        {
            if (runResult == null) throw new ArgumentNullException(nameof(runResult));
            if (runResult.RuleInfo == null) throw new ArgumentException(nameof(runResult.RuleInfo));

            var scanResult = CreateResult(runResult.RuleInfo, runResult.element);

            var description = runResult.RuleInfo.Description;
            var ruleResult = scanResult.GetRuleResultInstance(runResult.RuleInfo.ID, description);
            ruleResult.Status = ConvertEvaluationCodeToScanStatus(runResult.EvaluationCode);
            ruleResult.AddMessage(runResult.RuleInfo.Description);

            return scanResult;
        }

        public static ScanResult CreateResult(RuleInfo info, IA11yElement e)
        {
            var guidelineInfo = ReferenceLinks.GetGuidelineInfo(info.Standard);
            var scanResult = new ScanResult(info.Description, guidelineInfo.ShortDescription, e, info.PropertyID)
            {
                HelpUrl = new HelpUrl
                {
                    Url = guidelineInfo.Url,
                    Type = UrlType.Info
                }
            };

            return scanResult;
        }

        public static ScanStatus ConvertEvaluationCodeToScanStatus(EvaluationCode evaluationCode)
        {
            switch (evaluationCode)
            {
                case EvaluationCode.RuleExecutionError:
                    return ScanStatus.ScanNotSupported;
                case EvaluationCode.Error:
                    return ScanStatus.Fail;
                case EvaluationCode.Note:
                case EvaluationCode.Open:
                case EvaluationCode.Warning:
                    return ScanStatus.Uncertain;
                case EvaluationCode.NotApplicable:
                    return ScanStatus.Pass; // to avoid error in UI. 
            } // switch

            return ScanStatus.Pass;
        }

        private static Exception Error(string message)
        {
#if DEBUG
            var callStack = new System.Diagnostics.StackFrame(1, true);
            return new Exception(Invariant($"{message} in {callStack.GetMethod()} at line {callStack.GetFileLineNumber()} in {callStack.GetFileName()}"));
#else
            return new Exception(message);
#endif
        }
    } // class
} // namespace
