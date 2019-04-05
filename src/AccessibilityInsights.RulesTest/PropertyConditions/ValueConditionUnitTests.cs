// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Axe.Windows.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.RulesTest.PropertyConditions
{
    [TestClass]
    public class ValueConditionUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [Timeout(2000)]
        public void Equals_OtherIsSameType_ThrowsNotSupportedException()
        {
            var p = new ValueCondition<string>((e) => "abc", "test");
            p.Equals(p);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [Timeout(2000)]
        public void Equals_OtherIsDifferentType_ThrowsNotSupportedException()
        {
            var p = new ValueCondition<string>((e) => "abc", "test");
            p.Equals("abc");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        [Timeout(2000)]
        public void GetHashCode_ThrowsNotSupportedException()
        {
            var p = new ValueCondition<string>((e) => "abc", "test");
            p.GetHashCode();
        }
    }
}
