// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{
    public class Pixel
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public Color Color { get; private set; }

        public Pixel(Color color, int row, int column)
        {
            this.Row = row;
            this.Column = column;
            this.Color = color;
        }
    }
}
