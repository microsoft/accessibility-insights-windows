// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Results;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AccessibilityInsights.UnitTestSharedLibrary
{
    /// <summary>
    /// Class to contain shared utility methods for Unit tests
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Deserialize saved A11yElement from a JSON file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static A11yElement LoadA11yElementsFromJSON(string path)
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
                        next.Children.ForEach(c => {
                            c.Parent = next;
                            elements.Enqueue(c);
                        });
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
        /// <param name="ke"></param>
        /// <returns></returns>
        public static void PopulateChildrenTests(A11yElement ke)
        {
            ke.ScanResults.Items.ForEach(item => {
                item.Items = new List<RuleResult>();
                RuleResult r = new RuleResult();
                r.Status = ke.ControlTypeId == AccessibilityInsights.Core.Types.ControlType.UIA_ButtonControlTypeId ? ScanStatus.Pass : ScanStatus.Fail;
                item.Items.Add(r);
            });
            ke.Children.ForEach(c => {
                PopulateChildrenTests(c);
            });
        }
    }
}
