// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.RulesTest;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.RulesTest
{
    public class CCAControlTypesFilter
    {
        private static CCAControlTypesFilter DefaultInstance;
        private HashSet<int> MainTypes = new HashSet<int>();

        /// <summary>
        /// GetDefaultInstance
        /// </summary>
        /// <returns></returns>
        public static CCAControlTypesFilter GetDefaultInstance()
        {
            if (DefaultInstance == null)
            {
                DefaultInstance = new CCAControlTypesFilter();
                DefaultInstance.Filter();
            }

            return DefaultInstance;
        }

        public void Filter() {
            var iterator = ControlType.All.Difference(ControlType.Custom).GetEnumerator();

            do
            {
                MainTypes.Add(iterator.Current);
            } while (iterator.MoveNext());
        }

        public bool Contains(int typeId)
        {
            return MainTypes.Contains(typeId);
        }
    }
}
