// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights;
using AccessibilityInsights.SharedUx.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsightsUnitTests.MainWindowHelpers
{
    [TestClass]
    public class InitializeUnitTests
    {
        const double DefaultTop = 60.0;
        const double DefaultLeft = 80.0;
        const double DefaultHeight = 720.0;
        const double DefaultWidth = 870.0;
        const double VirtualLeft = -1.0;
        const double VirtualTop = -1.0;
        const double VirtualWidth = 1920.0;
        const double VirtualHeight = 1080.0;
        const double SmallVirtualWidth = 640;   // 1280 x 1024 at 200% zoom
        const double SmallVirtualHeight = 512;  // 1280 x 1024 at 200% zoom

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_DefaultValues_ValuesFit_UsesDefaultValues()
        {
            var layout = GetInitialConfig();
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(DefaultTop, layout.Top);
            Assert.AreEqual(DefaultLeft, layout.Left);
            Assert.AreEqual(DefaultWidth, layout.Width);
            Assert.AreEqual(DefaultHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValues_ValuesFit_UsesSavedValues()
        {
            const double oldTop = 80;
            const double oldLeft = 90;
            const double oldWidth = 1000;
            const double oldHeight = 900;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(oldTop, layout.Top);
            Assert.AreEqual(oldLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesStraddleTopEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldTop = VirtualTop - 1;
            const double oldLeft = 90;
            const double oldWidth = 1000;
            const double oldHeight = 900;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(VirtualTop, layout.Top);
            Assert.AreEqual(oldLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesStraddleLeftEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldTop = 80;
            const double oldLeft = VirtualLeft - 1;
            const double oldWidth = 1000;
            const double oldHeight = 900;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(oldTop, layout.Top);
            Assert.AreEqual(VirtualLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesStraddleBottomEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldTop = VirtualTop + VirtualHeight - 1;
            const double oldLeft = 90;
            const double oldWidth = 1000;
            const double oldHeight = 900;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(VirtualTop + VirtualHeight - oldHeight, layout.Top);
            Assert.AreEqual(oldLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesStraddleRightEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldTop = 80;
            const double oldLeft = VirtualLeft + VirtualWidth - 1;
            const double oldWidth = 1000;
            const double oldHeight = 900;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(oldTop, layout.Top);
            Assert.AreEqual(VirtualLeft + VirtualWidth - oldWidth, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesBeyondTopEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldLeft = 90;
            const double oldWidth = 1000;
            const double oldHeight = 900;
            const double oldTop = VirtualTop - oldHeight - 1;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(DefaultTop, layout.Top);
            Assert.AreEqual(DefaultLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesBeyondLeftEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldTop = 80;
            const double oldWidth = 1000;
            const double oldHeight = 900;
            const double oldLeft = VirtualLeft - oldWidth - 1;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(DefaultTop, layout.Top);
            Assert.AreEqual(DefaultLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesBeyondBottomEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldTop = VirtualTop + VirtualHeight + 1;
            const double oldLeft = 90;
            const double oldWidth = 1000;
            const double oldHeight = 900;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(DefaultTop, layout.Top);
            Assert.AreEqual(DefaultLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_SavedValuesBeyondRightEdgeOfVirtualScreen_IsOnScreen()
        {
            const double oldTop = 80;
            const double oldLeft = VirtualLeft + VirtualWidth + 1;
            const double oldWidth = 1000;
            const double oldHeight = 900;

            var layout = GetInitialConfig(oldTop, oldLeft, oldHeight, oldWidth);
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, VirtualHeight);

            Assert.AreEqual(DefaultTop, layout.Top);
            Assert.AreEqual(DefaultLeft, layout.Left);
            Assert.AreEqual(oldWidth, layout.Width);
            Assert.AreEqual(oldHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_DefaultValues_VirtualScreenTooNarrow_UsesFullVirtualScreenWidth()
        {
            var layout = GetInitialConfig();
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, SmallVirtualWidth, VirtualHeight);

            Assert.AreEqual(DefaultTop, layout.Top);
            Assert.AreEqual(VirtualLeft, layout.Left);
            Assert.AreEqual(SmallVirtualWidth, layout.Width);
            Assert.AreEqual(DefaultHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_DefaultValues_VirtualScreenTooShort_UsesFullVirtualScreenHeight()
        {
            var layout = GetInitialConfig();
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, DefaultTop, DefaultLeft,
                VirtualLeft, VirtualTop, VirtualWidth, SmallVirtualHeight);

            Assert.AreEqual(VirtualTop, layout.Top);
            Assert.AreEqual(DefaultLeft, layout.Left);
            Assert.AreEqual(DefaultWidth, layout.Width);
            Assert.AreEqual(SmallVirtualHeight, layout.Height);
        }

        [TestMethod]
        public void EnsureWindowIsInVirtualScreenWithInjection_DefaultValues_VirtualScreenTooSmall_UsesFullVirtualScreen()
        {
            var layout = GetInitialConfig();
            MainWindow.EnsureWindowIsInVirtualScreenWithInjection(layout, double.NaN, double.NaN,
                VirtualLeft, VirtualTop, SmallVirtualWidth, SmallVirtualHeight);

            Assert.AreEqual(VirtualTop, layout.Top);
            Assert.AreEqual(VirtualLeft, layout.Left);
            Assert.AreEqual(SmallVirtualWidth, layout.Width);
            Assert.AreEqual(SmallVirtualHeight, layout.Height);
        }

        private AppLayout GetInitialConfig(double? top = null, double? left = null, double? height = null, double? width = null)
        {
            var layout = new AppLayout(double.NaN, double.NaN);

            if (top.HasValue)
                layout.Top = top.Value;
            if (left.HasValue)
                layout.Left = left.Value;
            if (height.HasValue)
                layout.Height = height.Value;
            if (width.HasValue)
                layout.Width = width.Value;

            return layout;
        }
    }
}
