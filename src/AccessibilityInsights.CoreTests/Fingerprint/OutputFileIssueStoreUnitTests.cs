// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Fingerprint;
using Axe.Windows.Core.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if FAKES_SUPPORTED
using Axe.Windows.Core.Bases.Fakes;
using Axe.Windows.Core.Fingerprint.Fakes;
using Axe.Windows.Core.Results.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace Axe.Windows.CoreTests.Fingerprint
{
    [TestClass]
    public class OutputFileIssueStoreUnitTests
    {
        private const string TestFile = @"C:\blah";

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout(2000)]
        public void Ctor_FileNameIsTrivial_ThrowsArgumentException()
        {
            try
            {
                new OutputFileIssueStore(string.Empty, Enumerable.Empty<A11yElement>());
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("fileName", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void Ctor_ElementSetIsNull_ThrowsArgumentNullException()
        {
            try
            {
                new OutputFileIssueStore(TestFile, null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("elementSet", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void Ctor_ExtractorIsNull_ThrowsArgumentNullException()
        {
            try
            {
                new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>(), null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("extractor", e.ParamName);
                throw;
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsEnumerable_IsTrue()
        {
            using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>()))
            {
                Assert.IsTrue(store.IsEnumerable);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsUpdatable_IsFalse()
        {
            using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>()))
            {
                Assert.IsFalse(store.IsUpdatable);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void AddIssue_IssueIsNull_ThrowsArgumentNullException()
        {
            using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>()))
            {
                try
                {
                    store.AddIssue(null);
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("issue", e.ParamName);
                    throw;
                }
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(2000)]
        public void AddIssue_IssueIsNotNull_ReturnsNotSupported()
        {
            using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>()))
            {
                using (ShimsContext.Create())
                {
                    Assert.AreEqual(AddResult.NotSupported, store.AddIssue(new ShimIssue()));
                }
            }
        }
#endif

        [TestMethod]
        [Timeout(2000)]
        public void Issues_NoIssuesExist_ReturnsEmptySet()
        {
            using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>()))
            {
                Assert.IsFalse(store.Issues.Any());
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(2000)]
        public void AddIssue_IssuesExist_ReturnsCorrectSet()
        {
            using (ShimsContext.Create())
            {
                IFingerprint fingerprint1 = new StubIFingerprint { EqualsIFingerprint = (f) => false };
                IFingerprint fingerprint2 = new StubIFingerprint();
                IFingerprint fingerprint3 = new StubIFingerprint();
                Issue issue1 = new ShimIssue();
                Issue issue2 = new ShimIssue();
                Issue issue3 = new ShimIssue();
                using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>(), (f, e, s) => 
                {
                    s.Add(fingerprint1, issue1);
                    s.Add(fingerprint2, issue2);
                    s.Add(fingerprint3, issue3);
                }))
                {
                    List<Issue> issueList = store.Issues.ToList();

                    // Issues are unsorted!
                    Assert.AreEqual(3, issueList.Count);
                    Assert.IsTrue(issueList.Contains(issue1));
                    Assert.IsTrue(issueList.Contains(issue2));
                    Assert.IsTrue(issueList.Contains(issue3));
                }
            }
        }
#endif

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void TryFindIssue_FingerprintIsNull_ThrowsArgumentNullException()
        {
            using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>()))
            {
                try
                {
                    store.TryFindIssue(null, out Issue issue);
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("fingerprint", e.ParamName);
                    throw;
                }
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(2000)]
        public void TryFindIssue_IssueIsNotInStore_ReturnsFalseWithNullIssue()
        {
            using (ShimsContext.Create())
            {
                IFingerprint actualFingerprint = null;
                IFingerprint expectedFingerprint = new StubIFingerprint
                {
                    EqualsIFingerprint = (f) => { actualFingerprint = f; return false; },
                };
                Issue expectedIssue = new ShimIssue();

                using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>(), (f, e, s) =>
                {
                    s.Add(expectedFingerprint, expectedIssue);
                }))
                {
                    Assert.IsNull(actualFingerprint);
                    Assert.IsFalse(store.TryFindIssue(expectedFingerprint, out Issue actualIssue));
                    Assert.AreSame(expectedFingerprint, actualFingerprint);
                    Assert.IsNull(actualIssue);
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void TryFindIssue_IssueIsInStore_ReturnsTrueWithCorrectIssue()
        {
            using (ShimsContext.Create())
            {
                IFingerprint actualFingerprint = null;
                IFingerprint expectedFingerprint = new StubIFingerprint
                {
                    EqualsIFingerprint = (f) => { actualFingerprint = f; return true; },
                };
                Issue actualIssue = null;
                Issue expectedIssue = new ShimIssue();

                using (IIssueStore store = new OutputFileIssueStore(TestFile, Enumerable.Empty<A11yElement>(), (f, e, s) =>
                {
                    s.Add(expectedFingerprint, expectedIssue);
                }))
                {
                    Assert.IsNull(actualFingerprint);
                    Assert.IsNull(actualIssue);
                    Assert.IsTrue(store.TryFindIssue(expectedFingerprint, out actualIssue));
                    Assert.AreSame(expectedFingerprint, actualFingerprint);
                    Assert.AreSame(expectedIssue, actualIssue);
                }
            }
        }
#endif

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementSetIsEmpty_LeavesStoreUnchanged()
        {
            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            Assert.IsFalse(store.Any());
            OutputFileIssueStore.ExtractIssues(TestFile, Enumerable.Empty<A11yElement>(), store);
            Assert.IsFalse(store.Any());
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainNoScanResults_LeavesStoreUnchanged()
        {
            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                int resultsGotten = 0;
                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    new ShimA11yElement
                    {
                        ScanResultsGet = () => { resultsGotten++; return null; }
                    },
                    new ShimA11yElement
                    {
                        ScanResultsGet = () => { resultsGotten++; return null; }
                    },
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);
                Assert.IsFalse(store.Any());
                Assert.AreEqual(2, resultsGotten);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainOnePass_LeavesStoreUnchanged()
        {
            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                int resultsGotten = 0;
                ScanResults scanResults = new ShimScanResults
                {
                    StatusGet = () => { resultsGotten++; return ScanStatus.Pass; }
                };
                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    new ShimA11yElement { ScanResultsGet = () => scanResults },
                    new ShimA11yElement { ScanResultsGet = () => scanResults },
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);
                Assert.IsFalse(store.Any());
                Assert.AreEqual(2, resultsGotten);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainOneNoResult_LeavesStoreUnchanged()
        {
            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                int resultsGotten = 0;
                ScanResults scanResults = new ShimScanResults
                {
                    StatusGet = () => { resultsGotten++; return ScanStatus.NoResult; }
                };
                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    new ShimA11yElement { ScanResultsGet = () => scanResults },
                    new ShimA11yElement { ScanResultsGet = () => scanResults },
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);
                Assert.IsFalse(store.Any());
                Assert.AreEqual(2, resultsGotten);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainOneNotSupported_LeavesStoreUnchanged()
        {
            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                int resultsGotten = 0;
                ScanResults scanResults = new ShimScanResults
                {
                    StatusGet = () => { resultsGotten++; return ScanStatus.ScanNotSupported; }
                };
                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    new ShimA11yElement { ScanResultsGet = () => scanResults },
                    new ShimA11yElement { ScanResultsGet = () => scanResults },
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);
                Assert.IsFalse(store.Any());
                Assert.AreEqual(2, resultsGotten);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainOneFailure_AddsOneFailure()
        {
            const ScanStatus scanStatus = ScanStatus.Fail;

            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                List<ILocation> actualLocations = new List<ILocation>();
                List<IFingerprint> actualFingerprints = new List<IFingerprint>();
                List<string> actualFileNames = new List<string>();
                List<ScanStatus> actualStatuses = new List<ScanStatus>();
                List<string> actualIssueTypes = new List<string>();
                List<RuleId> actualRuleIds = new List<RuleId>();
                List<A11yElement> actualElements = new List<A11yElement>();
                IFingerprint expectedFingerprint = new StubIFingerprint();

                A11yElement expectedElement = null;
                ILocation expectedLocation = new StubILocation();
                string expectedIssueType = "ChildUniqueNameOrType_Fail";
                RuleId expectedRuleId = RuleId.ChildUniqueNameOrType;

                Issue expectedIssue = new ShimIssue
                {
                    AddLocationILocation = (l) =>
                    {
                        actualLocations.Add(l);
                        return AddResult.ItemAdded;
                    }
                };

                ShimOutputFileIssueStore.BuildFingerprintA11yElementRuleIdScanStatus = (e, r, s) =>
                {
                    actualElements.Add(e);
                    actualRuleIds.Add(r);
                    actualStatuses.Add(s);

                    return expectedFingerprint;
                };

                ShimOutputFileIssueStore.BuildLocationA11yElementString = (e, f) =>
                {
                    actualElements.Add(e);
                    actualFileNames.Add(f);

                    return expectedLocation;
                };

                ShimOutputFileIssueStore.BuildIssueStringIFingerprint = (i, f) =>
                {
                    actualIssueTypes.Add(i);
                    actualFingerprints.Add(f);

                    return expectedIssue;
                };

                ScanResults scanResults = new ShimScanResults
                {
                    StatusGet = () => scanStatus,
                    ItemsGet = () => new List<ScanResult>
                    {
                        new ShimScanResult
                        {
                            ItemsGet = () => new List<RuleResult>
                            {
                                new ShimRuleResult
                                {
                                    StatusGet = () => scanStatus,
                                    RuleGet = () => expectedRuleId,
                                }
                            }
                        }
                    }
                };
                expectedElement = new ShimA11yElement { ScanResultsGet = () => scanResults };

                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    expectedElement,
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);

                Assert.AreEqual(1, store.Count);
                Assert.AreEqual(2, actualElements.Count);
                Assert.AreSame(expectedElement, actualElements[0]);
                Assert.AreSame(expectedElement, actualElements[1]);
                Assert.AreEqual(1, actualFileNames.Count);
                Assert.AreEqual(TestFile, actualFileNames[0]);
                Assert.AreEqual(1, actualFingerprints.Count);
                Assert.AreSame(expectedFingerprint, actualFingerprints[0]);
                Assert.AreEqual(1, actualStatuses.Count);
                Assert.AreEqual(scanStatus, actualStatuses[0]);
                Assert.AreEqual(1, actualIssueTypes.Count);
                Assert.AreEqual(expectedIssueType, actualIssueTypes[0]);
                Assert.AreEqual(1, actualLocations.Count);
                Assert.AreSame(expectedLocation, actualLocations[0]);
                Assert.AreEqual(1, actualRuleIds.Count);
                Assert.AreEqual(expectedRuleId, actualRuleIds[0]);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainOneUncertain_AddsOneUncertain()
        {
            const ScanStatus scanStatus = ScanStatus.Uncertain;

            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                List<ILocation> actualLocations = new List<ILocation>();
                List<IFingerprint> actualFingerprints = new List<IFingerprint>();
                List<string> actualFileNames = new List<string>();
                List<ScanStatus> actualStatuses = new List<ScanStatus>();
                List<string> actualIssueTypes = new List<string>();
                List<RuleId> actualRuleIds = new List<RuleId>();
                List<A11yElement> actualElements = new List<A11yElement>();
                IFingerprint expectedFingerprint = new StubIFingerprint();

                A11yElement expectedElement = null;
                ILocation expectedLocation = new StubILocation();
                string expectedIssueType = "ChildUniqueNameOrType_Uncertain";
                RuleId expectedRuleId = RuleId.ChildUniqueNameOrType;

                Issue expectedIssue = new ShimIssue
                {
                    AddLocationILocation = (l) =>
                    {
                        actualLocations.Add(l);
                        return AddResult.ItemAdded;
                    }
                };

                ShimOutputFileIssueStore.BuildFingerprintA11yElementRuleIdScanStatus = (e, r, s) =>
                {
                    actualElements.Add(e);
                    actualRuleIds.Add(r);
                    actualStatuses.Add(s);

                    return expectedFingerprint;
                };

                ShimOutputFileIssueStore.BuildLocationA11yElementString = (e, f) =>
                {
                    actualElements.Add(e);
                    actualFileNames.Add(f);

                    return expectedLocation;
                };

                ShimOutputFileIssueStore.BuildIssueStringIFingerprint = (i, f) =>
                {
                    actualIssueTypes.Add(i);
                    actualFingerprints.Add(f);

                    return expectedIssue;
                };

                ScanResults scanResults = new ShimScanResults
                {
                    StatusGet = () => scanStatus,
                    ItemsGet = () => new List<ScanResult>
                    {
                        new ShimScanResult
                        {
                            ItemsGet = () => new List<RuleResult>
                            {
                                new ShimRuleResult
                                {
                                    StatusGet = () => scanStatus,
                                    RuleGet = () => expectedRuleId,
                                }
                            }
                        }
                    }
                };
                expectedElement = new ShimA11yElement { ScanResultsGet = () => scanResults };

                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    expectedElement,
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);

                Assert.AreEqual(1, store.Count);
                Assert.AreEqual(2, actualElements.Count);
                Assert.AreSame(expectedElement, actualElements[0]);
                Assert.AreSame(expectedElement, actualElements[1]);
                Assert.AreEqual(1, actualFileNames.Count);
                Assert.AreEqual(TestFile, actualFileNames[0]);
                Assert.AreEqual(1, actualFingerprints.Count);
                Assert.AreSame(expectedFingerprint, actualFingerprints[0]);
                Assert.AreEqual(1, actualStatuses.Count);
                Assert.AreEqual(scanStatus, actualStatuses[0]);
                Assert.AreEqual(1, actualIssueTypes.Count);
                Assert.AreEqual(expectedIssueType, actualIssueTypes[0]);
                Assert.AreEqual(1, actualLocations.Count);
                Assert.AreSame(expectedLocation, actualLocations[0]);
                Assert.AreEqual(1, actualRuleIds.Count);
                Assert.AreEqual(expectedRuleId, actualRuleIds[0]);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainFailuresWithSameFingerprint_AddsOneUncertainWithTwoLocations()
        {
            const ScanStatus scanStatus = ScanStatus.Fail;

            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                List<ILocation> actualLocations = new List<ILocation>();
                List<IFingerprint> actualFingerprints = new List<IFingerprint>();
                List<string> actualFileNames = new List<string>();
                List<ScanStatus> actualStatuses = new List<ScanStatus>();
                List<string> actualIssueTypes = new List<string>();
                List<RuleId> actualRuleIds = new List<RuleId>();
                List<A11yElement> actualElements = new List<A11yElement>();

                IFingerprint expectedFingerprint = new StubIFingerprint
                {
                    EqualsIFingerprint = (f) => true,
                };
                Issue expectedIssue = new ShimIssue
                {
                    AddLocationILocation = (l) =>
                    {
                        actualLocations.Add(l);
                        return AddResult.ItemAdded;
                    }
                };

                ILocation expectedLocation1 = new StubILocation();
                ILocation expectedLocation2 = new StubILocation();
                string expectedIssueType = "ChildUniqueNameOrType_Fail";
                RuleId expectedRuleId = RuleId.ChildUniqueNameOrType;

                ShimOutputFileIssueStore.BuildFingerprintA11yElementRuleIdScanStatus = (e, r, s) =>
                {
                    actualElements.Add(e);
                    actualRuleIds.Add(r);
                    actualStatuses.Add(s);

                    return expectedFingerprint;
                };

                List<ILocation> expectedLocations = new List<ILocation> { expectedLocation1, expectedLocation2 };

                ShimOutputFileIssueStore.BuildLocationA11yElementString = (e, f) =>
                {
                    actualElements.Add(e);
                    actualFileNames.Add(f);
                    ILocation location = expectedLocations[0];
                    expectedLocations.RemoveAt(0);
                    return location;
                };

                ShimOutputFileIssueStore.BuildIssueStringIFingerprint = (i, f) =>
                {
                    actualIssueTypes.Add(i);
                    actualFingerprints.Add(f);

                    return expectedIssue;
                };

                ScanResults scanResults = new ShimScanResults
                {
                    StatusGet = () => scanStatus,
                    ItemsGet = () => new List<ScanResult>
                    {
                        new ShimScanResult
                        {
                            ItemsGet = () => new List<RuleResult>
                            {
                                new ShimRuleResult
                                {
                                    StatusGet = () => scanStatus,
                                    RuleGet = () => expectedRuleId,
                                }
                            }
                        }
                    },
                };

                A11yElement expectedElement1 = new ShimA11yElement { ScanResultsGet = () => scanResults };
                A11yElement expectedElement2 = new ShimA11yElement { ScanResultsGet = () => scanResults };

                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    expectedElement1,
                    expectedElement2,
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);

                Assert.AreEqual(0, expectedLocations.Count);
                Assert.AreEqual(1, store.Count);
                Assert.AreEqual(4, actualElements.Count);
                Assert.AreSame(expectedElement1, actualElements[0]);
                Assert.AreSame(expectedElement1, actualElements[1]);
                Assert.AreSame(expectedElement2, actualElements[2]);
                Assert.AreSame(expectedElement2, actualElements[3]);
                Assert.AreEqual(2, actualFileNames.Count);
                Assert.AreEqual(TestFile, actualFileNames[0]);
                Assert.AreEqual(TestFile, actualFileNames[1]);
                Assert.AreEqual(1, actualFingerprints.Count);
                Assert.AreSame(expectedFingerprint, actualFingerprints[0]);
                Assert.AreEqual(2, actualStatuses.Count);
                Assert.AreEqual(scanStatus, actualStatuses[0]);
                Assert.AreEqual(scanStatus, actualStatuses[1]);
                Assert.AreEqual(1, actualIssueTypes.Count);
                Assert.AreEqual(expectedIssueType, actualIssueTypes[0]);
                Assert.AreEqual(2, actualLocations.Count);
                Assert.AreSame(expectedLocation1, actualLocations[0]);
                Assert.AreSame(expectedLocation2, actualLocations[1]);
                Assert.AreEqual(2, actualRuleIds.Count);
                Assert.AreEqual(expectedRuleId, actualRuleIds[0]);
                Assert.AreEqual(expectedRuleId, actualRuleIds[1]);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void ExtractIssues_ElementsContainUncertainsWithSameFingerprint_AddsOneUncertainWithTwoLocations()
        {
            const ScanStatus scanStatus = ScanStatus.Uncertain;

            Dictionary<IFingerprint, Issue> store = new Dictionary<IFingerprint, Issue>();

            using (ShimsContext.Create())
            {
                List<ILocation> actualLocations = new List<ILocation>();
                List<IFingerprint> actualFingerprints = new List<IFingerprint>();
                List<string> actualFileNames = new List<string>();
                List<ScanStatus> actualStatuses = new List<ScanStatus>();
                List<string> actualIssueTypes = new List<string>();
                List<RuleId> actualRuleIds = new List<RuleId>();
                List<A11yElement> actualElements = new List<A11yElement>();

                IFingerprint expectedFingerprint = new StubIFingerprint
                {
                    EqualsIFingerprint = (f) => true,
                };
                Issue expectedIssue = new ShimIssue
                {
                    AddLocationILocation = (l) =>
                    {
                        actualLocations.Add(l);
                        return AddResult.ItemAdded;
                    }
                };

                ILocation expectedLocation1 = new StubILocation();
                ILocation expectedLocation2 = new StubILocation();
                string expectedIssueType = "ChildUniqueNameOrType_Uncertain";
                RuleId expectedRuleId = RuleId.ChildUniqueNameOrType;

                ShimOutputFileIssueStore.BuildFingerprintA11yElementRuleIdScanStatus = (e, r, s) =>
                {
                    actualElements.Add(e);
                    actualRuleIds.Add(r);
                    actualStatuses.Add(s);

                    return expectedFingerprint;
                };

                List<ILocation> expectedLocations = new List<ILocation> { expectedLocation1, expectedLocation2 };

                ShimOutputFileIssueStore.BuildLocationA11yElementString = (e, f) =>
                {
                    actualElements.Add(e);
                    actualFileNames.Add(f);
                    ILocation location = expectedLocations[0];
                    expectedLocations.RemoveAt(0);
                    return location;
                };

                ShimOutputFileIssueStore.BuildIssueStringIFingerprint = (i, f) =>
                {
                    actualIssueTypes.Add(i);
                    actualFingerprints.Add(f);

                    return expectedIssue;
                };

                ScanResults scanResults = new ShimScanResults
                {
                    StatusGet = () => scanStatus,
                    ItemsGet = () => new List<ScanResult>
                    {
                        new ShimScanResult
                        {
                            ItemsGet = () => new List<RuleResult>
                            {
                                new ShimRuleResult
                                {
                                    StatusGet = () => scanStatus,
                                    RuleGet = () => expectedRuleId,
                                }
                            }
                        }
                    },
                };

                A11yElement expectedElement1 = new ShimA11yElement { ScanResultsGet = () => scanResults };
                A11yElement expectedElement2 = new ShimA11yElement { ScanResultsGet = () => scanResults };

                IEnumerable<A11yElement> elements = new List<A11yElement>
                {
                    expectedElement1,
                    expectedElement2,
                };

                Assert.IsFalse(store.Any());
                OutputFileIssueStore.ExtractIssues(TestFile, elements, store);

                Assert.AreEqual(0, expectedLocations.Count);
                Assert.AreEqual(1, store.Count);
                Assert.AreEqual(4, actualElements.Count);
                Assert.AreSame(expectedElement1, actualElements[0]);
                Assert.AreSame(expectedElement1, actualElements[1]);
                Assert.AreSame(expectedElement2, actualElements[2]);
                Assert.AreSame(expectedElement2, actualElements[3]);
                Assert.AreEqual(2, actualFileNames.Count);
                Assert.AreEqual(TestFile, actualFileNames[0]);
                Assert.AreEqual(TestFile, actualFileNames[1]);
                Assert.AreEqual(1, actualFingerprints.Count);
                Assert.AreSame(expectedFingerprint, actualFingerprints[0]);
                Assert.AreEqual(2, actualStatuses.Count);
                Assert.AreEqual(scanStatus, actualStatuses[0]);
                Assert.AreEqual(scanStatus, actualStatuses[1]);
                Assert.AreEqual(1, actualIssueTypes.Count);
                Assert.AreEqual(expectedIssueType, actualIssueTypes[0]);
                Assert.AreEqual(2, actualLocations.Count);
                Assert.AreSame(expectedLocation1, actualLocations[0]);
                Assert.AreSame(expectedLocation2, actualLocations[1]);
                Assert.AreEqual(2, actualRuleIds.Count);
                Assert.AreEqual(expectedRuleId, actualRuleIds[0]);
                Assert.AreEqual(expectedRuleId, actualRuleIds[1]);
            }
        }
#endif
    }
}
