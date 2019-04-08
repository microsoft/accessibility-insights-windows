// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class PlatformPropertiesTests
    {
        [TestMethod]
        public void SimpleStyleTrue()
        {
            using (var e = new MockA11yElement())
            {
                var p = new A11yProperty(PlatformPropertyType.Platform_WindowsStylePropertyId, 1);
                e.PlatformProperties.Add(PlatformPropertyType.Platform_WindowsStylePropertyId, p);

                Assert.IsTrue(PlatformProperties.SimpleStyle.Matches(e));
            } // using
        }

        [TestMethod]
        public void SimpleStyleFalse()
        {
            using (var e = new MockA11yElement())
            {
                Assert.IsFalse(PlatformProperties.SimpleStyle.Matches(e));
            } // using
        }
    } // class
} // namespace
