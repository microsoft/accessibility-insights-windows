// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.AutomationTests
{
    internal static class Utilities
    {
        public static void AssertEqual(Dictionary<string, string> expected, Dictionary<string, string> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            foreach (KeyValuePair<string, string> pair in (expected))
            {
                Assert.IsTrue(actual.TryGetValue(pair.Key, out string value), "Key \"" + pair.Key + "\" does not exist!");
                Assert.AreEqual(pair.Value, value, false, "Different values for key \"" + pair.Key + "\"");
            }
        }
    }
}
