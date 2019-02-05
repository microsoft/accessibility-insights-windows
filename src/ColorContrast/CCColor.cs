using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deque.ColorContrast
{
    /**
     * Provides utilities for working with Colors without an Alpha component.
     */
    public class DequeColor
    {

        public class DequeColorException : System.Exception
        {
            public DequeColorException(string message) : base(message) { }
        }

        /**
         * The luminance difference that will determine whether two colors are the same.
         * This helps deal with Anti-Aliasing in determining transitions for detecting text.
         */
        public static readonly double SAME_COLOR_THRESHOLD = 1.1;

        /**
         * Some commonly used colors.
         */
        public static readonly DequeColor WHITE = new DequeColor(255, 255, 255);
        public static readonly DequeColor BLACK = new DequeColor(0, 0, 0);
        public static readonly DequeColor RED = new DequeColor(255, 0, 0);
        public static readonly DequeColor GREEN = new DequeColor(0, 255, 0);
        public static readonly DequeColor BLUE = new DequeColor(0, 0, 255);

        /**
         * Below are constants from the W3C Luminance Calculation
         * http://www.w3.org/TR/WCAG/#dfn-relative-luminance
         */
        internal static readonly double W3C_LUMINANCE_CALCULATION_THRESHOLD = .03928;
        internal static readonly double W3C_LUMINANCE_RED_COMPONENT_MULTIPLIER = 0.2126;
        internal static readonly double W3C_LUMINANCE_GREEN_COMPONENT_MULTIPLIER = 0.7152;
        internal static readonly double W3C_LUMINANCE_BLUE_COMPONENT_MULTIPLIER = 0.0722;

        private readonly int Red;
        private readonly int Green;
        private readonly int Blue;

        public DequeColor(int red, int green, int blue)
        {
            const string failMessage = "Color components are values between 0 and 255";

            if (red >= 0 && red <= 255) Red = red;
            else throw new DequeColorException(failMessage);

            if (green >= 0 && green <= 255) Green = green;
            else throw new DequeColorException(failMessage);

            if (blue >= 0 && blue <= 255) Blue = blue;
            else throw new DequeColorException(failMessage);
        }

        public DequeColor(Color color) : this(color.R, color.G, color.B)
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
        public double Contrast(DequeColor otherColor)
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
        public bool IsSameColor(DequeColor otherColor)
        {
            return Contrast(otherColor) < SAME_COLOR_THRESHOLD;
        }

        /*
         * The Luminance calculation from the w3c website.
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
                DequeColor p = (DequeColor)obj;
                return (Red == p.Red) && (Blue == p.Blue) && (Green == p.Green);
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("DequeColor({0}, {1}, {2})", Red, Green, Blue);
        }
    }
}
