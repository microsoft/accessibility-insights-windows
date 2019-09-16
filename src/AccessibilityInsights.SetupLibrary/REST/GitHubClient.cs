// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Net;

namespace AccessibilityInsights.SetupLibrary.REST
{
    /// <summary>
    /// Class that actually makes calls to GitHub. So far, we're just making a simple GET call,
    /// with no GitHub-specific characteristics.
    /// </summary>
    public static class GitHubClient
    {
        /// <summary>
        /// Load the contents of the given Uri into a Stream
        /// </summary>
        public static void LoadUriContentsIntoStream(Uri uri, Stream stream, TimeSpan timeout)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = (int)timeout.TotalMilliseconds;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.GetResponseStream().CopyTo(stream);
                    return;
                }
            }

            throw new ArgumentException("Unable to get contents from " + uri.ToString(), nameof(uri));
        }
    }
}
