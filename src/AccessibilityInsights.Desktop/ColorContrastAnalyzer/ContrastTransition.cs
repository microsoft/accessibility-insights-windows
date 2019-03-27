// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    internal class ColorContrastTransition
    {

        internal Boolean IsClosed { get; private set; } = false;
        internal Boolean IsConnecting { get; private set; } = false;

        internal readonly Color StartingColor;

        internal Color MostContrastingColor { get; private set; }

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

        internal ColorContrastTransition(Color color)
        {
            StartingColor = color;
            MostContrastingColor = color;
        }

        internal void AddColor(Color color)
        {
            size++;

            if (size > ColorContrastConfig.MaxTextThickness || (size > 1 && StartingColor.Equals(color)))
            {

                if (StartingColor.Equals(color))
                {
                    IsConnecting = true;
                }

                IsClosed = true;

                return;
            }

            if (MostContrastingColor.Contrast(StartingColor) < color.Contrast(StartingColor))
            {
                MostContrastingColor = color;

                if (!isIncreasingInContrast)
                {
                    isMountainShaped = false;
                }
            }
            else
            {
                isIncreasingInContrast = false;
            }
        }

        /**
         * True if the transition may be a transition involving text.
         */
        public Boolean IsPotentialForegroundBackgroundPair()
        {
            return IsConsequential() && !ToColorPair().AreVisuallySimilarColors();
        }

        /**
         * Convert the starting color and most contrasting color to a ColorPair object.
         */
        public ColorPair ToColorPair()
        {
            return new ColorPair(StartingColor, MostContrastingColor);
        }

        internal Boolean IsConsequential()
        {
            return IsConnecting && size > 2 && isMountainShaped;
        }

        public override string ToString()
        {
            return ToColorPair().ToString();
        }
    }

}
