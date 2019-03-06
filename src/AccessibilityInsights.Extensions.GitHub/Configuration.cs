// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    public class Configuration
    {
        public string RepoLink {get;}

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
