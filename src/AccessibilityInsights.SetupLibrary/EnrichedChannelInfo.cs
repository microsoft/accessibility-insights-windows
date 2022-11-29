// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// A class to add upgrade manifest metadata to the upgrade manifest contents
    /// </summary>
    public class EnrichedChannelInfo : ChannelInfo
    {
        public StreamMetadata Metadata { get; set; }
    }
}
