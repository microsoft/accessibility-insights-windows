// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace AccessibilityInsights.SharedUxTests.Tests
{
    [TestClass()]
    public class LayoutTests
    {
        /// <summary>
        /// Ensures GetLayoutLive populates a Layout with proper default values.
        /// </summary>
        [TestMethod()]
        public void GetLayoutLiveTest()
        {
            MainWindowLayout la = MainWindowLayout.GetLayoutLive();

            Assert.AreEqual(460, la.Width);
            Assert.AreEqual(500, la.Height);
            Assert.AreEqual(410, la.ColumnSnapWidth);
            Assert.AreEqual(0, la.RowTestHeight);
            Assert.AreEqual(46 * 7, la.RowPropertiesHeight);
            Assert.AreEqual(WindowState.Normal, la.WinState);

        }

        /// <summary>
        /// Ensures GetLayoutSnapshot populates a Layout with proper default values.
        /// </summary>
        [TestMethod()]
        public void GetLayoutSnapshotTest()
        {
            MainWindowLayout la = MainWindowLayout.GetLayoutSnapshot();

            Assert.AreEqual(870, la.Width);
            Assert.AreEqual(720, la.Height);
            Assert.AreEqual(410, la.ColumnSnapWidth);
            Assert.AreEqual(260, la.RowTestHeight);
            Assert.AreEqual(46 * 7, la.RowPropertiesHeight);
            Assert.AreEqual(WindowState.Normal, la.WinState);

        }
    }
}
