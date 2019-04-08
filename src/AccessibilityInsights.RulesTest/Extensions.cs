// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axe.Windows.RulesTest
{
    static class Extensions
    {
        public static IEnumerable<int> Difference(this IEnumerable<int> allItems, params int[] itemsToRemove)
        {
            return allItems.Difference((IEnumerable<int>)itemsToRemove);
        }

        public static IEnumerable<int> Difference(this IEnumerable<int> allItems, IEnumerable<int> itemsToRemove)
        {
            if (allItems == null) throw new ArgumentException(nameof(allItems));
            if (itemsToRemove == null) throw new ArgumentException(nameof(itemsToRemove));

            return from item in allItems
                   where !itemsToRemove.Contains(item)
                   select item;
        }
    }
}
