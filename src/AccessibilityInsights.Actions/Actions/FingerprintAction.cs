// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Attributes;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Core.Fingerprint;
using Axe.Windows.Desktop.Settings;
using System;

namespace Axe.Windows.Actions.Actions
{
    /// <summary>
    /// FingerprintAction class
    /// Actions to help with fingerprints
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public static class FingerprintAction
    {
        /// <summary>
        /// Merge a results file into the local issue store. This clears all internal data contexts
        /// </summary>
        /// <param name="path">Path to the file to merge</param>
        /// <returns>The number of changes (additions or merges) to the store</returns>
        public static int MergeOutputFileToIssueStore(string path)
        {
            int changeCount = 0;
            try
            {
                Tuple<Guid, SnapshotMetaInfo> loadInfo = SelectAction.GetDefaultInstance().SelectLoadedData(path);

                ElementContext elementContext = DataManager.GetDefaultInstance().GetElementContext(loadInfo.Item1);
                IIssueStore storeToMerge = new OutputFileIssueStore(path, elementContext.DataContext.Elements.Values);

                changeCount = SessionIssueStore.GetInstance().MergeIssuesFromStore(storeToMerge);
            }
            catch (Exception)
            {
            }
            finally
            {
                SelectAction.GetDefaultInstance().ClearSelectedContext();
            }
            return changeCount;
        }
    }
}
