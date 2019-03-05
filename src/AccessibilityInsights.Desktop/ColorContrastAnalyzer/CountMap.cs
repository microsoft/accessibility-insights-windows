using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    class CountMap<T> : Dictionary<T, int>
    {        
        public void Increment(T item)
        {
            if (!ContainsKey(item))
            {
                this[item] = 1;
            }
            else
            {
                this[item] = this[item] + 1;
            }
        }

        public void Increment(T item, int value)
        {
            if (!ContainsKey(item))
            {
                this[item] = value;
            }
            else
            {
                this[item] = this[item] + value;
            }
        }

        public int GetValue(T item)
        {
            if (ContainsKey(item))
            {
                return this[item];
            }
            else
            {
                return 0;
            }
        }

        public T EntryWithGreatestValue()
        {
            int max = int.MinValue;
            T maxKey = default(T);

            foreach (var entry in this)
            {
                if (max < entry.Value)
                {
                    max = entry.Value;
                    maxKey = entry.Key;
                }
            }

            return maxKey;
        }
    }
}
