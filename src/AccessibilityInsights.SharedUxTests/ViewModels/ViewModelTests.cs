// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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
            A11yElement e = LoadA11yElementsFromJSON("./Snapshots/Taskbar.snapshot");
            PopulateChildrenTests(e);
            HierarchyNodeViewModel model = new HierarchyNodeViewModel(e, true, false);
            Assert.AreEqual(23, model.AggregateStatusCounts[(int)ScanStatus.Pass]);
        }

        /// <summary>
        /// Load the UI Automation elements hierarchy tree from JSON file.
        /// it returns the root UI Automation element from the tree.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static A11yElement LoadA11yElementsFromJSON(string path)
        {
            A11yElement element = null;
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                element = JsonConvert.DeserializeObject<A11yElement>(json);
                if (element == null)
                {
                    return null;
                }
                // Set parents
                Queue<A11yElement> elements = new Queue<A11yElement>();
                elements.Enqueue(element);
                while (elements.Count > 0)
                {
                    var next = elements.Dequeue();
                    if (next.Children != null)
                    {
                        foreach (var c in next.Children)
                        {
                            c.Parent = next;
                            elements.Enqueue(c);
                        };
                    }
                }
            }

            return element;
        }

        /// <summary>
        /// Populates all descendents with test results and sets them to
        ///     pass if the control is a button (any predicate would work)
        ///     and returns number that should pass
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static void PopulateChildrenTests(A11yElement e)
        {
            foreach (var item in e.ScanResults.Items)
            {
                item.Items = new List<RuleResult>();
                RuleResult r = new RuleResult();
                r.Status = e.ControlTypeId == Axe.Windows.Core.Types.ControlType.UIA_ButtonControlTypeId ? ScanStatus.Pass : ScanStatus.Fail;
                item.Items.Add(r);
            };
            foreach (var c in e.Children)
            {
                PopulateChildrenTests(c);
            };
        }
    }
}
