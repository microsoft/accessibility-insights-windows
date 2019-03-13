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
        /// Given a Uri, try to get its contents into a stream
        /// </summary>
        /// <returns>true if the call succeeded</returns>
        public static bool TryGet(Uri uri, Stream stream, TimeSpan timeout, IExceptionReporter exceptionReporter)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = (int)timeout.TotalMilliseconds;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        response.GetResponseStream().CopyTo(stream);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                exceptionReporter.ReportException(e);
                System.Diagnostics.Trace.WriteLine("AccessibilityInsights - exception in GET request: "
                    + e.ToString());
                return false;
            }
        }
    }
}
