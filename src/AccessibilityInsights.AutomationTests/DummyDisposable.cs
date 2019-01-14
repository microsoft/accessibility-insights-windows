// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.AutomationTests
{
    /// <summary>
    /// Placeholder class for where we need an IDisposable. It exposes the disposed count at
    /// any point in time
    /// </summary>
    class DummyDisposable : IDisposable
    {
        public int TimesDisposed { get; private set; }

        /// <summary>
        /// Dispose method. We explicitly DO NOT use the "fancy" disposal, since we want to
        /// detect cases where the owner incorrectly double-disposes
        /// </summary>
        public void Dispose()
        {
            TimesDisposed++;
        }
    }
}
