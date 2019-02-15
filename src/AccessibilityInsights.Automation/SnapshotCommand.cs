// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Actions.Misc;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Desktop.Settings;
using System.Collections.Generic;
using System.Globalization;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Class to take a snapshot (via ShapshotCommand.Execute). Can only be successfully called after
    /// a successful call to StartCommand.Execute
    /// </summary>
    public static class SnapshotCommand
    {
        private class ScanResultAccumulator
        {
            public int Passed { get; private set; }
            public int Failed { get; private set; }
            public int Inconclusive { get; private set;}
            public int Unsupported { get; private set; }
            public int Total { get => Passed + Failed + Inconclusive + Unsupported; }

            public bool MayHaveErrors => (Failed > 0) || (Inconclusive > 0);

            public void AddPass() => Passed++;
            public void AddFail() => Failed++;
            public void AddInconclusive() => Inconclusive++;
            public void AddUnsupported() => Unsupported++;
        }

        /// <summary>
        /// Execute the Start command. Used by both .NET and by PowerShell entry points
        /// </summary>
        /// <param name="primaryConfig">The primary calling parameters</param>
        /// <returns>A SnapshotCommandResult that describes the result of the command</returns>
        public static SnapshotCommandResult Execute(Dictionary<string, string> primaryConfig)
        {
            return ExecutionWrapper.ExecuteCommand<SnapshotCommandResult>(() =>
            {
                CommandParameters parameters = new CommandParameters(primaryConfig,
                    AutomationSession.Instance().SessionParameters);
                LocationHelper locationHelper = new LocationHelper(parameters);
                ElementContext ec = TargetElementLocator.LocateElement(parameters);
                DataManager dataManager = DataManager.GetDefaultInstance();
                SelectAction sa = SelectAction.GetDefaultInstance();
                sa.SetCandidateElement(ec.Element);
                if (sa.Select())
                {
                    using (ElementContext ec2 = sa.POIElementContext)
                    {
                        GetDataAction.GetProcessAndUIFrameworkOfElementContext(ec2.Id);
                        if (CaptureAction.SetTestModeDataContext(ec2.Id, DataContextMode.Test, TreeViewMode.Control))
                        {
                            // send telemetry of scan results. 
                            var dc = GetDataAction.GetElementDataContext(ec2.Id);
                            dc.PublishScanResults();

                            if (dc.ElementCounter.UpperBoundExceeded)
                            {
                                throw new A11yAutomationException(string.Format(CultureInfo.InvariantCulture,
                                    DisplayStrings.ErrorTooManyElementsToSetDataContext,
                                    dc.ElementCounter.UpperBound));
                            }

                            ScanResultAccumulator accumulator = new ScanResultAccumulator();
                            AccumulateScanResults(accumulator, ec2.Element);
                            bool retainIfNoViolations = parameters.RetainIfNoViolations();

                            if (accumulator.MayHaveErrors || retainIfNoViolations)
                            {
                                ScreenShotAction.CaptureScreenShot(ec2.Id);
                                SaveAction.SaveSnapshotZip(locationHelper.GetOutputFilePath(), ec2.Id, ec2.Element.UniqueId, A11yFileMode.Test);
                                if (locationHelper.IsSarifExtension())
                                {
                                    // Generate SARIF file.
                                    SaveAction.SaveSarifFile(locationHelper.GetSarifFilePath(), ec2.Id, !locationHelper.IsAllOption());
                                }
                            }

                            string summaryMessage;

                            if (accumulator.MayHaveErrors)
                            {
                                summaryMessage = string.Format(CultureInfo.InvariantCulture, DisplayStrings.SnapshotDetailViolationsFormat, accumulator.Failed, accumulator.Inconclusive, locationHelper.OutputFile);
                            }
                            else if (retainIfNoViolations)
                            {
                                summaryMessage = string.Format(CultureInfo.InvariantCulture, DisplayStrings.SnapshotDetailNoViolationsDataRetainedFormat, locationHelper.OutputFile);
                            }
                            else
                            {
                                summaryMessage = DisplayStrings.SnapshotDetailNoViolationsDataDiscarded;
                            }

                            return new SnapshotCommandResult
                            {
                                Completed = true,
                                SummaryMessage = summaryMessage,
                                ScanResultsPassed = accumulator.Passed,
                                ScanResultsFailed = accumulator.Failed,
                                ScanResultsInconclusive = accumulator.Inconclusive,
                                ScanResultsUnsupported = accumulator.Unsupported,
                                ScanResultsTotal = accumulator.Total,
                            };
                        }
                    }
                }
                throw new A11yAutomationException(DisplayStrings.ErrorUnableToSetDataContext);
            },
            ErrorCommandResultFactory);
        }

        /// <summary>
        /// How many violations were found (starting at the target element)
        /// </summary>
        /// <param name="accumulator">Where the results will be accumulated</param>
        /// <param name="element">The root element to check</param>
        private static void AccumulateScanResults(ScanResultAccumulator accumulator, A11yElement element)
        {
            if (element.ScanResults == null || element.ScanResults.Items == null)
                throw new A11yAutomationException(DisplayStrings.ErrorMissingScanResults);

            foreach (var scanResult in element.ScanResults.Items)
            {
                // This intentionally ignores NoResult values
                switch (scanResult.Status)
                {
                    case Core.Results.ScanStatus.Fail:
                        accumulator.AddFail();
                        break;

                    case Core.Results.ScanStatus.Pass:
                        accumulator.AddPass();
                        break;

                    case Core.Results.ScanStatus.Uncertain:
                        accumulator.AddInconclusive();
                        break;

                    case Core.Results.ScanStatus.ScanNotSupported:
                        accumulator.AddUnsupported();
                        break;
                }
            }

            if (element.Children != null)
            {
                foreach (var child in element.Children)
                {
                    AccumulateScanResults(accumulator, child);
                }
            }
        }

        private static SnapshotCommandResult ErrorCommandResultFactory(string errorDetail)
        {
            return new SnapshotCommandResult
            {
                Completed = false,
                SummaryMessage = errorDetail,
            };
        }
    }
}
