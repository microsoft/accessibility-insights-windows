// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;

namespace AccessibilityInsights.CoreTests.Bases
{
    /// <summary>
    /// Tests A11yPattern class
    /// </summary>
    [TestClass()]
    public class A11yPatternTests
    {
        /// <summary>
        /// Test ToString and constructor for A11yPattern
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            A11yElement ke = A11yElementTests.FromJson("Resources/A11yPatternTest.hier");
       
            Assert.AreEqual("SelectionPattern: False", ke.Patterns[0].ToString());
            Assert.AreEqual("ScrollPattern: False", ke.Patterns[1].ToString());
            Assert.AreEqual("ExpandCollapsePattern: 0", ke.Patterns[2].ToString());
            Assert.AreEqual("ItemContainerPattern: ", ke.Patterns[3].ToString());
            Assert.AreEqual("SynchronizedInputPattern: ", ke.Patterns[4].ToString());
        }
    }
}
