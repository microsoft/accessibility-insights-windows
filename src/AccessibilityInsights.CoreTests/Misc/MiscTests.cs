// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Results;
using Axe.Windows.Core.Misc;
using Axe.Windows.UnitTestSharedLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Utility = Axe.Windows.UnitTestSharedLibrary.Utility;

namespace Axe.Windows.CoreTests.Misc
{
    /// <summary>
    /// Tests for the misc folder in Axe.Windows.Core
    /// </summary>
    [TestClass()]
    public class MiscTests
    {
        /// <summary>
        /// Tests whether the aggregates status counts of a monster
        /// snapshot are accurate
        /// </summary>
        [TestMethod()]
        public void GetStatusCounts()
        {
            A11yElement ke = UnitTestSharedLibrary.Utility.LoadA11yElementsFromJSON("Snapshots/Taskbar.snapshot");
            ke.ScanResults.Items.ForEach(item => {
                item.Items = new System.Collections.Generic.List<RuleResult>();
                RuleResult r = new RuleResult();
                r.Status = ScanStatus.Pass;
                item.Items.Add(r);
            });
            ke.Children.ForEach(c => {
                Utility.PopulateChildrenTests(c);
            });
            var statuses = (from child in ke.Children
                           select child.TestStatus);
            int[] statusCounts = statuses.GetStatusCounts();
            Assert.AreEqual(3, statusCounts[(int) ScanStatus.Fail]);
            Assert.AreEqual(2, statusCounts[(int) ScanStatus.Pass]);
            Assert.AreEqual(0, statusCounts[(int) ScanStatus.Uncertain]);
            Assert.AreEqual(0, statusCounts[(int) ScanStatus.NoResult]);
        }
    }
}
