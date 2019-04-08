// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class LandmarksTest
    {
        [TestMethod]
        public void TestMainLandmarkTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = LandmarkType.UIA_MainLandmarkTypeId;

                Assert.IsTrue(Landmarks.Main.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestMainLandmarkFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = 0;

                Assert.IsFalse(Landmarks.Main.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestCustomLandmarkTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = LandmarkType.UIA_CustomLandmarkTypeId;

                Assert.IsTrue(Landmarks.Custom.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestCustomLandmarkFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = 0;

                Assert.IsFalse(Landmarks.Custom.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestFormLandmarkTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = LandmarkType.UIA_FormLandmarkTypeId;

                Assert.IsTrue(Landmarks.Form.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestFormLandmarkFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = 0;

                Assert.IsFalse(Landmarks.Form.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNavigationLandmarkTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = LandmarkType.UIA_NavigationLandmarkTypeId;

                Assert.IsTrue(Landmarks.Navigation.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestNavigationLandmarkFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = 0;

                Assert.IsFalse(Landmarks.Navigation.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestSearchLandmarkTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = LandmarkType.UIA_SearchLandmarkTypeId;

                Assert.IsTrue(Landmarks.Search.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestSearchLandmarkFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.LandmarkType = 0;

                Assert.IsFalse(Landmarks.Search.Matches(e));
            } // using
        }
    } // class
} // namespace
