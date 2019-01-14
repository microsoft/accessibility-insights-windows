// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Core.Bases
{
    /// <summary>
    /// Pattern Property class 
    /// </summary>
    public class A11yPatternProperty:IDisposable
    {
        public string Name { get; set; }

        public dynamic Value { get; set; }

        public string NodeValue
        {
            get
            {
                if (this.Value is string)
                {
                    return string.Format("{0} = \"{1}\"", this.Name, this.Value);
                }

                return string.Format("{0} = {1}", this.Name, this.Value);
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
                    this.Name = null;
                    this.Value = null;
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
