// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using System;
using System.Net;
using System.Web.Http.Results;

namespace AccessibilityInsights.WebApiHost.Controllers.Tests
{
    [TestClass()]
    public class CaptureControllerTests
    {
        [TestMethod()]
        public void TestCapture_Succeeded_Modification()
        {
            using (ShimsContext.Create())
            {
                var controller = new CaptureController();

                Actions.Fakes.ShimCaptureAction.SetTestModeDataContextGuidDataContextModeTreeViewModeBoolean = (g, da, tv, f) => true;

                var result = controller.Test(Guid.NewGuid());
                Assert.IsTrue(result is OkResult);
            }
        }

        [TestMethod()]
        public void TestCapture_Succeeded_NoModification()
        {
            using (ShimsContext.Create())
            {
                var controller = new CaptureController();

                Actions.Fakes.ShimCaptureAction.SetTestModeDataContextGuidDataContextModeTreeViewModeBoolean = (g, da, tv, f) => false;

                var result = controller.Test(Guid.NewGuid()) as StatusCodeResult;

                Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode);
            }
        }

        [TestMethod()]
        public void TestCapture_Failed_BadRequestAtException()
        {
            using (ShimsContext.Create())
            {
                var controller = new CaptureController();

                Actions.Fakes.ShimCaptureAction.SetTestModeDataContextGuidDataContextModeTreeViewModeBoolean = (g, da, tv, f) => throw new Exception();

                var result = controller.Test(Guid.NewGuid());
                Assert.IsTrue(result is BadRequestErrorMessageResult);
            }
        }
    }
}
