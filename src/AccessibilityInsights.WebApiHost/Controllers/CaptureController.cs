// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Actions.Misc;
using Axe.Windows.RuleSelection;
using Axe.Windows.Core.Enums;
using Axe.Windows.Desktop.Settings;
using System;
using System.Net;
using System.Web.Http;

namespace AccessibilityInsights.WebApiHost.Controllers
{
    /// <summary>
    /// ApiController for CaptureAction
    /// </summary>
    public class CaptureController : ApiController
    {
        /// <summary>
        /// static constructor to initialize the default TestFactory.
        /// </summary>
        static CaptureController()
        {
        }

        [HttpPut]
        public IHttpActionResult Test(Guid id)
        {
            return CaptureData(id, DataContextMode.Test, TreeViewMode.Control, true);
        }

        [HttpPut]
        public IHttpActionResult Live(Guid id)
        {
            return CaptureData(id, DataContextMode.Live, TreeViewMode.Control, true);
        }

        IHttpActionResult CaptureData(Guid id,
                DataContextMode dm,
                TreeViewMode tvm,
                bool force)
        {
            try
            {
                if(CaptureAction.SetTestModeDataContext(id, dm, tvm, force))
                {
                    if(dm == DataContextMode.Test)
                    {
                        var dc = GetDataAction.GetElementDataContext(id);
                        dc.PublishScanResults();
                    }
                    return Ok();
                }
                else
                {
                    return StatusCode(HttpStatusCode.NotModified);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
