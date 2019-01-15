// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AccessibilityInsights.Actions.Contexts
{
    /// <summary>
    ///  Class ElementDataContext
    ///  Contain Data from Element Context
    /// </summary>
    public class ElementDataContext : IDisposable
    {
        /// <summary>
        /// TreeWalker Mode;
        /// </summary>
        public TreeViewMode TreeMode { get; internal set; }

        /// <summary>
        /// Data Context Mode for this 
        /// </summary>
        public DataContextMode Mode { get; internal set; }

        /// <summary>
        /// All elements in this data context
        /// Ancestors, descendants and selected element.
        /// </summary>
        public Dictionary<int, A11yElement> Elements { get; internal set; }

        /// <summary>
        /// Root Element in tree hierarchy. it is the top most anscestor.
        /// </summary>
        public A11yElement RootElment { get; internal set; }

        /// <summary>
        /// Selected Element to populate this Data context
        /// </summary>
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Focused element UniqueId in this data context
        /// typically it is used to set selected element in tree when an element is clicked from Automated checks.
        /// </summary>
        public int? FocusedElementUniqueId { get; set; }

        /// <summary>
        /// Current screenshot
        /// </summary>
        public Bitmap Screenshot { get; set; }

        /// <summary>
        /// Id of element which was used to grab the screenshot
        /// </summary>
        public int ScreenshotElementId { get; internal set; }

        /// <summary>
        /// Bounded counter (to track constraints for the maximum number of element)
        /// </summary>
        public BoundedCounter ElementCounter { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        internal ElementDataContext(A11yElement e, int maxElements)
        {
            this.Element = e;
            ElementCounter = new BoundedCounter(maxElements);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Screenshot?.Dispose();
                    this.Screenshot = null;
                    this.Element = null;
                    if (this.Elements != null)
                    {
                        if (this.Mode == DataContextMode.Live)
                        {
                            // IUIAutomation can become non-responsive if Dispose is called in parallel.
                            // Explicitly Dispose the Element Values here to avoid this.
                            foreach (var e in this.Elements.Values)
                            {
                                e.Dispose();
                            }
                        }
                        else
                        {
                            // so far when it gets into test, it works ok. 
                            // it will keep the same perf when switch back to Live from Test. 
                            this.Elements.Values.AsParallel().ForAll(e => e.Dispose());
                        }

                        this.Elements?.Clear();
                    }
                }

                disposedValue = true;
            }
        }

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
