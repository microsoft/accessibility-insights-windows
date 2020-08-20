// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.SharedUxTests.Converters
{
    [TestClass]
    public class ColumnMaxWidthSpacingConverterTests
    {
        [TestMethod]
        public void BoolToVisibilityConverter_Convert()
        {
            double testWidth = 10;
            double expectedWidth = 6;
            ColumnMaxWidthSpacingConverter converter = new ColumnMaxWidthSpacingConverter();

            Assert.AreEqual(converter.Convert(testWidth, typeof(double), null, null), expectedWidth);
        }

        [TestMethod]
        public void BoolToVisibilityConverter_ConvertBack()
        {
            double testWidth = 4;
            double expectedWidth = 8;
            ColumnMaxWidthSpacingConverter converter = new ColumnMaxWidthSpacingConverter();

            Assert.AreEqual(converter.ConvertBack(testWidth, typeof(double), null, null), expectedWidth);
        }
    }
}
