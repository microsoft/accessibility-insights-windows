// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Axe.Windows.Core.Fingerprint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if FAKES_SUPPORTED
using Axe.Windows.Core.Fingerprint.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace Axe.Windows.CoreTests.Fingerprint
{
    [TestClass]
    public class InMemoryIssueStoreUnitTests
    {
        [TestMethod]
        [Timeout(2000)]
        public void Ctor_InitialStateHasEmptyCollection()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
            {
                Assert.IsFalse(store.Issues.Any());
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void IsEnumerable_IsTrue()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
            {
                Assert.IsTrue(store.IsEnumerable);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsUpdatable_IsTrue()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
            {
                Assert.IsTrue(store.IsUpdatable);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void AddIssue_IssueIsNull_ThrowsArgumentNullException()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
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
        [Timeout (2000)]
        public void AddIssue_IssueIsNotInList_AddsIssueAndReturnsCorrectResult()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
            {
                using (ShimsContext.Create())
                {
                    Issue issue = new ShimIssue
                    {
                        FingerprintGet = () => new StubIFingerprint(),
                    };
                    Assert.AreEqual(AddResult.ItemAdded, store.AddIssue(issue));

                    List<Issue> issueList = store.Issues.ToList();
                    Assert.AreEqual(1, issueList.Count);
                    Assert.AreSame(issue, issueList[0]);
                }
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void AddIssue_IssueIsInList_DoesNotAddIssueAndReturnsCorrectResult()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
            {
                using (ShimsContext.Create())
                {
                    IFingerprint actualFingerprint = null;
                    IFingerprint fingerprint = new StubIFingerprint
                    {
                        EqualsIFingerprint = (f) =>
                        {
                            actualFingerprint = f;
                            return true;
                        }
                    };

                    Issue issue1 = new ShimIssue
                    {
                        FingerprintGet = () => fingerprint,
                    };
                    Assert.AreEqual(AddResult.ItemAdded, store.AddIssue(issue1));
                    Assert.IsNull(actualFingerprint);

                    Issue issue2 = new ShimIssue
                    {
                        FingerprintGet = () => fingerprint,
                    };
                    Assert.AreEqual(AddResult.ItemAlreadyExists, store.AddIssue(issue2));
                    Assert.AreSame(fingerprint, actualFingerprint);
                    List<Issue> issueList = store.Issues.ToList();
                    Assert.AreEqual(1, issueList.Count);
                    Assert.AreSame(issue1, issueList[0]);
                }
            }
        }
#endif

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void TryFindIssue_IssueIsNull_ThrowsArgumentNullException()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
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
        public void TryFindIssue_FingerprintDoesNotMatch_ReturnsFalse()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
            {
                using (ShimsContext.Create())
                {
                    IFingerprint fingerprint = new StubIFingerprint();

                    Assert.IsFalse(store.TryFindIssue(fingerprint, out Issue issue));
                    Assert.IsNull(issue);
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void TryFindIssue_FingerprintDoesMatch_ReturnsTrueAndCorrectIssue()
        {
            using (InMemoryIssueStore store = new InMemoryIssueStore())
            {
                using (ShimsContext.Create())
                {
                    IFingerprint actualFingerprint = null;
                    IFingerprint fingerprint = new StubIFingerprint
                    {
                        EqualsIFingerprint = (f) =>
                        {
                            actualFingerprint = f;
                            return true;
                        },
                    };

                    Issue issue = new ShimIssue
                    {
                        FingerprintGet = () => fingerprint,
                    };

                    store.AddIssue(issue);
                    Assert.IsTrue(store.TryFindIssue(fingerprint, out Issue actualIssue));
                    Assert.AreSame(fingerprint, actualFingerprint);
                    Assert.AreSame(issue, actualIssue);
                }
            }
        }
#endif
    }
}
