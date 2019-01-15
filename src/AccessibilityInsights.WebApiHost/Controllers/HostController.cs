// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using System.Threading;
using System.Web.Http;

namespace AccessibilityInsights.WebApiHost.Controllers
{
    /// <summary>
    /// Controller class for Host process behavior
    /// </summary>
    public class HostController : ApiController
    {
        private static readonly AutoResetEvent HostExitEvent = new AutoResetEvent(false); // Event used to notify the end of worker thread

        /// <summary>
        /// Exit Host process
        /// </summary>
        [HttpPost]
        public void Exit()
        {
            // clean up Default SelectAction. 
            SelectAction.ClearDefaultInstance();

            HostExitEvent.Set();
        }

        public static void WaitForExitEvent()
        {
            HostExitEvent.WaitOne();
        }
    }
}
