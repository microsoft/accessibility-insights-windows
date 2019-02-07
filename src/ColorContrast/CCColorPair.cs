// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Deque.ColorContrast
{
    public class CCColorPair
    {
        readonly DequeColor lighterColor;
        readonly DequeColor darkerColor;

        public CCColorPair(DequeColor color1, DequeColor color2)
        {
            double contrast1 = DequeColor.WHITE.Contrast(color1);
            double contrast2 = DequeColor.WHITE.Contrast(color2);

            lighterColor = contrast1 < contrast2 ? color1 : color2;
            darkerColor = contrast1 < contrast2 ? color2 : color1;
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CCColorPair p = (CCColorPair)obj;
                return p.ToString().Equals(ToString());
            }
        }

        /**
         * True when the pair of colors are not visually different.
         */
        public Boolean AreVisuallySimilarColors()
        {
            return lighterColor.IsSameColor(darkerColor);
        }

        /**
         * True when a pair of colors have visibly similar pairs of colors.
         */
        public Boolean IsVisiblySimilarTo(CCColorPair otherPair)
        {
            return lighterColor.IsSameColor(otherPair.lighterColor) &&
                darkerColor.IsSameColor(otherPair.darkerColor);
        }

        /**
         * Calculate the Color Contrast between the pair of colors.
         */
        public double ColorContrast()
        {
            return Math.Round(lighterColor.Contrast(darkerColor), 2);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return lighterColor.ToString() + " --" + ColorContrast() + "-> " + darkerColor.ToString();
        }
    }
}
