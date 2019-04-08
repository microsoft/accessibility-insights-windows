// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Axe.Windows.Core.Types.Tests
{
    /// <summary>
    /// For PropertyTypes and PlatformPropertyTypes
    /// </summary>
    [TestClass()]
    public class PropertyTypesTest
    {
        [TestMethod()]
        public void CheckExists()
        {
            Assert.AreEqual(true, PropertyType.GetInstance().Exists(PropertyType.UIA_AnnotationPattern_AnnotationTypeNamePropertyId));
            Assert.AreEqual(false, PropertyType.GetInstance().Exists(0));
        }

        [TestMethod()]
        public void CheckGetNameById()
        {
            Assert.AreEqual("AnnotationPattern.AnnotationTypeName", PropertyType.GetInstance().GetNameById(PropertyType.UIA_AnnotationPattern_AnnotationTypeNamePropertyId));
        }

        [TestMethod()]
        public void CheckExistsPlatform()
        {
            Assert.AreEqual(true, PlatformPropertyType.GetInstance().Exists(PlatformPropertyType.Platform_ProcessNamePropertyId));
            Assert.AreEqual(false, PlatformPropertyType.GetInstance().Exists(0));
        }

        [TestMethod()]
        public void CheckGetNameByIdPlatform()
        {
            Assert.AreEqual("ProcessName", PlatformPropertyType.GetInstance().GetNameById(PlatformPropertyType.Platform_ProcessNamePropertyId));
        }
    }
}
