// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class Configuration
    {
        public string RepoLink { get; set; }

        public Configuration()
        {
            this.RepoLink = String.Empty;
        }

        public Configuration(string repoLink)
        {
            this.RepoLink = repoLink;
        }
    }
}
