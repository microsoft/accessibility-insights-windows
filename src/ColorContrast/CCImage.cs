using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deque.ColorContrast.ColorContrastResult;

namespace Deque.ColorContrast
{
    public class CCPixel
    {
        public readonly int row;
        public readonly int column;
        public readonly DequeColor color;

        public CCPixel(DequeColor color, int row, int column)
        {
            this.row = row;
            this.column = column;
            this.color = color;
        }
    }

    public abstract class DequeImage : IEnumerable<CCPixel>
    {

        public abstract int NumColumns();

        public abstract int NumRows();

        public abstract DequeColor GetColor(int row, int column);

        /**
         * Run the Color Contrast calculation on the image.
         */
        public ColorContrastResult RunColorContrastCalculation()
        {
            var contrastResult = new ColorContrastResult();

            ColorContrastResult oldContrastResult = null;

            var colorContrastTransitions = new List<ColorContrastTransition>();

            CCPixel lastPixel = new CCPixel(DequeColor.WHITE, 0, 0);

            foreach (var pixel in GetBinaryRowSearchIterator())
            {

                if (!lastPixel.row.Equals(pixel.row))
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

                colorContrastTransitions.Add(new ColorContrastTransition(pixel.color));

                foreach (var transition in colorContrastTransitions)
                {
                    transition.AddColor(pixel.color);

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
        public IEnumerable<CCPixel> GetBinaryRowSearchIterator()
        {
            foreach (var pixel in GetRow(0, NumRows()))
            {
                yield return pixel;
            }
        }

        public IEnumerable<CCPixel> GetRow(int top, int bottom)
        {
            int middle = (bottom + top) / 2;

            if ((bottom - top) < ColorContrastConfig.MIN_SPACE_BETWEEN_SAMPLES) yield break;

            for (var i = 0; i < NumColumns(); i++)
            {
                yield return new CCPixel(GetColor(middle, i), middle, i);
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

        IEnumerator<CCPixel> IEnumerable<CCPixel>.GetEnumerator()
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
