// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Bases;
using System;
using System.Globalization;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Class ColorContrastViewModel
    ///
    /// Contains information about two colors, whether they pass color contrast test,
    /// their ratio, etc
    /// </summary>
    public class ColorContrastViewModel : ViewModelBase
    {
        /// <summary>
        /// Ratios needed to pass color contrast
        /// </summary>
        const double SMALL_TEXT_THRESHOLD = 4.5;
        const double LARGE_TEXT_THRESHOLD = 3.0;
        const double NON_TEXT_THRESHOLD = 3.0;

        /// <summary>
        /// Constructor
        /// </summary>
        public ColorContrastViewModel() { }

        /// <summary>
        /// Reset fields to initial state
        /// </summary>
        public void Reset()
        {
            this.Element = null;
            this.BugId = null;
        }

        private A11yElement element;
        /// <summary>
        /// Element
        /// </summary>
        public A11yElement Element
        {
            get
            {
                return element;
            }
            set
            {
                element = value;
                OnPropertyChanged(nameof(Element));
            }
        }

        private System.Windows.Media.Color firstColor;
        public System.Windows.Media.Color FirstColor
        {
            get
            {
                return firstColor;
            }
            set
            {
                firstColor = value;
                // Indicates all properties have changed
                OnPropertyChanged(null);
            }
        }

        private System.Windows.Media.Color secondColor;
        public System.Windows.Media.Color SecondColor
        {
            get
            {
                return secondColor;
            }
            set
            {
                secondColor = value;
                // Indicates all properties have changed
                OnPropertyChanged(null);
            }
        }

        /// <summary>
        /// Contrast ratio
        /// </summary>
        public double Ratio
        {
            get
            {
                return CalculateContrastRatio(FirstColor, SecondColor);
            }
        }

        /// <summary>
        /// Formatted to 3 decimal places and then :1, ex: 5.444:1
        /// </summary>
        public String FormattedRatio
        {
            get
            {
                return (Math.Truncate(1000 * Ratio) / 1000).ToString(CultureInfo.InvariantCulture) + ":1";
            }
        }

        /// <summary>
        /// Whether the current ratio passes on small text
        /// </summary>
        public bool PassSmallText
        {
            get
            {
                return Ratio >= SMALL_TEXT_THRESHOLD;
            }
        }

        /// <summary>
        /// Whether the current ratio passes on large text
        /// </summary>
        public bool PassLargeText
        {
            get
            {
                return Ratio >= LARGE_TEXT_THRESHOLD;
            }
        }

        /// <summary>
        /// Whether the current ratio passes on graphical objects
        /// </summary>
        public bool PassNonTextObjects
        {
            get
            {
                return Ratio >= NON_TEXT_THRESHOLD;
            }
        }

        private int? bugId;
        /// <summary>
        /// Bug id of this element
        /// </summary>
        public int? BugId
        {
            get
            {
                return bugId;
            }
            set
            {
                this.bugId = value;
                OnPropertyChanged(nameof(BugIdString));
            }
        }

        public string BugIdString
        {
            get
            {
                return BugId.HasValue ? BugId.ToString() : null;
            }
        }

        /// <summary>
        /// Find the contrast ratio: https://www.w3.org/WAI/GL/wiki/Contrast_ratio
        /// </summary>
        /// <param name="first">first color</param>
        /// <param name="second">second color</param>
        /// <returns></returns>
        public static double CalculateContrastRatio(System.Windows.Media.Color first, System.Windows.Media.Color second)
        {
            var relLuminanceOne = GetRelativeLuminance(first);
            var relLuminanceTwo = GetRelativeLuminance(second);
            return (Math.Max(relLuminanceOne, relLuminanceTwo) + 0.05)
                / (Math.Min(relLuminanceOne, relLuminanceTwo) + 0.05);
        }

        /// <summary>
        /// Get luminance, source is: https://www.w3.org/WAI/GL/wiki/Relative_luminance
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double GetRelativeLuminance(System.Windows.Media.Color c)
        {
            var rSRGB = c.R / 255.0;
            var gSRGB = c.G / 255.0;
            var bSRGB = c.B / 255.0;

            var r = rSRGB <= 0.04045 ? rSRGB / 12.92 : Math.Pow(((rSRGB + 0.055) / 1.055), 2.4);
            var g = gSRGB <= 0.04045 ? gSRGB / 12.92 : Math.Pow(((gSRGB + 0.055) / 1.055), 2.4);
            var b = bSRGB <= 0.04045 ? bSRGB / 12.92 : Math.Pow(((bSRGB + 0.055) / 1.055), 2.4);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }
    }
}
