using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.DesktopUI.Utility
{
    public class ReferenceHolder<K, V>
    {
        private Dictionary<K, V> _dictionary = new Dictionary<K, V>();

        public void Add(K key, V value)
        {
            _dictionary[key] = value;
        }

        public void Remove(K key)
        {
            _dictionary.Remove(key);
        }

        public bool TryGet(K key, out V value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
    }
}
