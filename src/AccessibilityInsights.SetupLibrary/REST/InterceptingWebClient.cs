// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;

namespace AccessibilityInsights.SetupLibrary.REST
{
    /// <summary>
    /// This class allows us to access the fully resolved Uri which isn't otherwise exposed
    /// </summary>
    internal class InterceptingWebClient : WebClient
    {
        public Uri ResponseUri { get; private set; }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ResponseUri = new Uri(response.ResponseUri.AbsoluteUri);
            return response;
        }
    }
}
