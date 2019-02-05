using Microsoft.VisualStudio.TestTools.UnitTesting;
using Deque.ColorContrast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deque.ColorContrast.Tests
{
    [TestClass()]
    public class CCColorTests
    {
        [TestMethod()]
        public void ColorContrastTest_BlackAndWhite()
        {
            DequeColor black = new DequeColor(0, 0, 0);
            DequeColor white = new DequeColor(255, 255, 255);

            Assert.AreEqual(21.0, black.Contrast(white));
            Assert.AreEqual(21.0, white.Contrast(black));
        }

        [TestMethod()]
        public void ColorContrastTest_WhiteAndRed()
        {
            Assert.AreEqual(4.00, Math.Round(DequeColor.RED.Contrast(DequeColor.WHITE), 2));
        }

        [TestMethod()]
        public void ColorContrastTest_RedAndBlue()
        {
            Assert.AreEqual(2.15, Math.Round(DequeColor.RED.Contrast(DequeColor.BLUE), 2));
        }

        [TestMethod()]
        public void ColorContrastTest_RedAndGreen()
        {
            Assert.AreEqual(2.91, Math.Round(DequeColor.RED.Contrast(DequeColor.GREEN), 2));
        }

        [TestMethod()]
        public void LuminanceOf_White()
        {
            Assert.AreEqual(1, new DequeColor(255, 255, 255).Luminance());
        }

        [TestMethod()]
        public void LuminanceOf_Black()
        {
            Assert.AreEqual(0, new DequeColor(0, 0, 0).Luminance());
        }
    }
}