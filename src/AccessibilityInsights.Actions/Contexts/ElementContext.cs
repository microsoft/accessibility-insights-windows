// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Desktop.Utility;
using System;

namespace AccessibilityInsights.Actions.Contexts
{
    /// <summary>
    /// Class ElementContext
    /// Contain ElementContext information
    /// </summary>
    public class ElementContext:IDisposable
    {
        /// <summary>
        /// Process name of the selected element
        /// </summary>
        public string ProcessName { get; private set; }

        /// <summary>
        /// Element information
        /// </summary>
        public A11yElement Element { get; private set; }

        /// <summary>
        /// Indicate the Select Type (Live or Loaded)
        /// </summary>
        public SelectType SelectType { get; private set; }

        /// <summary>
        /// Element Context Id
        /// </summary>
        public Guid Id { get; internal set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        public ElementContext (A11yElement element)
        {
            this.Element = element;
            if (this.Element.PlatformObject == null)
            {
                this.SelectType = SelectType.Loaded;
                this.ProcessName = "Unknown";
            }
            else
            {
                this.SelectType = SelectType.Live;
                this.ProcessName = this.Element.GetProcessName();
            }

            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// it will be retrieved via GetSnapshotContext;
        /// </summary>
        ElementDataContext _DataContext = null;

        /// <summary>
        /// Data context for this element context
        /// </summary>
        public ElementDataContext DataContext
        {
            get
            {
                return _DataContext;
            }

            set
            {
                _DataContext?.Dispose();
                _DataContext = value;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.ProcessName = null;
                    if (this.DataContext != null)
                    {
                        this.DataContext?.Dispose();
                    }
                    else
                    {
                        this.Element.Dispose();
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
