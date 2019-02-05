using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deque.ColorContrast
{
    public class ColorContrastConfig
    {
        /**
         * The number of Color transitions we expect to make us believe a "Row" of Pixels
         * contains Text.
         */
        public static readonly int MIN_NUMBER_COLOR_TRANSITIONS = 10;

        /**
         * We will inevitably find other color combinations in a row. To be highly confident
         * we have text, we expect the most likely color combination to be at least this dominant.
         * Type: Double between 0 and 1.
         */
        public static readonly double TEXT_COLOR_PAIR_DOMINANCE_VALUE = .2;

        /**
         * We're going to binary search sample rows. Once two samples are this 
         * far apart we should not sample between them as finding text is highly unlikely.
         * A good value for this is the smallest reasonable font size for the DPI of the screen.
         * 
         * Note: Making this number too low will have dramatic performance impacts when a HIGH
         * confidence result is NOT found. EX: at 0, you would end up scanning the entire image.
         */
        public static readonly int MIN_SPACE_BETWEEN_SAMPLES = 12;
    }
}
