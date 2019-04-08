// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.QualityTools.Testing.Fakes;
using System.Web.Http.Results;

namespace AccessibilityInsights.WebApiHost.Controllers.Tests
{
    [TestClass()]
    public class SelectControllerTests
    {
        [TestMethod()]
        public void Current_NoSelected_NotFound()
        {
            using (ShimsContext.Create())
            {
                var sa = new Axe.Windows.Actions.Fakes.ShimSelectAction();
                Axe.Windows.Actions.Fakes.ShimSelectAction.GetDefaultInstance = () => sa;
                sa.GetSelectedElementContextId = () => null;

                var controller = new SelectController();

                var result = controller.Current();

                Assert.IsTrue(result is NotFoundResult);
            }
        }

        [TestMethod()]
        public void Current_Selected_Found()
        {
            using (ShimsContext.Create())
            {
                var guid = Guid.NewGuid();
                var sa = new Axe.Windows.Actions.Fakes.ShimSelectAction();
                Axe.Windows.Actions.Fakes.ShimSelectAction.GetDefaultInstance = () => sa;
                sa.GetSelectedElementContextId = () => guid;

                var controller = new SelectController();

                var result = controller.Current();

                Assert.IsTrue(result is OkNegotiatedContentResult<Guid?>);
            }
        }
    }
}
