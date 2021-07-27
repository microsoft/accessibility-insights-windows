// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace AccessibilityInsights.SetupLibrary.REST
{
    /// <summary>
    /// Class that actually makes calls to GitHub. So far, we're just making a simple GET call,
    /// with no GitHub-specific characteristics.
    /// </summary>
    public static class GitHubClient
    {
        private class DownloadState
        {
            public bool Complete { get; set; }
            public Stream Stream { get; set; }
        }

        /// <summary>
        /// Load the contents of the given Uri into a Stream
        /// </summary>
        public static void LoadUriContentsIntoStream(Uri uri, Stream stream, TimeSpan timeout)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            Stopwatch stopwatch = Stopwatch.StartNew();
            DownloadState state = new DownloadState { Stream = stream };

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadDataCompleted += DownloadCompleted;
                    client.DownloadDataAsync(uri, state);

                    while (!state.Complete)
                    {
                        if (stopwatch.ElapsedMilliseconds > timeout.TotalMilliseconds)
                        {
                            throw new TimeoutException("Timeout exceeded");
                        }
                        Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    }
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Unable to get contents from " + uri.ToString(), nameof(uri));
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private static void DownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            DownloadState state = e.UserState as DownloadState;

            state.Stream.Write(e.Result, 0, e.Result.Length);
            state.Complete = true;
        }
    }
}
