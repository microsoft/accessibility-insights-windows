// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Automation;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
#if FAKES_SUPPORTED
using Axe.Windows.Actions.Contexts.Fakes;
using Axe.Windows.Actions.Fakes;
using Axe.Windows.Automation.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace Axe.Windows.AutomationTests
{
    [TestClass]
    public class SnapshotCommandUnitTests
    {
        const string TestMessage = "I find his reaction most illogical";

        static readonly Dictionary<string, string> TestParameters = new Dictionary<string, string>()
        {
            {"NCC-1700", "USS Constitution" },
            {"NCC-1701", "USS Enterprise" },
            {"NCC-1864", "USS Reliant" },
        };

        static readonly Dictionary<string, string> TestParametersNoRetention = new Dictionary<string, string>()
        {
            {"NoViolationPolicy",   "Discard" },
            {"NCC-1700",            "USS Constitution" },
            {"NCC-1701",            "USS Enterprise" },
            {"NCC-1864",            "USS Reliant" },
        };

        private static void AssertIncompleteResult(SnapshotCommandResult result, string expectedString, bool matchExactString = true)
        {
            Assert.AreEqual(false, result.Completed);
            Assert.AreEqual(0, result.ScanResultsFailedCount, "Mismatch in count of failures");
            Assert.AreEqual(0, result.ScanResultsInconclusiveCount, "Mismatch in count of inconclusives");
            Assert.AreEqual(0, result.ScanResultsPassedCount, "Mismatch in count of passes");
            Assert.AreEqual(0, result.ScanResultsUnsupportedCount, "Mismatch in count of unsupporteds");
            Assert.AreEqual(0, result.ScanResultsTotalCount);
            if (matchExactString)
            {
                Assert.AreEqual(expectedString, result.SummaryMessage);
            }
            else
            {
                Assert.IsTrue(result.SummaryMessage.Contains(expectedString), "\"" + result.SummaryMessage+ "\" doesn't contain \"" + expectedString + "\"");
            }
        }

        private static void AssertCompleteResult(SnapshotCommandResult result, int expectedPass, int expectedFail, int expectedInconclusive, int expectedUnsupported)
        {
            Assert.AreEqual(true, result.Completed);
            Assert.AreEqual(expectedFail, result.ScanResultsFailedCount, "Mismatch in count of failures");
            Assert.AreEqual(expectedInconclusive, result.ScanResultsInconclusiveCount, "Mismatch in count of inconclusives");
            Assert.AreEqual(expectedPass, result.ScanResultsPassedCount, "Mismatch in count of passes");
            Assert.AreEqual(expectedUnsupported, result.ScanResultsUnsupportedCount, "Mismatch in count of unsupporteds");
            Assert.AreEqual(expectedPass + expectedFail + expectedInconclusive + expectedUnsupported, result.ScanResultsTotalCount);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.SummaryMessage), "SummaryMessage can't be trivial");
        }

        private static A11yElement CreateA11yElement(List<A11yElement> children = null, List<ScanStatus> statuses = null)
        {
            A11yElement element = new A11yElement();

            if (children != null)
                element.Children = children;

            if (statuses != null)
            {
                ScanResult scanResult = new ScanResult
                {
                    Items = new List<RuleResult>()
                };

                foreach (ScanStatus status in statuses)
                {
                    scanResult.Items.Add(new RuleResult {Status = status});
                }

                ScanResults elementScanResults = new ScanResults();
                elementScanResults.AddScanResult(scanResult);

                element.ScanResults = elementScanResults;
            }

            return element;
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(2000)]
        public void Execute_AutomationSessionInstanceThrowsAutomationException_ReturnsIncomplete_MessageMatchesException()
        {
            using (ShimsContext.Create())
            {
                int callsToInstance = 0;

                ShimAutomationSession.Instance = () =>
                {
                    callsToInstance++;
                    throw new A11yAutomationException(TestMessage);
                };

                InitializeShims();

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                Assert.AreEqual(1, callsToInstance);
                AssertIncompleteResult(result, TestMessage);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_TargetElementLocatorReceivesExpectedParameters()
        {
            using (ShimsContext.Create())
            {
                CommandParameters actualParameters = null;
                CommandParameters expectedParameters = new CommandParameters(TestParameters, string.Empty);

                ShimAutomationSession.Instance = () =>
                {
                    return new ShimAutomationSession()
                    {
                        SessionParametersGet = () => expectedParameters
                    };
                };

                ShimTargetElementLocator.LocateElementCommandParameters = (commandParameters) =>
                {
                    actualParameters = commandParameters;
                    throw new Exception(TestMessage);
                };

                InitializeShims(populateLocationHelper: false);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                Utilities.AssertEqual(expectedParameters.ConfigCopy, actualParameters.ConfigCopy);
                AssertIncompleteResult(result, TestMessage, false);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_UnableToSelectCandidateElement_ReturnsIncomplete_ErrorAutomation008()
        {
            using (ShimsContext.Create())
            {
                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => false,
                };

                InitializeShims(selectAction:selectAction, populateLocationHelper: false, enableRetention: true, shimTargetElementLocator: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertIncompleteResult(result, " Automation008:", false);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_UnableToSetTestModeDataContext_ReturnsIncomplete_ErrorAutomation008()
        {
            using (ShimsContext.Create())
            {
                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(CreateA11yElement()),
                };

                InitializeShims(populateLocationHelper: false, enableRetention: true, shimTargetElementLocator: true,
                    shimUiFramework: true, setTestModeSucceeds: false);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertIncompleteResult(result, " Automation008:", false);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_NoScanResultsInPOI_ReturnsIncomplete_ErrorAutomation012()
        {
            using (ShimsContext.Create())
            {
                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(CreateA11yElement()),
                };

                InitializeShims(populateLocationHelper: false, enableRetention: true, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertIncompleteResult(result, " Automation012:", false);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_UpperBoundIsReached_ReturnsIncomplete_ErrorAutomation017()
        {
            using (ShimsContext.Create())
            {
                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement= (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        new A11yElement()),
                };

                InitializeShims(populateLocationHelper: false, enableRetention: false, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: true, shimUiFramework: true,
                    setTestModeSucceeds: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertIncompleteResult(result, " Automation017:", false);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_NoScanResultsInPOI_RetentionIsDisabled_ReturnsCompleteWithNoIssues()
        {
            using (ShimsContext.Create())
            {
                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        new A11yElement
                        {
                            ScanResults = new ScanResults(),
                            Children = new List<A11yElement>
                            {
                                new A11yElement
                                {
                                    ScanResults = new ScanResults(),
                                    Children = new List<A11yElement>(),
                                }
                            }
                        }),
                };

                InitializeShims(populateLocationHelper: false, enableRetention: false, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertCompleteResult(result, 0, 0, 0, 0);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_NoScanResultsInPOI_RetentionIsEnabled_SavesFile_ReturnsCompleteWithNoIssues()
        {
            using (ShimsContext.Create())
            {
                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        new A11yElement
                        {
                            ScanResults = new ScanResults(),
                            Children = new List<A11yElement>
                            {
                                new A11yElement
                                {
                                    ScanResults = new ScanResults(),
                                    Children = new List<A11yElement>(),
                                }
                            }
                        }),
                };

                InitializeShims(populateLocationHelper: true, enableRetention: true, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true, shimScreenCapture: true, shimSnapshot: true, shimSarif: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertCompleteResult(result, 0, 0, 0, 0);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_OnlyPassesInPOI_RetentionIsEnabled_SavesFile_ReturnsComplete_PassesOnly()
        {
            using (ShimsContext.Create())
            {
                List<ScanStatus> scanStatusPass = new List<ScanStatus> { ScanStatus.Pass };

                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        CreateA11yElement(
                            new List<A11yElement>
                            {
                                CreateA11yElement(new List<A11yElement>(), scanStatusPass)
                            },
                            scanStatusPass))
                };

                InitializeShims(populateLocationHelper: true, enableRetention: true, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true, shimScreenCapture: true, shimSnapshot: true, shimSarif: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertCompleteResult(result, 2, 0, 0, 0);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_OnlyInconclusiveInPOI_RetentionIsEnabled_SavesFile_ReturnsComplete_InconclusivesOnly()
        {
            using (ShimsContext.Create())
            {
                List<ScanStatus> scanScatusUncertain = new List<ScanStatus> { ScanStatus.Uncertain };

                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        CreateA11yElement(
                            new List<A11yElement>
                            {
                                CreateA11yElement(new List<A11yElement>(), scanScatusUncertain)
                            },
                            scanScatusUncertain))
                };

                InitializeShims(populateLocationHelper: true, enableRetention: true, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true, shimScreenCapture: true, shimSnapshot: true, shimSarif: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertCompleteResult(result, 0, 0, 2, 0);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_OnlyFailuresInPOI_RetentionIsEnabled_SavesFile_ReturnsComplete_FailuresOnly()
        {
            using (ShimsContext.Create())
            {
                List<ScanStatus> scanStatusFail = new List<ScanStatus> { ScanStatus.Fail };

                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        CreateA11yElement(
                            new List<A11yElement>
                            {
                                CreateA11yElement(new List<A11yElement>(), scanStatusFail)
                            },
                            scanStatusFail))
                };

                InitializeShims(populateLocationHelper: true, enableRetention: true, shimTargetElementLocator: true,
                    selectAction:selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true, shimScreenCapture: true, shimSnapshot: true, shimSarif: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertCompleteResult(result, 0, 2, 0, 0);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_OnlyUnsupportedsInPOI_RetentionIsEnabled_SavesFile_ReturnsComplete_UnsupportedsOnly()
        {
            using (ShimsContext.Create())
            {
                List<ScanStatus> scanStatusUnsupported = new List<ScanStatus> { ScanStatus.ScanNotSupported };

                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        CreateA11yElement(
                            new List<A11yElement>
                            {
                                CreateA11yElement(new List<A11yElement>(), scanStatusUnsupported)
                            },
                            scanStatusUnsupported))
                };

                InitializeShims(populateLocationHelper: true, enableRetention: true, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true, shimScreenCapture: true, shimSnapshot: true, shimSarif: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                AssertCompleteResult(result, 0, 0, 0, 2);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void Execute_MixedResultsInPOI_RetentionIsEnabled_SavesFile_ReturnsComplete_CorrectMixedResults()
        {
            using (ShimsContext.Create())
            {
                ShimSelectAction selectAction = new ShimSelectAction()
                {
                    SetCandidateElementA11yElement = (element) => { },
                    Select = () => true,
                    POIElementContextGet = () => new ElementContext(
                        CreateA11yElement(
                            new List<A11yElement>
                            {
                                CreateA11yElement(new List<A11yElement>(), new List<ScanStatus>
                                {
                                    ScanStatus.Fail, ScanStatus.Pass   // Will count as failure
                                }),
                                CreateA11yElement(new List<A11yElement>(), new List<ScanStatus>
                                {
                                    ScanStatus.Pass  // Will count as pass
                                }),
                                CreateA11yElement(new List<A11yElement>(), new List<ScanStatus>
                                {
                                    ScanStatus.Uncertain, ScanStatus.Uncertain, ScanStatus.ScanNotSupported  // Will count as unsupported
                                }),
                                CreateA11yElement(new List<A11yElement>(), new List<ScanStatus>
                                {
                                    ScanStatus.Pass, ScanStatus.Uncertain, ScanStatus.NoResult  // Will count as uncertain
                                }),
                                CreateA11yElement(new List<A11yElement>(), new List<ScanStatus>
                                {
                                    ScanStatus.Uncertain, ScanStatus.NoResult  // Will count as uncertain
                                }),
                            },
                            new List<ScanStatus>
                            {
                                ScanStatus.Pass  // Will count as pass
                            }))
                };

                InitializeShims(populateLocationHelper: true, enableRetention: true, shimTargetElementLocator: true,
                    selectAction: selectAction, elementBoundExceeded: false, shimUiFramework: true,
                    setTestModeSucceeds: true, shimScreenCapture: true, shimSnapshot: true, shimSarif: true);

                SnapshotCommandResult result = SnapshotCommand.Execute(new Dictionary<string, string>());

                // Note: Results are for each A11yElement, not for each ScanStatus!
                AssertCompleteResult(result, 2, 1, 2, 1);
            }
        }

        private static void InitializeShims(
            bool? populateLocationHelper = null,
            bool? enableRetention = null,
            bool shimTargetElementLocator = false,
            bool? elementBoundExceeded = null,
            bool shimUiFramework = false,
            bool? setTestModeSucceeds = null,
            bool shimScreenCapture = false,
            bool shimSnapshot = false,
            bool shimSarif = false,
            ShimSelectAction selectAction = null)
        {
            if (enableRetention.HasValue)
            {
                ShimAutomationSession.Instance = () =>
                {
                    return new ShimAutomationSession()
                    {
                        SessionParametersGet = () => new CommandParameters(enableRetention.Value ? TestParameters : TestParametersNoRetention, string.Empty)
                    };
                };
            }

            if (shimTargetElementLocator)
            {
                ShimTargetElementLocator.LocateElementCommandParameters = (commandParameters) =>
                {
                    return new ElementContext(CreateA11yElement());
                };
            }

            if (populateLocationHelper.HasValue)
            {
                ShimLocationHelper.ConstructorCommandParameters = (locationHelper, parameters) => CreateShimLocationHelper(locationHelper, populateLocationHelper.Value);
            }

            if (selectAction != null)
            {
                ShimSelectAction.GetDefaultInstance = () => selectAction;
            }

            if (elementBoundExceeded.HasValue)
            {
                SetElementDataContext(elementBoundExceeded.Value);
            }

            if (shimUiFramework)
            {
                ShimGetDataAction.GetProcessAndUIFrameworkOfElementContextGuid = (_) => new Tuple<string, string>("one", "two");
            }

            if (setTestModeSucceeds.HasValue)
            {
                ShimCaptureAction.SetTestModeDataContextGuidDataContextModeTreeViewModeBoolean = (_, __, ___, _____) 
                    => setTestModeSucceeds.Value;
            }

            if (shimScreenCapture)
            {
                ShimScreenShotAction.CaptureScreenShotGuid = (_) => { };
            }

            if (shimSnapshot)
            {
                ShimSaveAction.SaveSnapshotZipStringGuidNullableOfInt32A11yFileModeDictionaryOfSnapshotMetaPropertyNameObjectCompletenessMode = (_, __, ___, ____, _____, ______) => { };
            }

            if (shimSarif)
            {
                ShimSaveAction.SaveSarifFileStringGuidBoolean = (_, __, ___) => { };
            }
        }

        private static void CreateShimLocationHelper(LocationHelper locationHelper, bool populateData)
        {
            ShimLocationHelper shim = new ShimLocationHelper(locationHelper);

            if (populateData)
            {
                shim.OutputPathGet = () => "path";
                shim.OutputFileFormatGet = () => "format";
                shim.OutputFileGet = () => "file";
                shim.GetOutputFilePath = () => "a11y";
                shim.GetSarifFilePath = () => "sarif";
                shim.IsAllOption = () => true;
            }
        }

        private static void SetElementDataContext(bool upperBoundExceeded)
        {
            BoundedCounter counter = new BoundedCounter(1);
            counter.TryIncrement();

            if (upperBoundExceeded)  // if true, we're 1 over capacity. if false, we're exactly at capacity
            {
                counter.TryIncrement();
            }

            ShimElementDataContext dc = new ShimElementDataContext
            {
                ElementCounterGet = () => counter
            };

            ShimGetDataAction.GetElementDataContextGuid = (_) => dc;
        }
#endif
    }
}
