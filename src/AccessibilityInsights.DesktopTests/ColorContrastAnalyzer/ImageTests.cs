// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Desktop.ColorContrastAnalyzer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Drawing;
using static AccessibilityInsights.Desktop.ColorContrastAnalyzer.ColorContrastResult;
using CCColor = AccessibilityInsights.Desktop.ColorContrastAnalyzer.Color;

namespace AccessibilityInsights.DesktopTests.ColorContrastAnalyzer
{
    [TestClass()]
    public class ImageTests
    {

        // A convenience method for loading Bitmap images from test resources.
        public static BitmapCollection LoadFromResources(string name)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("AccessibilityInsights.DesktopTests.TestImages." + name);
            Bitmap bmp = new Bitmap(myStream);
            return new BitmapCollection(bmp);
        }

        [TestMethod, Timeout(2000)]
        public void SimpleBlackAndGreyButton()
        {
            var image = LoadFromResources("simple_black_and_grey_button.bmp");

            var result = image.RunColorContrastCalculation();

            Assert.AreEqual(new ColorPair(new CCColor(204, 204, 204), new CCColor(0, 0, 0)), 
                result.GetMostLikelyColorPair());

            Assert.AreEqual(Confidence.High, result.ConfidenceValue());
        }

        /**
         * In this test we are analyzing two similar images. One with text near the bottom of the image
         * and the other with the text near the top. The results should be identical.
         */
        [TestMethod, Timeout(2000)]
        public void CortanaImagesWithDifferentOffsets()
        {
            ColorPair expected = new ColorPair(new CCColor(0, 0, 0), new CCColor(139, 204, 41));

            ColorContrastResult resultOffsetDownImage = LoadFromResources("cortana_with_offset_down.bmp")
                .RunColorContrastCalculation();

            ColorContrastResult resultOffsetUpImage = LoadFromResources("cortana_with_offset_up.bmp")
                .RunColorContrastCalculation();

            Assert.AreEqual(expected, resultOffsetUpImage.GetMostLikelyColorPair());
            Assert.AreEqual(expected, resultOffsetDownImage.GetMostLikelyColorPair());

            Assert.AreEqual(Confidence.High, resultOffsetDownImage.ConfidenceValue());
            Assert.AreEqual(Confidence.High, resultOffsetUpImage.ConfidenceValue());
        }

        [TestMethod, Timeout(2000)]
        public void VisualStudioTab()
        {
            var colorContrastResult = LoadFromResources("visual_studio_tab.bmp").RunColorContrastCalculation();

            Assert.AreEqual(Confidence.Low, colorContrastResult.ConfidenceValue());
        }

        /**
         * Note in this test case we have Mide confidence. As such, we also are asserting that the color is only 
         * approximately what we expect, this allows our algorithm a little flexibility, without having to modify
         * these tests every time we sneeze on our configuration file.
         */
        [TestMethod, Timeout(2000)]
        public void WeirdTextArrangement()
        {
            var image = LoadFromResources("weird_text_arrangement.bmp");

            ColorPair approximateColorPair = new ColorPair(new CCColor(37, 37, 37), new CCColor(193, 183, 165));

            ColorContrastResult result = image.RunColorContrastCalculation();

            Assert.IsTrue(approximateColorPair.IsVisiblySimilarTo(result.GetMostLikelyColorPair()), result.GetMostLikelyColorPair().ToString());

            Assert.AreEqual(Confidence.High, result.ConfidenceValue());
        }

        [TestMethod, Timeout(2000)]
        public void BinaryRowSearchIterator()
        {

            List<int> rowOrder = new List<int>
            {
                23, 11, 17, 34, 40
            };

            foreach (var pixel in LoadFromResources("simple_black_and_grey_button.bmp").GetBinaryRowSearchIterator())
            {
                if (pixel.Row != rowOrder.First())
                {
                    rowOrder.RemoveAt(0);
                }

                Assert.AreEqual(rowOrder.First(), pixel.Row);
            }

            rowOrder.RemoveAt(0);

            Assert.AreEqual(0, rowOrder.Count(), "We should have inspected all expected rows.");
        }
    }
}