// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using AccessibilityInsights.Desktop.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using System.IO.Fakes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AccessibilityInsights.DesktopTests.Settings
{
    /// <summary>
    /// Tests whether various values in the OtherProperties dictionary
    /// are properly deserialized in SnapshotMetaInfoTests
    /// </summary>
    [TestClass]
    public class SnapshotMetaInfoTests
    {
        [TestMethod]
        public void Deserialize_FirstColor_Standard()
        {
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.FirstColor, System.Windows.Media.Colors.Red }
            };
            Assert.AreEqual(System.Windows.Media.Colors.Red, GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.FirstColor]);
        }

        [TestMethod]
        public void Deserialize_SecondColor_Standard()
        {
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.SecondColor, System.Windows.Media.Colors.Red }
            };
            Assert.AreEqual(System.Windows.Media.Colors.Red, GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.SecondColor]);
        }

        [TestMethod]
        public void Deserialize_FirstColor_RedHex()
        {
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.FirstColor, "#FF0000" }
            };
            Assert.AreEqual(System.Windows.Media.Colors.Red, GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.FirstColor]);
        }

        [TestMethod]
        public void Deserialize_SecondColor_RedHex()
        {
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.SecondColor, "#FF0000" }
            };
            Assert.AreEqual(System.Windows.Media.Colors.Red, GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.SecondColor]);
        }

        [TestMethod]
        public void Deserialize_FirstPixel_Standard()
        {
            Dictionary<SnapshotMetaPropertyName, object> dictionary = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.FirstPixel, "10,10" }
            };
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = dictionary;
            Assert.AreEqual(new System.Drawing.Point(10, 10), GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.FirstPixel]);
        }

        [TestMethod]
        public void Deserialize_SecondPixel_Standard()
        {
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.SecondPixel, "10,10" }
            };
            Assert.AreEqual(new System.Drawing.Point(10, 10), GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.SecondPixel]);
        }
        
        [TestMethod]
        public void Deserialize_FirstPixel_Null()
        {
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.FirstPixel, null }
            };
            Assert.IsNull(GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.FirstPixel]);
        }

        [TestMethod]
        public void Deserialize_SecondPixel_Null()
        {
            Dictionary<SnapshotMetaPropertyName, object> otherProperties = new Dictionary<SnapshotMetaPropertyName, object>
            {
                { SnapshotMetaPropertyName.SecondPixel, null }
            };
            Assert.IsNull(GetDeserializedProperties(otherProperties)[SnapshotMetaPropertyName.SecondPixel]);
        }

        private static Dictionary<SnapshotMetaPropertyName, object> GetDeserializedProperties(Dictionary<SnapshotMetaPropertyName, object> inputProperties)
        {
            using (ShimsContext.Create())
            {
                SnapshotMetaInfo info = new SnapshotMetaInfo(A11yFileMode.Contrast,"Test", 0, 0, inputProperties);
                var text = JsonConvert.SerializeObject(info);
                ShimStreamReader.AllInstances.ReadToEnd = (_) => { return text; };
                using (var testStream = new MemoryStream())
                {
                    SnapshotMetaInfo metaInfo = SnapshotMetaInfo.DeserializeFromStream(testStream);
                    return metaInfo.OtherProperties;
                }
            }
        }

    }
}
