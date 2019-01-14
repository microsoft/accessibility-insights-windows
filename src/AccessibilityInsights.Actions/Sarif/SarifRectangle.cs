// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Actions.Sarif
{
    class SarifRectangle
    {
        public int left { get; }
        public int top { get; }
        public int width { get; }
        public int height { get; }

        public SarifRectangle(int left, int top, int width, int height)
        {
            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
        }
    }
}
