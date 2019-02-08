// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections;
using System.Collections.Generic;
using static AccessibilityInsights.Desktop.ColorContrastAnalyzer.ColorContrastResult;

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    public abstract class ImageCollection : IEnumerable<Pixel>
    {

        public abstract int NumColumns();

        public abstract int NumRows();

        public abstract Color GetColor(int row, int column);

        /**
         * Run the Color Contrast calculation on the image.
         */
        public ColorContrastResult RunColorContrastCalculation()
        {
            var contrastResult = new ColorContrastResult();

            ColorContrastResult oldContrastResult = null;

            var colorContrastTransitions = new List<ColorContrastTransition>();

            Pixel lastPixel = new Pixel(Color.WHITE, 0, 0);

            foreach (var pixel in GetBinaryRowSearchIterator())
            {

                if (!lastPixel.Row.Equals(pixel.Row))
                {
                    if (contrastResult.ConfidenceValue().Equals(Confidence.HIGH))
                    {
                        return contrastResult;
                    }

                    if (oldContrastResult == null)
                    {
                        oldContrastResult = contrastResult;
                    }

                    if (contrastResult.ConfidenceValue() > oldContrastResult.ConfidenceValue())
                    {
                        oldContrastResult = contrastResult;
                    }
                }

                colorContrastTransitions.Add(new ColorContrastTransition(pixel.Color));

                foreach (var transition in colorContrastTransitions)
                {
                    transition.AddColor(pixel.Color);

                    if (transition.IsPotentialForegroundBackgroundPair())
                    {
                        contrastResult.OnColorPair(transition.ToColorPair());
                    }
                }

                colorContrastTransitions.RemoveAll(transition => transition.IsStartingAndEndingColorSame());

                lastPixel = pixel;
            }

            return oldContrastResult;
        }

        /**
         * A special iterator, that looks at the middle of an image first, 
         * followed by recursively looking at the upper half and lower half
         * of the two pieces of image, until the given samples are some 
         * distance apart.
         */
        public IEnumerable<Pixel> GetBinaryRowSearchIterator()
        {
            foreach (var pixel in GetRow(0, NumRows()))
            {
                yield return pixel;
            }
        }

        public IEnumerable<Pixel> GetRow(int top, int bottom)
        {
            int middle = (bottom + top) / 2;

            if ((bottom - top) < ColorContrastConfig.MinSpaceBetweenSamples) yield break;

            for (var i = 0; i < NumColumns(); i++)
            {
                yield return new Pixel(GetColor(middle, i), middle, i);
            }

            foreach (var pixel in GetRow(top, middle))
            {
                yield return pixel;
            }

            foreach (var pixel in GetRow(middle, bottom))
            {
                yield return pixel;
            }
        }

        public IEnumerator<Pixel> GetEnumerator()
        {
            foreach (var pixel in GetRow(0, NumRows()))
            {
                yield return pixel;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var pixel in GetRow(0, NumRows()))
            {
                yield return pixel;
            }
        }
    }
}
