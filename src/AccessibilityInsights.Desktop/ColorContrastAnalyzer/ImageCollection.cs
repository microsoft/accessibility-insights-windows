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

        private bool IsNewRow(Pixel pixel)
        {
            return pixel.Column == 0;
        }

        private bool IsEndOfRow(Pixel pixel)
        {
            return pixel.Column == (NumColumns() - 1);
        }
        /**
         * Run the Color Contrast calculation on the image.
         */
        public ColorContrastResult RunColorContrastCalculation()
        {

            ColorContrastResult result = null;

            ColorContrastRunner runner = new ColorContrastRunner();

            Color previousColor = null;

            foreach (var pixel in GetBinaryRowSearchIterator())
            {
                if (IsNewRow(pixel)) runner.OnRowBegin();

                runner.OnPixel(pixel.Color, previousColor);
                previousColor = pixel.Color;


                // If this is the end of a y, see if we can make conclusions about our color maps.

                if (IsEndOfRow(pixel))
                {
                    var newResult = runner.OnRowEnd();

                    if (result == null) result = newResult;

                    if (newResult.ConfidenceValue() == Confidence.High)
                    {
                        result = newResult;
                        break;
                    }
                    else if (newResult.ConfidenceValue() == Confidence.Mid &&
                      result.ConfidenceValue() == Confidence.Low)
                    {
                        result = newResult;
                    }
                }
            }

            return result;
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
