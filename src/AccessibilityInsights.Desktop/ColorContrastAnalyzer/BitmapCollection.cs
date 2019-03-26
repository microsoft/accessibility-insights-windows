// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.Desktop.ColorContrastAnalyzer
{

    public class BitmapCollection : ImageCollection
    {
        private readonly System.Drawing.Bitmap bitmap;

        public BitmapCollection(System.Drawing.Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public override int NumColumns()
        {
            return bitmap.Width;
        }

        public override int NumRows()
        {
            return bitmap.Height;
        }

        public override Color GetColor(int row, int column)
        {
            System.Drawing.Color color = bitmap.GetPixel(column, row);

            return new Color(color);
        }
    }
}
