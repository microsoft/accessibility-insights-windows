// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Class to pass back metadata about a downloaded stream
    /// </summary>
    public class StreamMetadata
    {
        public Uri RequestUri { get; }

        public Uri ResponseUri { get; }

        public int DataByteCount { get; }

        public StreamMetadata(Uri originalUri, Uri responseUri, int dataByteCount)
        {
            RequestUri = originalUri;
            ResponseUri = responseUri;
            DataByteCount = dataByteCount;
        }
    }
}
