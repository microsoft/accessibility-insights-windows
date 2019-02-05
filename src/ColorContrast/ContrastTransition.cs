using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deque.ColorContrast
{
    internal class ColorContrastTransition
    {
        readonly DequeColor startingColor;

        private double priorColorContrast = 1;

        private DequeColor mostContrastingColor;

        /**
         * We can no longer add colors to this transition.
         */
        private Boolean isClosed = false;

        /**
         * This Transition has returned back to its starting color.
         */
        private Boolean isComplete = false;

        /**
         * A text transition will increase in contrast from its original color
         * until it has reached a maximum color. Then it will decrease in contrast.
         * These two booleans help us track that, without having to store all the colors
         * in a list.
         */
        private Boolean isMountainShaped = true;
        private Boolean isIncreasingInContrast = true;

        /**
         * It is useful to track the size of a transition. Especially for debugging purposes,
         * though if performance is an issue, closing large transitions can help significantly.
         */
        private int size = 1;

        internal ColorContrastTransition(DequeColor color)
        {
            startingColor = color;
            mostContrastingColor = color;
        }

        internal void AddColor(DequeColor color)
        {

            if (startingColor.Equals(color))
            {
                if (size > 1)
                {
                    isClosed = true;
                    isComplete = true;
                }
            }

            if (isClosed) return;

            size++;

            double contrast = color.Contrast(startingColor);

            if (contrast > priorColorContrast)
            {
                mostContrastingColor = color;

                if (!isIncreasingInContrast)
                {
                    isClosed = true;
                    isMountainShaped = false;
                }
            }
            else if (contrast < priorColorContrast)
            {
                isIncreasingInContrast = false;
            }

            priorColorContrast = contrast;
        }

        /**
         * True if the transition may be a transition involving text.
         */
        public Boolean IsPotentialForegroundBackgroundPair()
        {
            return size > 2 && isMountainShaped && isComplete && !ToColorPair().AreVisuallySimilarColors();
        }

        /**
         * Convert the starting color and most contrasting color to a ColorPair object.
         */
        public CCColorPair ToColorPair()
        {
            return new CCColorPair(startingColor, mostContrastingColor);
        }

        /**
         * True when the Transition closed because it ended with a color identical to the starting color.
         */
        internal Boolean IsStartingAndEndingColorSame()
        {
            return isComplete;
        }

        public override string ToString()
        {
            return ToColorPair().ToString();
        }
    }

}
