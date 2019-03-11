// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.SharedUx.ViewModels;
using AccessibilityInsights.UnitTestSharedLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            A11yElement ke = Utility.LoadA11yElementsFromJSON("Snapshots/Taskbar.snapshot");
            Utility.PopulateChildrenTests(ke);
            HierarchyNodeViewModel model = new HierarchyNodeViewModel(ke, true, false);
            Assert.AreEqual(23, model.AggregateStatusCounts[(int)ScanStatus.Pass]);
        }
    }
}
