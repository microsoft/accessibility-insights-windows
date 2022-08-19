// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Utilities
{
    /// <summary>
    /// Wrapper to store the hwnd as key and (BorderLine, TextTip or Win32SnapShotButton) as Value
    /// Since it all runs in the UI thread, we don't need to use a thread safe data structure like ConcurrentDictionary
    /// </summary>
    public class ReferenceHolder<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            if (EqualityComparer<TKey>.Default.Equals(key, default(TKey)))
            {
                return;
            }
            _dictionary[key] = value;
        }

        public void Remove(TKey key)
        {
            if (EqualityComparer<TKey>.Default.Equals(key, default(TKey)))
            {
                return;
            }
            _dictionary.Remove(key);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (EqualityComparer<TKey>.Default.Equals(key, default(TKey)))
            {
                value = default(TValue);
                return false;
            }
            return _dictionary.TryGetValue(key, out value);
        }
    }
}
