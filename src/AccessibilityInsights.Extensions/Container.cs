// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace AccessibilityInsights.Extensions
{
    /// <summary>
    /// Container class
    /// it contains all extensions for Accessibility Insights
    /// </summary>
    internal class Container : IDisposable
    {
        private CompositionContainer _container;
        private ResolveEventHandler _assemblyEventResolver;
        private readonly IEnumerable<string> _extensionPaths;
        const string ExtensionSearchPattern = "*.extensions.*.dll *.exe";

        internal static EventHandler<ReportExceptionEventArgs> ReportedExceptionEvent;

        /// <summary>
        /// Get all the extension sub folder names under ...\AccessibilityInsights\Extensions directory
        /// </summary>
        internal static IEnumerable<string> GetExtensionPaths()
        {
            string searchPath = Path.Combine(Path.GetDirectoryName(typeof(Container).Assembly.Location), @"Extensions");
            if (Directory.Exists(searchPath))
            {
                foreach (string directory in Directory.EnumerateDirectories(searchPath))
                {
                    yield return directory;
                }
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            foreach (string path in _extensionPaths)
            {
                string filePath = Path.Combine(path, assemblyName.Name + ".dll");
                if (File.Exists(filePath))
                {
                    try
                    {
                        return Assembly.LoadFrom(filePath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// constructor
        /// </summary>
        private Container()
        {
            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            // Adds all the parts found in the folder whether Microsoft.AcccessiblityInsights.Extentions.dll
            // later we will need to make it configurable.
            var dirCatalog = new DirectoryCatalog(Path.GetDirectoryName(typeof(Container).Assembly.Location), ExtensionSearchPattern);
            catalog.Catalogs.Add(dirCatalog);

            // Dynamically search for assemblies when it fails to resolve
            _extensionPaths = GetExtensionPaths();
            _assemblyEventResolver = new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += _assemblyEventResolver;

            // Adds all the parts found in ...\AccessibilityInsights\Extensions directory
            foreach (string path in _extensionPaths)
            {
                catalog.Catalogs.Add(new DirectoryCatalog(path, ExtensionSearchPattern));
            }

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
            catch
            {
                // Fail silently, since the code is designed to run without extensions
            }
        }

        #region extensions
        [Import(typeof(IAutoUpdate), AllowDefault = true)]
        public IAutoUpdate AutoUpdate { get; set; }

        [Import(typeof(ITelemetry), AllowDefault = true)]
        public ITelemetry Telemetry { get; set; }

        [ImportMany(typeof(IIssueReporting))]
        public List<IIssueReporting> IssueReporting { get; set; }

        #endregion

        #region static members
        readonly static object _lockObject = new object();
        static Container _defaultInstance = null;

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
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = new Container();
                    }
                }
            }

            return _defaultInstance;
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
                    if (_assemblyEventResolver != null)
                    {
                        AppDomain.CurrentDomain.AssemblyResolve -= _assemblyEventResolver;
                        _assemblyEventResolver = null;
                    }

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
