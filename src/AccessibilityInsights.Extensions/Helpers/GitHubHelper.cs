using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
