// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.SharedUx.ViewModels;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.CoreTests.Bases;

namespace AccessibilityInsights.SharedUxTests.ViewModels
{
    [TestClass]
    public class ViewModelTests
    {
        /// <summary>
        /// Tests the descendent status counts in HierarchyNodeViewModel
        /// </summary>
        [TestMethod()]
        public void GetDescendentStatusCounts()
        {
            A11yElement ke = A11yElementTests.FromJson("Resources/Taskbar.snapshot");
            AccessibilityInsights.CoreTests.Misc.MiscTests.PopulateChildrenTests(ke);
            HierarchyNodeViewModel model = new HierarchyNodeViewModel(ke, true, false);
            Assert.AreEqual(23, model.AggregateStatusCounts[(int)ScanStatus.Pass]);
        }
    }
}
