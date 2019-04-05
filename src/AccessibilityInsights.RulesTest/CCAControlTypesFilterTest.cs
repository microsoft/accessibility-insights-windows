// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.RulesTest
{
    [TestClass]
    public class CCAControlTypesFilterTest
    {
        [TestMethod]
        public void GetDefaultInstanceIsNotNull()
        {
            var instance = CCAControlTypesFilter.GetDefaultInstance();
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void GetDefaultInstanceIsSingletone()
        {
            var instance1 = CCAControlTypesFilter.GetDefaultInstance();
            var instance2 = CCAControlTypesFilter.GetDefaultInstance();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void ContainsTest()
        {
            var instance1 = CCAControlTypesFilter.GetDefaultInstance();
            var listOfTypes = ControlType.All.GetEnumerator();

            while (listOfTypes.MoveNext())
            {
                Assert.IsTrue(instance1.Contains(listOfTypes.Current));
            }

            Assert.IsFalse(instance1.Contains(0));
            Assert.IsFalse(instance1.Contains(-1));
        }
    }
}
