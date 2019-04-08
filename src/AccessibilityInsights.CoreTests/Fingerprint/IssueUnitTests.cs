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
    public class IssueUnitTests
    {
        private const string IssueType = "MyIssueType";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void Ctor_FingerprintIsNull_ThrowsArgumentNullException()
        {
            try
            {
                new Issue(null, IssueType);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("fingerprint", e.ParamName);
                throw;
            }
        }

#if FAKES_SUPPORTED
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Timeout(2000)]
        public void Ctor_IssueTypeIsTrivial_ThrowsArgumentException()
        {
            using (ShimsContext.Create())
            {
                try
                {
                    new Issue(new StubIFingerprint(), string.Empty);
                }
                catch (ArgumentException e)
                {
                    Assert.AreEqual("issueType", e.ParamName);
                    throw;
                }
            }
        }

        [TestMethod]
        [Timeout (2000)]
        public void Ctor_FingerprintIsValid_SetsInitialProperties()
        {
            using (ShimsContext.Create())
            {
                IFingerprint fingerprint = new StubIFingerprint();

                Issue issue = new Issue(fingerprint, IssueType);

                Assert.AreEqual(IssueType, issue.IssueType);
                Assert.AreSame(fingerprint, issue.Fingerprint);
                Assert.IsFalse(issue.Locations.Any());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout (2000)]
        public void AddLocation_LocationIsNull_ThrowsArgumentNullException()
        {
            using (ShimsContext.Create())
            {
                IFingerprint fingerprint = new StubIFingerprint();

                Issue issue = new Issue(fingerprint, IssueType);

                try
                {
                    issue.AddLocation(null);
                }
                catch (ArgumentNullException e)
                {
                    Assert.AreEqual("location", e.ParamName);
                    throw;
                }
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void AddLocation_LocationIsNotInList_IsAddedToLocations()
        {
            using (ShimsContext.Create())
            {
                IFingerprint fingerprint = new StubIFingerprint();

                Issue issue = new Issue(fingerprint, IssueType);

                ILocation location = new StubILocation();

                Assert.AreEqual(AddResult.ItemAdded, issue.AddLocation(location));

                List<ILocation> locationList = issue.Locations.ToList();

                Assert.AreEqual(1, locationList.Count);
                Assert.AreSame(location, locationList[0]);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void AddLocation_LocationIsInList_IsNotAddedToLocations()
        {
            using (ShimsContext.Create())
            {
                IFingerprint fingerprint = new StubIFingerprint();

                Issue issue = new Issue(fingerprint, IssueType);
                ILocation actualLocation = null;
                ILocation location = new StubILocation
                {
                    EqualsILocation = (l) => { actualLocation = l; return true; }
                };

                Assert.AreEqual(AddResult.ItemAdded, issue.AddLocation(location));

                List<ILocation> locationList = issue.Locations.ToList();

                Assert.AreEqual(1, locationList.Count);
                Assert.AreSame(location, locationList[0]);
                Assert.IsNull(actualLocation);

                Assert.AreEqual(AddResult.ItemAlreadyExists, issue.AddLocation(location));
                Assert.AreEqual(1, issue.Locations.Count());
                Assert.AreSame(location, actualLocation);
            }
        }
#endif
    }
}
