// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.WebApiHost.Controllers;
using Microsoft.Owin.Hosting;

namespace AccessibilityInsights.WebApiHost
{
    class Program
    {
        // Specify the URI to use for the local host:
        const string BaseUri = "http://localhost:8080";

        [MTAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Accessibility Insights WebApi Host...");
            using (WebApp.Start<Startup>(BaseUri))
            {
                Console.WriteLine("Server running at {0}.", BaseUri);
                Console.WriteLine("Wait for api/Host/Exit post to exit...");

                HostController.WaitForExitEvent();
                Console.WriteLine("Accessibility Insights WebApi Host exits.");
            }
        }
    }
}
