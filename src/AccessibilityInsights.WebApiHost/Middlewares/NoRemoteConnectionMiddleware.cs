// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace AccessibilityInsights.WebApiHost.Middlewares
{
    /// <summary>
    /// NoRemoteConnectionMiddleware
    /// Block a connection from remote box. 
    /// </summary>
    public class NoRemoteConnectionMiddleware : OwinMiddleware
    {
        public NoRemoteConnectionMiddleware(OwinMiddleware next): base(next) { }

        public async override Task Invoke(IOwinContext context)
        {
            if (IsLocalHost(context.Request.RemoteIpAddress))
            {
                await Next.Invoke(context);
            }
            else
            {
                Console.WriteLine("Request is from remote. rejected");
            }
        }

        private bool IsLocalHost(string ipAddress)
        {
            return ipAddress == "::1" || ipAddress == "127.0.0.1" || ipAddress == "localhost";
        }
    }
}
