// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    public class ColorContrastResult
    {
        private Dictionary<ColorPair, int> countsOfSimilarPairs = new Dictionary<ColorPair, int>();

        private Dictionary<ColorPair, int> countsOfExactPairs = new Dictionary<ColorPair, int>();

        private Dictionary<Color, int> countsOfExactColors = new Dictionary<Color, int>();

        public enum Confidence { Low, Mid, High }

        /**
         * Returns the most occurent color pair with the highest color contrast.
         */
        public ColorPair GetMostLikelyColorPair()
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
                return Confidence.Low;
            }

            var orderedByCountThenContrast = countsOfSimilarPairs
                .OrderByDescending(x => x.Value)
                .ThenByDescending(x => x.Key.ColorContrast());

            int lastCount = 0;
            ColorPair lastColorPair = null;

            if (orderedByCountThenContrast.First().Value >= ColorContrastConfig.MinNumberColorTransitions)
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
                            if (lastCount > entry.Value * ColorContrastConfig.TextColorPairDominanceValue)
                            {
                                return Confidence.High;
                            }
                        }
                    }

                    lastColorPair = entry.Key;
                    lastCount = entry.Value;
                }

                return Confidence.Mid;
            }

            return Confidence.Low;
        }


        internal void Clear()
        {
            countsOfSimilarPairs.Clear();
            countsOfExactPairs.Clear();
            countsOfExactColors.Clear();
        }

        internal void OnColorPair(ColorPair newColorPair)
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

            var similarPairs = new List<ColorPair>(countsOfSimilarPairs.Keys.Where(x => x.IsVisiblySimilarTo(newColorPair)));

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

        internal void OnColor(Color color)
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
