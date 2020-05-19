// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.TelemetryTests
{
    internal class TestRegistry
    {
        private readonly IReadOnlyList<Tuple<string, string, string>> _expectedQueries;
        public int QueriesMade { get; private set; } = 0;

        public TestRegistry(IEnumerable<Tuple<string, string, string>> expectedQueries)
        {
            _expectedQueries = new List<Tuple<string, string, string>>(expectedQueries);
        }

        public string GetStringValue(string keyName, string valueName, string defaultValue)
        {
            int index = QueriesMade++;

            if (index >= _expectedQueries.Count)
                return defaultValue;

            Assert.AreEqual(_expectedQueries[index].Item1, keyName);
            Assert.AreEqual(_expectedQueries[index].Item2, valueName);
            return _expectedQueries[index].Item3;
        }
    }
}
