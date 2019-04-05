// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.Core.Types.Tests
{
    [TestClass()]
    public class ControlTypesTest
    {
        [TestMethod()]
        public void CheckExists()
        {
            Assert.AreEqual(true, ControlType.GetInstance().Exists(ControlType.UIA_AppBarControlTypeId));
            Assert.AreEqual(false, ControlType.GetInstance().Exists(0));
        }

        [TestMethod()]
        public void CheckGetNameById()
        {
            Assert.AreEqual("AppBar(50040)", ControlType.GetInstance().GetNameById(ControlType.UIA_AppBarControlTypeId));
        }
    }
}
