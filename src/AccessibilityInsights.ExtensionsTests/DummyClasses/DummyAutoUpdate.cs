// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AccessibilityInsights.ExtensionsTests.DummyClasses
{
    /// <summary>
    ///  Dummy AutoUpdate implementation for container. No need to
    ///  implement the methods other than the boilerplate
    /// </summary>
    [Export(typeof(IAutoUpdate))]
    class DummyAutoUpdate : IAutoUpdate
    {
        public Task<AutoUpdateOption> UpdateOptionAsync => throw new NotImplementedException();

        public Uri ReleaseNotesUri => throw new NotImplementedException();

        public Version InstalledVersion => throw new NotImplementedException();

        public string ReleaseChannel => throw new NotImplementedException();

        public Uri ManifestRequestUri => throw new NotImplementedException();

        public Uri ManifestResponseUri => throw new NotImplementedException();

        public int ManifestSizeInBytes => throw new NotImplementedException();

        public TimeSpan? GetInitializationTime()
        {
            throw new NotImplementedException();
        }

        public TimeSpan? GetUpdateTime()
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResult> UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
