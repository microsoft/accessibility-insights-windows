// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Attributes;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Actions.Misc;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Desktop.Settings;
using AccessibilityInsights.RuleSelection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Text;

namespace AccessibilityInsights.Actions
{
    /// <summary>
    /// Action for saving snapshot data
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public static class SaveAction
    {
        /// <summary>
        /// Names for files inside of zipped snapshot
        /// </summary>
        public const string elementFileName = "el.snapshot";
        public const string screenshotFileName = "scshot.png";
        public const string metatdataFileName = "metadata.json";

        /// <summary>
        /// Buffer size for stream copying
        /// </summary>
        const int buffSize = 0x1000;

        /// <summary>
        /// Save snapshot zip
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="path">The output file</param>
        /// <param name="bmp">The screenshot</param>
        /// <param name="focusedElementId">The ID of the element with the current focus</param>
        /// <param name="mode">The type of file being saved</param>
        /// <param name="otherProperties">Properties to add to the snapshot metadata</param>
        /// <param name="completenessMode">Mode to control selective removal of data from the output</param>
        public static void SaveSnapshotZip(string path, Guid ecId, int? focusedElementId, A11yFileMode mode, Dictionary<SnapshotMetaPropertyName, object> otherProperties = null, CompletenessMode completenessMode = CompletenessMode.Full)
        {
            var ec = DataManager.GetDefaultInstance().GetElementContext(ecId);

            using (FileStream str = File.Open(path, FileMode.Create))
            using (Package package = ZipPackage.Open(str, FileMode.Create))
            {
                if (completenessMode == CompletenessMode.Full)
                {
                    SaveSnapshotFromElement(focusedElementId, mode, otherProperties, completenessMode, ec, package, ec.DataContext.RootElment);
                }
                else
                {
                    using (A11yElement reducedRoot = SelectAction.GetDefaultInstance().POIElementContext.Element.GetScrubbedElementTree())
                    {
                        SaveSnapshotFromElement(focusedElementId, mode, otherProperties, completenessMode, ec, package, reducedRoot);
                    }
                }
            }
        }

        /// <summary>
        /// Private helper function (formerly in SaveSnapshotZip) to make it easier to call with different inputs
        /// </summary>
        private static void SaveSnapshotFromElement(int? focusedElementId, A11yFileMode mode, Dictionary<SnapshotMetaPropertyName, object> otherProperties, CompletenessMode completenessMode, Contexts.ElementContext ec, Package package, A11yElement root)
        {
            var json = JsonConvert.SerializeObject(root, Formatting.Indented);
            using (MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                AddStream(package, mStrm, elementFileName);
            }

            if (completenessMode == CompletenessMode.Full && ec.DataContext.Screenshot != null)
            {
                using (MemoryStream mStrm = new MemoryStream())
                {
                    ec.DataContext.Screenshot.Save(mStrm, System.Drawing.Imaging.ImageFormat.Png);
                    mStrm.Seek(0, SeekOrigin.Begin);

                    AddStream(package, mStrm, screenshotFileName);
                }
            }

            var meta = new SnapshotMetaInfo(mode, RuleRunner.GetRuleVersion(), focusedElementId, ec.DataContext.ScreenshotElementId, otherProperties);
            var jsonMeta = JsonConvert.SerializeObject(meta, Formatting.Indented);
            using (MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(jsonMeta)))
            {
                AddStream(package, mStrm, metatdataFileName);
            }
        }

        /// <summary>
        /// Will generate and save the sarif file at the specifed path from the specified element downwards
        /// </summary>
        /// <param name="path"> The path at which the generated file needs to be persisted </param>
        /// <param name="ecId"> the guid of the root element </param>
        /// <param name="deleteGeneratedResultsFile"> Whether the generated results should be deleted </param>
        /// <param name="otherProperties"></param>
        public static void SaveSarifFile(string path, Guid ecId, Boolean deleteGeneratedResultsFile)
        {
            ResultsFileSarifMapper.GenerateAndPersistSarifFile(path, ecId, deleteGeneratedResultsFile);
        }

        /// <summary>
        /// Add stream to package
        /// </summary>
        /// <param name="package"></param>
        /// <param name="stream"></param>
        /// <param name="Name"></param>
        private static void AddStream(Package package, Stream stream, string Name)
        {
            var partUri = PackUriHelper.CreatePartUri(new Uri(Name, UriKind.Relative));
            var part = package.CreatePart(partUri, "", CompressionOption.Normal);

            CopyStream(stream, part.GetStream());
        }

        /// <summary>
        /// Copy stream to target stream
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void CopyStream(Stream source, Stream target)
        {
            byte[] buf = new byte[buffSize];
            int bytesRead = 0;

            while ((bytesRead = source.Read(buf, 0, buffSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
            }
        }
    }
}
