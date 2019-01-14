// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Types;
using AccessibilityInsights.Desktop.UIAutomation.Patterns;

namespace AccessibilityInsights.Desktop.UIAutomation.Support
{
    /// <summary>
    /// class TextRangeFinder
    /// helper class to do "FindXXX()" method call with a given TextRange. 
    /// it supports next/previou and forward/backward
    /// </summary>
    public class TextRangeFinder
    {
        private TextRange OriginalRange;
        private TextRange FoundRange;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="range">Original range</param>
        public TextRangeFinder(TextRange range)
        {
            this.OriginalRange = range;
        }

        public TextRange Find(int id, dynamic value, bool backward, bool ignorecase)
        {
            var range = GetRangeForFind(backward);
      
            if (id == TextAttributeType.UIA_TextAttributeId)
            {
                this.FoundRange = range.FindText(value, backward, ignorecase);
            }
            else
            {
                this.FoundRange = range.FindAttribute(id, value, backward);
            }

            return this.FoundRange;
        }

        private TextRange GetRangeForFind(bool backward)
        {
            var range = this.OriginalRange.Clone();

            if(this.FoundRange != null)
            {
                if(backward == true)
                {
                    range.MoveEndpointByRange(UIAutomationClient.TextPatternRangeEndpoint.TextPatternRangeEndpoint_End, this.FoundRange, UIAutomationClient.TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start);
                }
                else
                {
                    range.MoveEndpointByRange(UIAutomationClient.TextPatternRangeEndpoint.TextPatternRangeEndpoint_Start, this.FoundRange, UIAutomationClient.TextPatternRangeEndpoint.TextPatternRangeEndpoint_End);
                }
            }

            return range;
        }
    }
}
