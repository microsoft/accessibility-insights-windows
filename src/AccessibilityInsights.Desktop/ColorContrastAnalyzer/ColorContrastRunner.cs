// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    /**
    * Manages running of the color contrast calculation. Collects transition and
    * color counts and attempts to come to conclusions based on these values.
    */
    internal class ColorContrastRunner
    {
        private Dictionary<Color, ColorContrastTransition> openTransitions = new Dictionary<Color, ColorContrastTransition>();

        private CountMap<Color> countExactColors = new CountMap<Color>();

        private CountMap<ColorPair> countExactPairs = new CountMap<ColorPair>();

        internal void OnPixel(Color color, Color previousColor)
        {
            countExactColors.Increment(color);

            var newlyClosedTransitions = new List<ColorContrastTransition>();

            foreach (var transition in openTransitions.Values)
            {
                transition.AddColor(color);

                if (transition.IsClosed)
                {
                    newlyClosedTransitions.Add(transition);

                    if (transition.IsPotentialForegroundBackgroundPair())
                    {
                        countExactPairs.Increment(new ColorPair(
                            transition.StartingColor,
                            transition.MostContrastingColor
                        ));
                    }
                }
            }

            foreach (ColorContrastTransition transition in newlyClosedTransitions)
            {
                openTransitions.Remove(transition.StartingColor);
            }

            if (previousColor != null && !color.Equals(previousColor))
            {
                openTransitions[previousColor] = new ColorContrastTransition(previousColor);
            }
        }

        internal void OnRowBegin()
        {
            openTransitions.Clear();
            countExactPairs.Clear();
            countExactColors.Clear();
        }

        private Color backgroundColorByCount()
        {
            return countExactColors.EntryWithGreatestValue();
        }

        // Returns true when entries have lead to a confident conclusion about Text and Background color.

        internal ColorContrastResult OnRowEnd()
        {
            ColorContrastResult result = new ColorContrastResult();

            CountMap<ColorPair> pairsWithSimilarTextColor = new CountMap<ColorPair>();

            foreach (var exactPairOuter in countExactPairs)
            {
                foreach (var exactPairInner in countExactPairs)
                {
                    if (exactPairOuter.Key.backgroundColor.Equals(exactPairInner.Key.backgroundColor))
                    {
                        if (exactPairOuter.Key.foregroundColor.IsSimilarColor(exactPairInner.Key.foregroundColor))
                        {
                            pairsWithSimilarTextColor.Increment(exactPairOuter.Key, exactPairInner.Value);
                        }
                    }
                }
            }

            var sortedByValueAndContrast = pairsWithSimilarTextColor.OrderByDescending(x => x.Value)
                .ThenByDescending(x => x.Key.ColorContrast());

            if (sortedByValueAndContrast.Count() <= 0) return result;

            var resultPairs = new HashSet<ColorPair>();

            var firstEntryCount = sortedByValueAndContrast.First().Value;

            if (firstEntryCount < ColorContrastConfig.MinNumberColorTransitions) return result;

            var firstEntryCountAdjusted = firstEntryCount / ColorContrastConfig.TransitionCountDominanceFactor;

            foreach (var entry in sortedByValueAndContrast)
            {
                // Only Collect Pairs that have a reasonable occurence count.
                if (entry.Value < firstEntryCountAdjusted) break;

                resultPairs.Add(entry.Key);
            }

            foreach (var colorPair in resultPairs)
            {
                result.Add(colorPair);
            }

            countExactColors.Clear();

            openTransitions.Clear();

            return result;
        }
    }
}
