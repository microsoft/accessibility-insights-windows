// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Axe.Windows.Desktop.Settings
{
    /// <summary>
    /// Contains metadata for a snapshot file. Created when saving snapshot,
    /// loaded and used when opening snapshot
    /// </summary>
    public class SnapshotMetaInfo
    {
        /// <summary>
        /// Mode to return to after loading
        /// </summary>
        public A11yFileMode Mode { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        // these properties are serialized/deserialized via json. so can't make it readonly. 

        /// <summary>
        /// Property bag for extra values that don't belong in the DataContext but might
        /// be helpful when setting the view upon loading
        /// </summary>
        public Dictionary<SnapshotMetaPropertyName, object> OtherProperties { get; set; }

        /// <summary>
        /// Selected elements' unique IDs
        /// </summary>
        public List<int> SelectedItems { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public int ScreenshotElementId { get; set; }

        /// <summary>
        /// Rule Version string
        /// </summary>
        public string RuleVersion { get; set; }

        /// <summary>
        /// Version of Axe.Windows.Core assembly
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SnapshotMetaInfo() { }

        /// <summary>
        /// Constructor that takes single ID
        /// </summary>
        /// <param name="mode">The save mode</param>
        /// <param name="ruleVersion">Indicate the version of rule</param>
        /// <param name="selected">The selected item (null if no item is selected)</param>
        /// <param name="screlementId">the ID of an element which was used to grab the screenshot</param>
        /// <param name="otherProperties">values useful when setting the view upon loading this file</param>
        public SnapshotMetaInfo(A11yFileMode mode,string ruleVersion, int? selected, int screlementId, Dictionary<SnapshotMetaPropertyName, object> otherProperties = null)
        {
            this.Mode = mode;
            this.RuleVersion = ruleVersion;
            this.Version = Axe.Windows.Core.Misc.Utility.GetAppVersion();

            if (selected.HasValue)
            {
                this.SelectedItems = new List<int>() { selected.Value };
            }
            this.ScreenshotElementId = screlementId;
            this.OtherProperties = otherProperties;
        }

        /// <summary>
        /// Deserializes from the given stream
        ///     Converts any values in OtherProperties dictionary to appropriate types - e.g FirstColor / SecondColor to System.Windows.Media.Color
        /// </summary>
        /// <param name="metadataPart"></param>
        /// <returns></returns>
        public static SnapshotMetaInfo DeserializeFromStream(Stream metadataPart)
        {
            using (StreamReader reader = new StreamReader(metadataPart))
            {
                System.Drawing.PointConverter converter = new System.Drawing.PointConverter();
                string jsonMeta = reader.ReadToEnd();
                SnapshotMetaInfo initial = JsonConvert.DeserializeObject<SnapshotMetaInfo>(jsonMeta);
                ConvertSnapshotColor(initial.OtherProperties, SnapshotMetaPropertyName.FirstColor);
                ConvertSnapshotColor(initial.OtherProperties, SnapshotMetaPropertyName.SecondColor);
                ConvertSnapshotPoint(initial.OtherProperties, SnapshotMetaPropertyName.FirstPixel, converter);
                ConvertSnapshotPoint(initial.OtherProperties, SnapshotMetaPropertyName.SecondPixel, converter);
                return initial;
            }
        }

        /// <summary>
        /// Attempts to convert the specified property from its string representation in the loaded dictionary
        /// to its typed version. Sets the value to null if conversion fails. Intended for use with System.Windows.Media.Color
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="property"></param>
        private static void ConvertSnapshotColor(Dictionary<SnapshotMetaPropertyName, object> properties, SnapshotMetaPropertyName property)
        {
            object strVal = null;
            if (properties?.TryGetValue(property, out strVal) == true)
            {
                try
                {
                    properties[property] = System.Windows.Media.ColorConverter.ConvertFromString((string)strVal);
                }
                catch (NotSupportedException)
                {
                    properties[property] = null;
                }
            }
        }

        /// <summary>
        /// Attempts to convert the specified property from its string representation in the loaded dictionary
        /// to its typed version. Sets the value to null if conversion fails. Intended for use with System.Drawing.Point
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="property"></param>
        /// <param name="converter"></param>
        private static void ConvertSnapshotPoint(Dictionary<SnapshotMetaPropertyName, object> properties, 
            SnapshotMetaPropertyName property, System.Drawing.PointConverter converter)
        {
            object strVal = null;
            if (properties?.TryGetValue(property, out strVal) == true)
            {
                try
                {
                    properties[property] = converter.ConvertFromString((string)strVal);
                }
                catch (NotSupportedException)
                {
                    properties[property] = null;
                }
            }
        }
    }

    /// <summary>
    /// Modes to return to after loading
    /// </summary>
    public enum A11yFileMode
    {
        Inspect,
        Test,
        Contrast,
    }

    /// <summary>
    /// Appropriate keys to use in the OtherProperties dictionary 
    /// Be sure to modify serialization if needed when adding new values
    /// </summary>
    public enum SnapshotMetaPropertyName
    {
        FirstColor,
        SecondColor,
        FirstPixel,
        SecondPixel,
    }
}
