// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    ///  This class encapsulates our custom assembly resolution code. It's needed because we lose the
    ///  ability to decide via .NET's .config files when we're running inside of PowerShell.
    /// </summary>
    internal class CustomAssemblyResolver : IDisposable
    {
        private ResolveEventHandler _resolveEventHandler;

        // The override map. Key = the value used by dependencies, Value = the value that we ship.
        // System.Net.Http.Formatting intentionally has 2 versions for values. We ship different
        // versions of this assembly, depending on if we have the full app or just the automation
        // package. Having both entries allows us to load the assembly from either environment.
        private static readonly IReadOnlyDictionary<string, string> BindingOverrides = new Dictionary<string, string>
        {
            {
                "System.Net.Http.Formatting, Version=5.2.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "System.Net.Http.Formatting, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            },
            {
                "System.Net.Http.Formatting, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "System.Net.Http.Formatting, Version=5.2.6.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            },
            {
                "Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed",
                "Newtonsoft.Json, Version=10.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed"
            },
            {
                "Newtonsoft.Json, Version=8.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed",
                "Newtonsoft.Json, Version=10.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed"
            },
        };

        /// <summary>
        /// This handler allows us to force our dependent assemblies to load the .NET assemblies that we
        /// ship. This is the functional equivalent of the version binding that we'd have if we were a
        /// standalone app.
        /// </summary>
        /// <param name="sender">The object sending the request</param>
        /// <param name="args">Details about the assembly to be resolved</param>
        /// <returns>The loaded assembly if we override, null if not</returns>
        internal static Assembly EventHandler(object sender, ResolveEventArgs args)
        {
            // Always expect our files to be correct (less spam that way)
            if (args.Name.StartsWith("AccessibilityInsights.", StringComparison.Ordinal))
                return null;

            if (BindingOverrides.TryGetValue(args.Name, out string overrideName))
            {
#if DEBUG
                Console.WriteLine("*** Overrode {0} to {1}", args.Name, overrideName);
#endif
                return Assembly.Load(overrideName);
            }

#if DEBUG
            Console.WriteLine("*** No override for  {0}", args.Name);
#endif

            return null;
        }

        internal CustomAssemblyResolver()
        {
            _resolveEventHandler = new ResolveEventHandler(EventHandler);
            AppDomain.CurrentDomain.AssemblyResolve += _resolveEventHandler;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_resolveEventHandler != null)
                    {
                        AppDomain.CurrentDomain.AssemblyResolve -= _resolveEventHandler;
                        _resolveEventHandler = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
