// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace AccessibilityInsights.Extensions
{
    /// <summary>
    /// Container class
    /// it contains all extensions for Accessibility Insights
    /// </summary>
    internal class Container : IDisposable
    {
        private CompositionContainer _container;
        private const string ExtensionSearchPattern = "*.extensions.*.dll";

        internal static EventHandler<ReportExceptionEventArgs> ReportedExceptionEvent;

        /// <summary>
        /// Production ctor
        /// </summary>
        private Container() : this(null) { }

        /// <summary>
        /// Testable ctor
        /// </summary>
        /// <param name="searchPatternOverride">Allows override of search pattern for testing</param>
        internal Container(string searchPatternOverride)
        {
            string searchPattern = searchPatternOverride ?? ExtensionSearchPattern;

            // Suppress CA2000 since we want the object to persist until the end of the process
#pragma warning disable CA2000 // Dispose objects before losing scope
            // Adds all the parts found in the folder where Microsoft.AcccessiblityInsights.Extentions.dll lives
            var catalog = new DirectoryCatalog(Path.GetDirectoryName(typeof(Container).Assembly.Location), searchPattern);

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                // Fail silently, since the code is designed to run without extensions
                // and our Telemetry extension will always be null at this point
            }
#pragma warning restore CA1031 // Do not catch general exception types
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        #region extensions
        [Import(typeof(IAutoUpdate), AllowDefault = true)]
        public IAutoUpdate AutoUpdate { get; set; }

        [Import(typeof(ITelemetry), AllowDefault = true)]
        public ITelemetry Telemetry { get; set; }

        [ImportMany(typeof(IIssueReporting))]
        public IEnumerable<IIssueReporting> IssueReportingOptions { get; set; }

        #endregion

        #region static members
        readonly static object _lockObject = new object();
        static Container _defaultInstance;

        /// <summary>
        /// Report an Exception via ReportedExceptionEvent
        /// </summary>
        /// <param name="e">The Exception to buffer</param>
        /// <param name="sender">The sender (can be null)</param>
        internal static void ReportException(Exception e, object sender)
        {
            if (e != null && ReportedExceptionEvent != null)
            {
                ReportExceptionEventArgs args = new ReportExceptionEventArgs();
                args.ReportedException = e;
                ReportedExceptionEvent(sender, args);
            }
        }

        public static Container GetDefaultInstance()
        {
            if(_defaultInstance == null)
            {
                lock(_lockObject)
                {
#pragma warning disable CA1508 // Analyzer doesn't understand threading
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = new Container();
                    }
#pragma warning restore CA1508 // Analyzer doesn't understand threading
                }
            }

            return _defaultInstance;
        }

        /// <summary>
        /// Set the ReleaseChannel for update
        /// </summary>
        /// <param name="releaseChannel">The value to use in the AutoUpdate code</param>
        public static void SetAutoUpdateReleaseChannel(string releaseChannel)
        {
            ReleaseChannelProvider.ReleaseChannel = releaseChannel;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this._container == null)
                    {
                        this._container.Dispose();
                        this._container = null;
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
