// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Net;


namespace AccessibilityInsights.Extensions.Helpers
{
    public static class GitHubHelper
    {
        public static bool TryGet(Uri uri, Stream stream, TimeSpan timeout)
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
                e.ReportException();
                System.Diagnostics.Trace.WriteLine("AccessibilityInsights upgrade - exception in GET request: "
                    + e.ToString());
                return false;
            }
        }
    }
}
