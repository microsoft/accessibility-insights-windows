// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Actions.Contexts.Fakes;
using AccessibilityInsights.Actions.Fakes;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Bases.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Drawing.Fakes;

namespace AccessibilityInsights.ActionsTests.Actions
{
    [TestClass]
    public class ScreenShotActionUnitTests
    {
        [TestMethod]
        [Timeout(2000)]
        public void CaptureScreenShot_ElementWithoutBoundingRectangle_NoScreenShot()
        {
            using (ShimsContext.Create())
            {
                bool bitmapsetcalled = false;

                // no bounding rectangle.
                A11yElement element = new ShimA11yElement
                {
                    ParentGet = () => null,
                    BoundingRectangleGet = () => Rectangle.Empty,
                };

                ElementDataContext dc = new ShimElementDataContext()
                {
                    ScreenshotSetBitmap = (_) => bitmapsetcalled = true,
                };

                ElementContext elementContext = new ShimElementContext
                {
                    ElementGet = () => element,
                    DataContextGet = () => dc,
                };

                ShimDataManager.GetDefaultInstance = () => new ShimDataManager
                {
                    GetElementContextGuid = (_) => elementContext,
                };

                ScreenShotAction.CaptureScreenShot(Guid.NewGuid());

                // screenshot is not set(null)
                Assert.IsNull(dc.Screenshot);
                // ScreenShotSet was not called.
                Assert.IsFalse(bitmapsetcalled);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void CaptureScreenShot_ElementWithBoundingRectangle_ScreenShotCreated()
        {
            using (ShimsContext.Create())
            {
                bool bitmapsetcalled = false;

                //  bounding rectangle exists.
                A11yElement element = new ShimA11yElement
                {
                    ParentGet = () => null,
                    BoundingRectangleGet = () => new Rectangle(0,0,10,10),
                    UniqueIdGet = () => 1,
                };

                ElementDataContext dc = new ShimElementDataContext()
                {
                    ScreenshotSetBitmap = (_) => bitmapsetcalled = true,
                };

                ElementContext elementContext = new ShimElementContext
                {
                    ElementGet = () => element,
                    DataContextGet = () => dc,
                };

                ShimDataManager.GetDefaultInstance = () => new ShimDataManager
                {
                    GetElementContextGuid = (_) => elementContext,
                };

                Graphics g = new ShimGraphics();

                ShimGraphics.FromImageImage = (_) => g;

                ScreenShotAction.CaptureScreenShot(Guid.NewGuid());

                // bitmap is set
                Assert.IsTrue(bitmapsetcalled);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void CaptureScreenShotOnWCOS_ElementWithBoundingRectangle_NoScreenShot()
        {
            using (ShimsContext.Create())
            {
                bool bitmapsetcalled = false;

                //  bounding rectangle exists.
                A11yElement element = new ShimA11yElement
                {
                    ParentGet = () => null,
                    BoundingRectangleGet = () => new Rectangle(0, 0, 10, 10),
                    UniqueIdGet = () => 1,
                };

                ElementDataContext dc = new ShimElementDataContext()
                {
                    ScreenshotSetBitmap = (_) => bitmapsetcalled = true,
                };

                ElementContext elementContext = new ShimElementContext
                {
                    ElementGet = () => element,
                    DataContextGet = () => dc,
                };

                ShimDataManager.GetDefaultInstance = () => new ShimDataManager
                {
                    GetElementContextGuid = (_) => elementContext,
                };

                ShimBitmap.ConstructorInt32Int32 = (_, w, h) => throw new TypeInitializationException("Bitmap", null);

                ScreenShotAction.CaptureScreenShot(Guid.NewGuid());

                // screenshot is not set(null)
                Assert.IsNull(dc.Screenshot);
                // ScreenShotSet was not called.
                Assert.IsFalse(bitmapsetcalled);

            }
        }
    }
}
