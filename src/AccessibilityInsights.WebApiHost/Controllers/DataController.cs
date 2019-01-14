// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using System;
using System.Web.Http;

namespace AccessibilityInsights.WebApiHost.Controllers
{
    /// <summary>
    /// ApiController for GetDataAction
    /// </summary>
    public class DataController : ApiController
    {
        [HttpGet]
        public IHttpActionResult ElementContext(Guid id)
        {
            try
            {
                // check whether Id exists first. 
                if(GetDataAction.ExistElementContext(id))
                {
                    return Ok(GetDataAction.GetElementContext(id));
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public IHttpActionResult DataContext(Guid id)
        {
            try
            {
                // check whether Id exist first. 
                if (GetDataAction.ExistElementContext(id))
                {
                    return Ok(GetDataAction.GetElementDataContext(id));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Get the element information in Live mode
        /// </summary>
        /// <param name="id">element context Id(Guid)</param>
        /// <param name="elementId">element Unique Id(int)</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LiveElement(Guid id,[FromUri(Name = "ElementId")] int elementId)
        {
            try
            {
                // check whether Id exist first. 
                if (GetDataAction.ExistElementContext(id))
                {
                    return Ok(GetDataAction.GetA11yElementWithLiveData(id,elementId));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
