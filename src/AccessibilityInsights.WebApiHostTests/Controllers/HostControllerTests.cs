// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccessibilityInsights.WebApiHost.Controllers.Tests
{
    [TestClass()]
    public class HostControllerTests
    {
        [Timeout(2000)]
        [TestMethod()]
        public void TestExit_ExitAutoResetEvent_True()
        {
            var controller = new HostController();

            controller.Exit();

            HostController.WaitForExitEvent();
        }
    }
}
