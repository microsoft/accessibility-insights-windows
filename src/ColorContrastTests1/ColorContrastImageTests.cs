// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Deque.ColorContrast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Drawing;
using static Deque.ColorContrast.ColorContrastResult;

namespace Deque.ColorContrast.Tests
{
    [TestClass()]
    public class IDequeImageTests
    {

        // A convenience method for loading Bitmap images from test resources.
        public static DequeBitmap LoadFromResources(string name)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("ColorContrastTests.TestImages." + name);
            Bitmap bmp = new Bitmap(myStream);
            return new DequeBitmap(bmp);
        }

        [TestMethod, Timeout(2000)]
        public void SimpleBlackAndGreyButton()
        {
            DequeImage dequeImage = LoadFromResources("simple_black_and_grey_button.bmp");

            var result = dequeImage.RunColorContrastCalculation();

            Assert.AreEqual(new CCColorPair(new DequeColor(204, 204, 204), new DequeColor(0, 0, 0)), 
                result.GetMostLikelyColorPair());

            Assert.AreEqual(Confidence.HIGH, result.ConfidenceValue());
        }

        /**
         * In this test we are analyzing two similar images. One with text near the bottom of the image
         * and the other with the text near the top. The results should be identical.
         */
        [TestMethod, Timeout(2000)]
        public void CortanaImagesWithDifferentOffsets()
        {
            CCColorPair expected = new CCColorPair(new DequeColor(0, 0, 0), new DequeColor(139, 204, 41));

            ColorContrastResult resultOffsetDownImage = LoadFromResources("cortana_with_offset_down.bmp")
                .RunColorContrastCalculation();

            ColorContrastResult resultOffsetUpImage = LoadFromResources("cortana_with_offset_up.bmp")
                .RunColorContrastCalculation();

            Assert.AreEqual(expected, resultOffsetUpImage.GetMostLikelyColorPair());
            Assert.AreEqual(expected, resultOffsetDownImage.GetMostLikelyColorPair());

            Assert.AreEqual(Confidence.HIGH, resultOffsetDownImage.ConfidenceValue());
            Assert.AreEqual(Confidence.HIGH, resultOffsetUpImage.ConfidenceValue());
        }

        /**
         * Note in this test case we have Mide confidence. As such, we also are asserting that the color is only 
         * approximately what we expect, this allows our algorithm a little flexibility, without having to modify
         * these tests every time we sneeze on our configuration file.
         */
        [TestMethod, Timeout(2000)]
        public void WeirdTextArrangement()
        {
            DequeImage dequeImage = LoadFromResources("weird_text_arrangement.bmp");

            CCColorPair approximateColorPair = new CCColorPair(new DequeColor(37, 37, 37), new DequeColor(193, 183, 165));

            ColorContrastResult result = dequeImage.RunColorContrastCalculation();

            Assert.IsTrue(approximateColorPair.IsVisiblySimilarTo(result.GetMostLikelyColorPair()), result.GetMostLikelyColorPair().ToString());

            Assert.AreEqual(Confidence.HIGH, result.ConfidenceValue());
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
                if (pixel.row != rowOrder.First())
                {
                    rowOrder.RemoveAt(0);
                }

                Assert.AreEqual(rowOrder.First(), pixel.row);
            }

            rowOrder.RemoveAt(0);

            Assert.AreEqual(0, rowOrder.Count(), "We should have inspected all expected rows.");
        }
    }
}