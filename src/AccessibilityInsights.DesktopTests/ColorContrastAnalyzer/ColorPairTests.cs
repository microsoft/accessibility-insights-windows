// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.ColorContrastAnalyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.DesktopTests.ColorContrastAnalyzer
{
    [TestClass()]
    public class ColorPairTests
    {
        [TestMethod, Timeout(2000)]
        public void IsVisiblySimilarToTest()
        {
            ColorPair colorPair1 = new ColorPair(new Color(0, 0, 0), new Color(1, 1, 1));
            ColorPair colorPair2 = new ColorPair(new Color(2, 2, 2), new Color(3, 3, 3));

            Assert.IsTrue(colorPair1.IsVisiblySimilarTo(colorPair2));
        }
    }
}