// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Deque.ColorContrast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deque.ColorContrast.Tests
{
    [TestClass()]
    public class CCColorPairTests
    {
        [TestMethod, Timeout(2000)]
        public void IsVisiblySimilarToTest()
        {
            CCColorPair colorPair1 = new CCColorPair(new DequeColor(0, 0, 0), new DequeColor(1, 1, 1));
            CCColorPair colorPair2 = new CCColorPair(new DequeColor(2, 2, 2), new DequeColor(3, 3, 3));

            Assert.IsTrue(colorPair1.IsVisiblySimilarTo(colorPair2));
        }
    }
}