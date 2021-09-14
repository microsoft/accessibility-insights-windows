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
            public TriState Status { get; set; }
            public Stream Stream { get; set; }
            public Action<int> ProgressCallback { get; set; }
        }

        /// <summary>
        /// Load the contents of the given Uri into a Stream
        /// </summary>
        public static void LoadUriContentsIntoStream(Uri uri, Stream stream, TimeSpan timeout, Action<int> progressCallback)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            Stopwatch stopwatch = Stopwatch.StartNew();
            DownloadState state = new DownloadState 
            {
                Stream = stream,
                ProgressCallback = progressCallback,
            };

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadDataCompleted += DownloadCompleted;
                    client.DownloadProgressChanged += ProgressChanged;
                    client.DownloadDataAsync(uri, state);

                    while (state.Status == TriState.Unknown)
                    {
                        if (stopwatch.ElapsedMilliseconds > timeout.TotalMilliseconds)
                        {
                            state.Status = TriState.Failure;
                            break;
                        }
                        Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                state.Status = TriState.Failure;
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                stopwatch.Stop();
            }

            if (state.Status != TriState.Success)
            {
                throw new ArgumentException("Unable to get contents from " + uri.ToString(), nameof(uri));
            }
        }

        private static void DownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            DownloadState state = e.UserState as DownloadState;

            try
            {
                state.Stream.Write(e.Result, 0, e.Result.Length);
                state.Status = TriState.Success;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
            {
                state.Status = TriState.Failure;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadState state = e.UserState as DownloadState;

            state.ProgressCallback?.Invoke(e.ProgressPercentage);
        }
    }
}
