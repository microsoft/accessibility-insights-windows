// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions;
using System;
using System.Web.Http;

namespace AccessibilityInsights.WebApiHost.Controllers
{
    /// <summary>
    /// Controller class for Axe.Windows.Actions.SelectAction
    /// it will let external proces select a process or an element
    /// </summary>
    public class SelectController : ApiController
    {
        /// <summary>
        /// static constructor to initialize the default SelectAction instance. 
        /// </summary>
        static SelectController()
        {
            // make sure that Selection Action is not doing anything at the beginning. 
            var sa = SelectAction.GetDefaultInstance();
            sa.Stop();
            sa.ClearSelectedContext();
            sa.IsFocusSelectOn = false; // turn off Focus selection
            sa.IsMouseSelectOn = false; // turn off Mouse selection
            sa.Start(); // it allows caller to select an element or an App
        }

        /// <summary>
        /// Get the context id of currently selected element. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Current()
        {
            try
            {
                var sa = SelectAction.GetDefaultInstance();
                var gid = sa.GetSelectedElementContextId();

                if (gid.HasValue)
                {
                    return Ok(gid);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Select an App by PID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if there is any error, return BadRequest</returns>
        [HttpPost]
        public IHttpActionResult Process(int id)
        {
            try
            {
                var sa = SelectAction.GetDefaultInstance();
                sa.SetCandidateElementFromProcessId(id);
                var gid = SelectElement();

                return Ok(gid);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Select an element by Handle(HWnd)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if there is any error, return BadRequest</returns>
        [HttpPost]
        public IHttpActionResult Element(int id)
        {
            try
            {
                var sa = SelectAction.GetDefaultInstance();
                sa.SetCandidateElementFromHandle(new IntPtr(id));
                var gid = SelectElement();

                return Ok(gid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// select an element
        /// </summary>
        private Guid SelectElement()
        {
            var sa = SelectAction.GetDefaultInstance();

            sa.Select();

            var ecId = sa.GetSelectedElementContextId();
            if (ecId.HasValue == false)
            {
                throw new InvalidOperationException("Element is not selected.");
            }

            return ecId.Value;
        }
    }
}
