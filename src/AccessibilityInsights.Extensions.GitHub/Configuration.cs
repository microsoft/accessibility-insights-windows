// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Configuration Connection Data
    /// </summary>
    public class ConnectionConfiguration
    {
        public string RepoLink { get; set; }

        public ConnectionConfiguration() : this(String.Empty)
        {
        }

        public ConnectionConfiguration(string repoLink)
        {
            this.RepoLink = repoLink;
        }
    }
}
