// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Attributes;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Actions
{
    /// <summary>
    /// Manager for actions
    /// it handles all internal data
    /// - ElementContext
    /// - DataContext
    /// - Element
    /// - Property
    /// - Pattern
    /// - etc
    /// When Actions needs data(object) to do anything(excute/select and etc), they should get data from Actions. 
    /// the Action caller will pass the ID of data to use than actual object. 
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public class DataManager:IDisposable
    {
        /// <summary>
        /// ElementContext dictionary
        /// keep the record of all element context added by AddElementContext
        /// </summary>
        Dictionary<Guid, ElementContext> ElementContexts = new Dictionary<Guid, ElementContext>();

        /// <summary>
        /// Get A11yPattern from an indicated element/elementcontext
        /// </summary>
        /// <param name="ecId">Element Context Id</param>
        /// <param name="eId">Element Id</param>
        /// <param name="ptId">Pattern Id</param>
        /// <returns></returns>
        internal A11yPattern GetA11yPattern(Guid ecId, int eId, int ptId)
        {
            return GetA11yElement(ecId, eId).Patterns.ById(ptId);
        }

        /// <summary>
        /// Set element Context
        /// </summary>
        /// <param name="ec"></param>
        internal void AddElementContext(ElementContext ec)
        {
            if (ec != null && this.ElementContexts.ContainsKey(ec.Id) == false)
            {
                this.ElementContexts.Add(ec.Id, ec);
            }
        }

        /// <summary>
        /// Get ElementContext 
        /// </summary>
        /// <param name="ecId"></param>
        /// <returns></returns>
        internal ElementContext GetElementContext(Guid ecId)
        {
            return this.ElementContexts.ContainsKey(ecId) ? this.ElementContexts[ecId] : null;
        }

        /// <summary>
        /// Remove Element Context and Dispose
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        internal void RemoveElementContext(Guid ecId)
        {
            if (ElementContexts.ContainsKey(ecId))
            {
                var ec = this.ElementContexts[ecId];

                this.ElementContexts.Remove(ecId);
                ec.Dispose();
            }
        }

        /// <summary>
        /// Remove DataContext from an ElementContext
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="keepMainElment">true, keep it</param>
        internal void RemoveDataContext(Guid ecId)
        {
            // check whether key exists. if not, just silently ignore. 
            if (this.ElementContexts.ContainsKey(ecId))
            {
                var ec = this.ElementContexts[ecId];
                if (ec.DataContext != null)
                {
                    // make sure that selected element is not disposed by removing it from the list in DataContext
                    ec.DataContext.Elements.Remove(ec.Element.UniqueId);

                    ec.DataContext.Dispose();
                    ec.DataContext = null;
                }
            }
        }

        /// <summary>
        /// Get Element by Id
        /// </summary>
        /// <param name="ecId">ElementContext Id</param>
        /// <param name="eId">Element Id</param>
        /// <returns></returns>
        internal A11yElement GetA11yElement(Guid ecId, int eId)
        {
            if (this.ElementContexts.ContainsKey(ecId)) 
            {
                var ec = ElementContexts[ecId];
                if (eId == 0)
                {
                    return ec.Element;
                }
                else
                {
                    var es = ElementContexts[ecId].DataContext.Elements;
                    if (es.ContainsKey(eId))
                    {
                        return es[eId];
                    }
                }
            }

            return null;
        }

        #region constants
        /// <summary>
        /// default instance name constant
        /// </summary>
        const string DefaultInstanceName = "AccessibilityInsightsDefault";
        #endregion

        #region static members
        static Dictionary<string, DataManager> sDataManagers = new Dictionary<string, DataManager>();

        /// <summary>
        /// Get default Data Manager instance
        /// if it doesn't exist, create one. 
        /// </summary>
        /// <returns></returns>
        public static DataManager GetDefaultInstance()
        {
            var dm = GetInstance(DefaultInstanceName);
            if(dm == null)
            {
                sDataManagers.Add(DefaultInstanceName, new DataManager());
            }

            return dm;
        }

        /// <summary>
        /// Get DataManager by key. 
        /// if it doesn't exist, create a new instance with the key. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static DataManager GetInstance(string key)
        {
            DataManager dm = null;
            if (sDataManagers.ContainsKey(key))
            {
                dm = sDataManagers[key];
            }
            else
            {
                dm = new DataManager();
                sDataManagers.Add(key, dm);
            }

            return dm;
        }

        /// <summary>
        /// Clear all Data Managers
        /// </summary>
        /// <returns></returns>
        public static void Clear()
        {
            sDataManagers.Values.AsParallel().ForAll(dm => dm.Dispose());
            sDataManagers.Clear();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DataManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
