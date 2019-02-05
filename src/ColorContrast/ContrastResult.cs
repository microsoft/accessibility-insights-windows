using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deque.ColorContrast
{
    public class ColorContrastResult
    {
        private Dictionary<CCColorPair, int> countsOfSimilarPairs = new Dictionary<CCColorPair, int>();

        private Dictionary<CCColorPair, int> countsOfExactPairs = new Dictionary<CCColorPair, int>();

        private Dictionary<DequeColor, int> countsOfExactColors = new Dictionary<DequeColor, int>();

        public enum Confidence { LOW, MID, HIGH }

        /**
         * Returns the most occurent color pair with the highest color contrast.
         */
        public CCColorPair GetMostLikelyColorPair()
        {
            var orderedPairs = countsOfSimilarPairs.OrderByDescending(x => x.Value);

            return orderedPairs.First().Key;
        }

        /**
         * How confident we are that this result found the actual (or Visibly Similar)
         * Text and Background color combination. 
         * 
         * Note: A value of HIGH can be depended on for a Rule.
         */ 
        public Confidence ConfidenceValue()
        {

            if (countsOfSimilarPairs.Count == 0)
            {
                return Confidence.LOW;
            }

            var orderedByCountThenContrast = countsOfSimilarPairs
                .OrderByDescending(x => x.Value)
                .ThenByDescending(x => x.Key.ColorContrast());

            int lastCount = 0;
            CCColorPair lastColorPair = null;

            if (orderedByCountThenContrast.First().Value >= ColorContrastConfig.MIN_NUMBER_COLOR_TRANSITIONS)
            {

                foreach (var entry in orderedByCountThenContrast)
                {
                    var colorPair = entry.Key;

                    if (lastColorPair != null)
                    {
                        if (lastColorPair.IsVisiblySimilarTo(colorPair))
                        {
                            continue;
                        }
                        else
                        {
                            if (lastCount > entry.Value * ColorContrastConfig.TEXT_COLOR_PAIR_DOMINANCE_VALUE)
                            {
                                return Confidence.HIGH;
                            }
                        }
                    }

                    lastColorPair = entry.Key;
                    lastCount = entry.Value;
                }

                return Confidence.MID;
            }

            return Confidence.LOW;
        }


        internal void Clear()
        {
            countsOfSimilarPairs.Clear();
            countsOfExactPairs.Clear();
            countsOfExactColors.Clear();
        }

        internal void OnColorPair(CCColorPair newColorPair)
        {

            if (!countsOfSimilarPairs.ContainsKey(newColorPair))
            {
                countsOfSimilarPairs[newColorPair] = 1;

                foreach (var colorPairEntry in countsOfSimilarPairs)
                {
                    if (colorPairEntry.Key.IsVisiblySimilarTo(newColorPair))
                    {
                        countsOfSimilarPairs[newColorPair] = colorPairEntry.Value;

                        //All similar colors are kept in sync, so we only need ony match.
                        break;
                    }
                }
            }

            var similarPairs = new List<CCColorPair>(countsOfSimilarPairs.Keys.Where(x => x.IsVisiblySimilarTo(newColorPair)));

            foreach (var colorPair in similarPairs)
            {
                countsOfSimilarPairs[colorPair] = countsOfSimilarPairs[colorPair] + 1;
            }

            if (!countsOfExactPairs.ContainsKey(newColorPair))
            {
                countsOfExactPairs[newColorPair] = 1;
            }
            else
            {
                countsOfExactPairs[newColorPair] = countsOfExactPairs[newColorPair] + 1;
            }
        }

        internal void OnColor(DequeColor color)
        {

            if (!countsOfExactColors.ContainsKey(color))
            {
                countsOfExactColors[color] = 1;
            }
            else
            {
                countsOfExactColors[color] = countsOfExactColors[color] + 1;
            }
        }

    }
}
