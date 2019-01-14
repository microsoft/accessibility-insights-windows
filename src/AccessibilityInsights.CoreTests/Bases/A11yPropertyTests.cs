// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;

namespace AccessibilityInsights.CoreTests.Bases
{
    /// <summary>
    /// Tests A11yProperty class
    /// </summary>
    [TestClass()]
    public class A11yPropertyTests
    {
        /// <summary>
        /// Tests A11yProperty.ToString() through A11yProperty.TextValue. 
        /// Also tests constructor.
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            A11yElement ke = A11yElementTests.FromJson("Resources/A11yPropertyTest.hier");            
            string kpVal;
            
            kpVal = ke.Properties[PropertyType.UIA_ControlTypePropertyId].ToString();
            Assert.AreEqual("Text(50020)", kpVal);
            
            kpVal = ke.Properties[PropertyType.UIA_RuntimeIdPropertyId].ToString();
            Assert.AreEqual("[7,1F48,24D4850]", kpVal);
            
            kpVal = ke.Properties[PropertyType.UIA_BoundingRectanglePropertyId].ToString();
            Assert.AreEqual("[l=1285,t=91,r=1368,b=116]", kpVal);

            kpVal = ke.Properties[PropertyType.UIA_OrientationPropertyId].ToString();
            Assert.AreEqual("None(0)", kpVal);

            kpVal = ke.Properties[PropertyType.UIA_LabeledByPropertyId].ToString();
            Assert.AreEqual("Test", kpVal);
                             
            kpVal = ke.Properties[PropertyType.UIA_HasKeyboardFocusPropertyId].ToString();
            Assert.AreEqual("False", kpVal);           
        }
    }
}
