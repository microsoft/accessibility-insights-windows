using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    class ColorContrastRunner
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

                if (transition.IsClosed())
                {
                    newlyClosedTransitions.Add(transition);

                    if (transition.IsConsequential())
                    {
                        countExactPairs.Increment(new ColorPair(
                            transition.StartingColor(),
                            transition.MostContrastingColor()
                        ));
                    }
                }
            }
            
            if (previousColor != null && !color.Equals(previousColor))
            {
                openTransitions[previousColor] = new ColorContrastTransition(previousColor);
            }

            foreach (ColorContrastTransition transition in newlyClosedTransitions)
            {
                openTransitions.Remove(transition.StartingColor());
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
                    if (exactPairOuter.Key.LighterColor.Equals(exactPairInner.Key.LighterColor))
                    {
                        if (exactPairOuter.Key.DarkerColor.IsSimilarColor(exactPairInner.Key.DarkerColor))
                        {
                            pairsWithSimilarTextColor.Increment(exactPairOuter.Key, exactPairInner.Value);
                        }
                    }

                    if (exactPairOuter.Key.DarkerColor.Equals(exactPairInner.Key.DarkerColor))
                    {
                        if (exactPairOuter.Key.LighterColor.IsSimilarColor(exactPairInner.Key.LighterColor))
                        {
                            pairsWithSimilarTextColor.Increment(exactPairOuter.Key, exactPairInner.Value);
                        }
                    }
                }
            }

            var sortedByValueAndContrast = pairsWithSimilarTextColor.OrderBy(x => x.Value)
                .ThenBy(x => x.Key.ColorContrast());

            if (sortedByValueAndContrast.Count() <= 0) return result;

            var resultPairs = new HashSet<ColorPair>();

            var firstEntryCountAdjusted = sortedByValueAndContrast.First().Value /
                ColorContrastConfig.TransitionCountDominanceFactor;

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
