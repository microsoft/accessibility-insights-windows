// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;
using System.Globalization;

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    /**
     * Provides utilities for working with Colors without an Alpha component.
     */
    public class Color
    {

        /**
         * The luminance difference that will determine whether two colors are the same.
         * This helps deal with Anti-Aliasing in determining transitions for detecting text.
         */
        const double SAME_COLOR_THRESHOLD = 1.1;

        /**
         * Some commonly used colors.
         */
        public static readonly Color WHITE = new Color(255, 255, 255);
        public static readonly Color BLACK = new Color(0, 0, 0);
        public static readonly Color RED = new Color(255, 0, 0);
        public static readonly Color GREEN = new Color(0, 255, 0);
        public static readonly Color BLUE = new Color(0, 0, 255);

        /**
         * Below are constants from the W3C Luminance Calculation
         * http://www.w3.org/TR/WCAG/#dfn-relative-luminance
         */
        const double W3C_LUMINANCE_CALCULATION_THRESHOLD = .03928;
        const double W3C_LUMINANCE_RED_COMPONENT_MULTIPLIER = 0.2126;
        const double W3C_LUMINANCE_GREEN_COMPONENT_MULTIPLIER = 0.7152;
        const double W3C_LUMINANCE_BLUE_COMPONENT_MULTIPLIER = 0.0722;

        private readonly int Red;
        private readonly int Green;
        private readonly int Blue;

        public Color(int red, int green, int blue)
        {
            const string failMessage = "Color components are values between 0 and 255";

            if (red >= 0 && red <= 255) Red = red;
            else throw new DequeColorException(failMessage);

            if (green >= 0 && green <= 255) Green = green;
            else throw new DequeColorException(failMessage);

            if (blue >= 0 && blue <= 255) Blue = blue;
            else throw new DequeColorException(failMessage);
        }

        public Color(System.Drawing.Color color) : this(color.R, color.G, color.B)
        {
            if (color.A < 255)
            {
                throw new DequeColorException("DequeColors can only be instantiated with fully opaque (Alpha = 255) colors. " +
                    "The DequeColor Blend method can help you if you know the background color");
            }
        }

        /**
         * Calculate contrast between this color and another color.
         */
        public double Contrast(Color otherColor)
        {
            double luminance1 = Luminance();
            double luminance2 = otherColor.Luminance();

            if (luminance1 > luminance2)
            {
                return (luminance1 + .05) / (luminance2 + .05);
            }
            else
            {
                return (luminance2 + .05) / (luminance1 + .05);
            }
        }

        /**
         * Determines if two colors are close enough to be considered the same to the naked eye.
         */
        public bool IsSameColor(Color otherColor)
        {
            return Contrast(otherColor) < SAME_COLOR_THRESHOLD;
        }

        /*
         * The Luminance calculation from the w3c website.
         * http://www.w3.org/TR/2008/REC-WCAG20-20081211/#relativeluminancedef
         */
        public double Luminance()
        {
            // The Magic Numbers are from the W3C Calculation, and I don't know what they represent.
            // Seemed more clear to present it the same way it was presented there.
            double redComponent = LuminanceComponent(Red) * W3C_LUMINANCE_RED_COMPONENT_MULTIPLIER;
            double greenComponent = LuminanceComponent(Green) * W3C_LUMINANCE_GREEN_COMPONENT_MULTIPLIER;
            double blueComponent = LuminanceComponent(Blue) * W3C_LUMINANCE_BLUE_COMPONENT_MULTIPLIER;

            return redComponent + greenComponent + blueComponent;
        }

        /**
         * The W3C Luinance calculation separates the component colors. This function calculates
         * the Luminance component from a given color component.
         * 
         */
        private static double LuminanceComponent(int color)
        {
            double colorRatio = color / 255.0;
            return colorRatio <= W3C_LUMINANCE_CALCULATION_THRESHOLD ?
                ((colorRatio) / 12.92) :
                Math.Pow((colorRatio + 0.055) / 1.055, 2.4);
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
                Color p = (Color)obj;
                return (Red == p.Red) && (Blue == p.Blue) && (Green == p.Green);
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "DequeColor({0}, {1}, {2})", Red, Green, Blue);
        }
    }

    [Serializable]
    public class DequeColorException : System.Exception
    {
        public DequeColorException(string message) : base(message) { }

        public DequeColorException()
        {
        }

        public DequeColorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DequeColorException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
