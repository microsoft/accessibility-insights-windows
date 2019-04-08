// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Threading;

namespace Axe.Windows.Automation.PowerShell
{
    /// <summary>
    /// Simple class to adjust and restore the AppDomain around each call via PowerShell
    /// </summary>
    internal class AppDomainAdjuster : IDisposable
    {
        // This remembers our original Evidence
        private Evidence originalEvidence;

        internal AppDomainAdjuster()
        {
            // Save the original evidence
            this.originalEvidence = Thread.GetDomain().Evidence;

            // Create a new Evidence that include the MyComputer zone
            Evidence replacementEvidence = new Evidence();
            replacementEvidence.AddHostEvidence(new Zone(SecurityZone.MyComputer));

            SetAppDomainEvidence(replacementEvidence);
        }

        /// <summary>
        /// Use reflection to set the AppDomain's evidence
        /// </summary>
        /// <param name="evidence"></param>
        private static void SetAppDomainEvidence(Evidence evidence)
        {
            if (evidence != null)
            {
                AppDomain currentAppDomain = Thread.GetDomain();
                FieldInfo securityIdentityField = currentAppDomain.GetType().GetField("_SecurityIdentity", BindingFlags.Instance | BindingFlags.NonPublic);
                securityIdentityField.SetValue(currentAppDomain, evidence);
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
                    SetAppDomainEvidence(this.originalEvidence);
                    this.originalEvidence = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AppDomainAdjuster() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
