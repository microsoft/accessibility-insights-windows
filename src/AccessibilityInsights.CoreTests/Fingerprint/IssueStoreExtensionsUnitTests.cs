// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using AccessibilityInsights.Core.Fingerprint;
using AccessibilityInsights.Core.Fingerprint.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.CoreTests.Fingerprint
{
    [TestClass]
    public class IssueStoreExtensionsUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void MergeIssuesFromStore_TargetIsNull_ThrowsArgumentNullException()
        {
            using (ShimsContext.Create())
            {
                using (IIssueStore issueStore = new StubIIssueStore())
                {
                    try
                    {
                        IssueStoreExtensions.MergeIssuesFromStore(null, issueStore);
                    }
                    catch (ArgumentNullException e)
                    {
                        Assert.AreEqual("targetStore", e.ParamName);
                        throw;
                    }
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void MergeIssuesFromStore_SourceIsNull_ThrowsArgumentNullException()
        {
            using (ShimsContext.Create())
            {
                using (IIssueStore targetStore = new StubIIssueStore())
                {
                    try
                    {
                        targetStore.MergeIssuesFromStore(null);
                    }
                    catch (ArgumentNullException e)
                    {
                        Assert.AreEqual("sourceStore", e.ParamName);
                        throw;
                    }
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Timeout(2000)]
        public void MergeIssuesFromStore_SourceIsNotEnumerable_ThrowsInvalidOperationException()
        {
            using (ShimsContext.Create())
            {
                using (IIssueStore sourceStore = new StubIIssueStore
                {
                    IsEnumerableGet = () => false,
                })
                using (IIssueStore targetStore = new StubIIssueStore())
                {
                    targetStore.MergeIssuesFromStore(sourceStore);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Timeout(2000)]
        public void MergeIssuesFromStore_TargetIsNotUpdatable_ThrowsInvalidOperationException()
        {
            using (ShimsContext.Create())
            {
                using (IIssueStore sourceStore = new StubIIssueStore
                {
                    IsEnumerableGet = () => true,
                })
                using (IIssueStore targetStore = new StubIIssueStore
                {
                    IsUpdatableGet = () => false,
                })
                {
                    targetStore.MergeIssuesFromStore(sourceStore);
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void MergeIssuesFromStore_SourceStoreHasNoIssues_ReturnsZero()
        {
            using (ShimsContext.Create())
            {
                using (IIssueStore sourceStore = new StubIIssueStore
                {
                    IsEnumerableGet = () => true,
                    IssuesGet = () => Enumerable.Empty<Issue>(),
                })
                using (IIssueStore targetStore = new StubIIssueStore
                {
                    IsUpdatableGet = () => true,
                })
                {
                    Assert.AreEqual(0, targetStore.MergeIssuesFromStore(sourceStore));
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void MergeIssuesFromStore_SourceIssueIsNotInTarget_AddsIssueToTarget_AddFails_ReturnsZero()
        {
            using (ShimsContext.Create())
            {
                IFingerprint actualFingerprint = null;
                Issue actualIssue = null;
                IFingerprint expectedFingerprint = new StubIFingerprint();
                Issue expectedIssue = new ShimIssue
                {
                    FingerprintGet = () => expectedFingerprint,
                };

                using (IIssueStore sourceStore = new StubIIssueStore
                {
                    IsEnumerableGet = () => true,
                    IssuesGet = () => new List<Issue> { expectedIssue },
                })
                using (IIssueStore targetStore = new StubIIssueStore
                {
                    IsUpdatableGet = () => true,
                    TryFindIssueIFingerprintIssueOut = (IFingerprint f, out Issue i) =>
                    {
                        actualFingerprint = f;
                        i = null;
                        return false;
                    },
                    AddIssueIssue = (i) =>
                    {
                        actualIssue = i;
                        return AddResult.ItemAlreadyExists;
                    },
                })
                {
                    Assert.IsNull(actualFingerprint);
                    Assert.IsNull(actualIssue);
                    Assert.AreEqual(0, targetStore.MergeIssuesFromStore(sourceStore));
                    Assert.AreSame(expectedFingerprint, actualFingerprint);
                    Assert.AreSame(expectedIssue, actualIssue);
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void MergeIssuesFromStore_SourceIssueIsNotInTarget_AddsIssueToTarget_AddSucceeds_ReturnsOne()
        {
            using (ShimsContext.Create())
            {
                IFingerprint actualFingerprint = null;
                Issue actualIssue = null;
                IFingerprint expectedFingerprint = new StubIFingerprint();
                Issue expectedIssue = new ShimIssue
                {
                    FingerprintGet = () => expectedFingerprint,
                };

                using (IIssueStore sourceStore = new StubIIssueStore
                {
                    IsEnumerableGet = () => true,
                    IssuesGet = () => new List<Issue> { expectedIssue },
                })
                using (IIssueStore targetStore = new StubIIssueStore
                {
                    IsUpdatableGet = () => true,
                    TryFindIssueIFingerprintIssueOut = (IFingerprint f, out Issue i) =>
                    {
                        actualFingerprint = f;
                        i = null;
                        return false;
                    },
                    AddIssueIssue = (i) =>
                    {
                        actualIssue = i;
                        return AddResult.ItemAdded;
                    },
                })
                {
                    Assert.IsNull(actualFingerprint);
                    Assert.IsNull(actualIssue);
                    Assert.AreEqual(1, targetStore.MergeIssuesFromStore(sourceStore));
                    Assert.AreSame(expectedFingerprint, actualFingerprint);
                    Assert.AreSame(expectedIssue, actualIssue);
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void MergeIssuesFromStore_SourceIssueIsInTarget_AddsLocationToIssue_AddFails_ReturnsZero()
        {
            using (ShimsContext.Create())
            {
                IFingerprint actualFingerprint = null;
                ILocation actualLocation = null;
                IFingerprint expectedFingerprint = new StubIFingerprint();
                ILocation expectedLocation = new StubILocation();
                Issue expectedIssue = new ShimIssue
                {
                    FingerprintGet = () => expectedFingerprint,
                    LocationsGet = () => new List<ILocation> { expectedLocation },
                    AddLocationILocation = (l) =>
                    {
                        actualLocation = l;
                        return AddResult.NotSupported;
                    },
                };

                using (IIssueStore sourceStore = new StubIIssueStore
                {
                    IsEnumerableGet = () => true,
                    IssuesGet = () => new List<Issue> { expectedIssue },
                })
                using (IIssueStore targetStore = new StubIIssueStore
                {
                    IsUpdatableGet = () => true,
                    TryFindIssueIFingerprintIssueOut = (IFingerprint f, out Issue i) =>
                    {
                        actualFingerprint = f;
                        i = expectedIssue;
                        return true;
                    },
                })
                {
                    Assert.IsNull(actualFingerprint);
                    Assert.IsNull(actualLocation);
                    Assert.AreEqual(0, targetStore.MergeIssuesFromStore(sourceStore));
                    Assert.AreSame(expectedFingerprint, actualFingerprint);
                    Assert.AreSame(expectedLocation, actualLocation);
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void MergeIssuesFromStore_SourceIssueIsInTarget_AddsLocationToIssue_AddSucceeds_ReturnsOne()
        {
            using (ShimsContext.Create())
            {
                IFingerprint actualFingerprint = null;
                ILocation actualLocation = null;
                IFingerprint expectedFingerprint = new StubIFingerprint();
                ILocation expectedLocation = new StubILocation();
                Issue expectedIssue = new ShimIssue
                {
                    FingerprintGet = () => expectedFingerprint,
                    LocationsGet = () => new List<ILocation> { expectedLocation },
                    AddLocationILocation = (l) =>
                    {
                        actualLocation = l;
                        return AddResult.ExistingItemUpdated;
                    },
                };

                using (IIssueStore sourceStore = new StubIIssueStore
                {
                    IsEnumerableGet = () => true,
                    IssuesGet = () => new List<Issue> { expectedIssue },
                })
                using (IIssueStore targetStore = new StubIIssueStore
                {
                    IsUpdatableGet = () => true,
                    TryFindIssueIFingerprintIssueOut = (IFingerprint f, out Issue i) =>
                    {
                        actualFingerprint = f;
                        i = expectedIssue;
                        return true;
                    },
                })
                {
                    Assert.IsNull(actualFingerprint);
                    Assert.IsNull(actualLocation);
                    Assert.AreEqual(1, targetStore.MergeIssuesFromStore(sourceStore));
                    Assert.AreSame(expectedFingerprint, actualFingerprint);
                    Assert.AreSame(expectedLocation, actualLocation);
                }
            }
        }
    }
}
