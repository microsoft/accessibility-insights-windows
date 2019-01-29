// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace AccessibilityInsights.DesktopUI.Utility
{
    /// <summary>
    /// Wrapper to store the hwind as key and (BorderLine, TextTip or Win32SnapShotButton) as Value
    /// Since it all runs in the UI thread, we don't need to use a thread safe data structure like ConcurrentDictionary
    /// </summary>
    public class ReferenceHolder<K, V>
    {
        private Dictionary<K, V> _dictionary = new Dictionary<K, V>();

        public void Add(K key, V value)
        {
            if (EqualityComparer<K>.Default.Equals(key, default(K)))
            {
                return;
            }
            _dictionary[key] = value;
        }

        public void Remove(K key)
        {
            if (EqualityComparer<K>.Default.Equals(key, default(K)))
            {
                return;
            }
            _dictionary.Remove(key);
        }

        public bool TryGet(K key, out V value)
        {
            if (EqualityComparer<K>.Default.Equals(key, default(K)))
            {
                value = default(V);
                return false;
            }
            return _dictionary.TryGetValue(key, out value);
        }
    }
}
