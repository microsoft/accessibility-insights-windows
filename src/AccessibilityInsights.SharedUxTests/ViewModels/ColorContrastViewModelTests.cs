// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUxTest.ViewModels
{
    /// <summary>
    /// Tests various properties of color contrast view model
    /// and their interaction
    ///
    /// Ratio values are checked against the results of
    /// colour contrast analyzer tool
    /// </summary>
    [TestClass]
    public class ColorContrastViewModelTests
    {
        #region basic relative luminance correct

        [TestMethod]
        public void TestGetRelativeLuminance_White()
        {
            double lumWhite = ColorContrastViewModel.GetRelativeLuminance(Colors.White);
            Assert.AreEqual(1.0, lumWhite);
        }

        [TestMethod]
        public void TestGetRelativeLuminance_Black()
        {
            double lumBlack = ColorContrastViewModel.GetRelativeLuminance(Colors.Black);
            Assert.AreEqual(0.0, lumBlack);
        }

        #endregion

        #region ratios are correct and commutative

        [TestMethod]
        public void TestCalculateContrastRatio_BlackWhite()
        {
            Assert.AreEqual(21.0, ColorContrastViewModel
                .CalculateContrastRatio(Colors.Black, Colors.White));
        }

        [TestMethod]
        public void TestCalculateContrastRatio_WhiteWhite()
        {
            Assert.AreEqual(1.0, ColorContrastViewModel
                .CalculateContrastRatio(Colors.White, Colors.White));
        }

        [TestMethod]
        public void TestCalculateContrastRatio_RedWhite()
        {
            var value = ColorContrastViewModel
                .CalculateContrastRatio(Color.FromRgb(255, 0, 0), Colors.White);
            Assert.AreEqual(4.0, Math.Round(value, 1));
        }

        [TestMethod]
        public void TestCalculateContrastRatio_WhiteRed()
        {
            var value = ColorContrastViewModel
                .CalculateContrastRatio(Colors.White, Color.FromRgb(255, 0, 0));
            Assert.AreEqual(4.0, Math.Round(value, 1));
        }

        [TestMethod]
        public void TestCalculateContrastRatio_GreenWhite()
        {
            var value = ColorContrastViewModel
                .CalculateContrastRatio(Color.FromRgb(0, 255, 0), Colors.White);
            Assert.AreEqual(1.4, Math.Round(value, 1));
        }

        [TestMethod]
        public void TestCalculateContrastRatio_WhiteGreen()
        {
            var value = ColorContrastViewModel
                .CalculateContrastRatio(Colors.White, Color.FromRgb(0, 255, 0));
            Assert.AreEqual(1.4, Math.Round(value, 1));
        }

        [TestMethod]
        public void TestCalculateContrastRatio_WhiteBlue()
        {
            var value = ColorContrastViewModel
                .CalculateContrastRatio(Colors.White, Color.FromRgb(0, 0, 255));
            Assert.AreEqual(8.6, Math.Round(value, 1));
        }

        [TestMethod]
        public void TestCalculateContrastRatio_BlueWhite()
        {
            var value = ColorContrastViewModel
                .CalculateContrastRatio(Color.FromRgb(0, 0, 255), Colors.White);
            Assert.AreEqual(8.6, Math.Round(value, 1));
        }

        #endregion

        #region PassSmallText / PassLargeText correctly update
        [TestMethod]
        public void TestPassSmallText_WhiteWhite()
        {
            ColorContrastViewModel vm = new ColorContrastViewModel
            {
                FirstColor = Colors.White,
                SecondColor = Colors.White
            };
            Assert.IsFalse(vm.PassSmallText);
        }

        [TestMethod]
        public void TestPassLargeText_WhiteWhite()
        {
            ColorContrastViewModel vm = new ColorContrastViewModel
            {
                FirstColor = Colors.White,
                SecondColor = Colors.White
            };
            Assert.IsFalse(vm.PassLargeText);
        }

        [TestMethod]
        public void TestPassSmallText_WhiteBlack()
        {
            ColorContrastViewModel vm = new ColorContrastViewModel
            {
                FirstColor = Colors.White,
                SecondColor = Colors.Black
            };
            Assert.IsTrue(vm.PassSmallText);
        }

        [TestMethod]
        public void TestPassLargeText_WhiteBlack()
        {
            ColorContrastViewModel vm = new ColorContrastViewModel
            {
                FirstColor = Colors.White,
                SecondColor = Colors.Black
            };
            Assert.IsTrue(vm.PassLargeText);
        }

        [TestMethod]
        public void TestPassSmallText_WhiteRed()
        {
            ColorContrastViewModel vm = new ColorContrastViewModel
            {
                FirstColor = Colors.White,
                SecondColor = Color.FromRgb(255, 0, 0)
            };
            Assert.IsFalse(vm.PassSmallText);
        }

        [TestMethod]
        public void TestPassLargeText_WhiteRed()
        {
            ColorContrastViewModel vm = new ColorContrastViewModel
            {
                FirstColor = Colors.White,
                SecondColor = Color.FromRgb(255, 0, 0)
            };
            Assert.IsTrue(vm.PassLargeText);
        }
        #endregion
    }
}
