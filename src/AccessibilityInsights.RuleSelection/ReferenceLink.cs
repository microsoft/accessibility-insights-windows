// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using AccessibilityInsights.Extensions.Interfaces.ReferenceLinks;

namespace AccessibilityInsights.RuleSelection
{
    class ReferenceLink : IReferenceLink
    {
        public string ShortDescription { get; private set; }
        public Uri Uri { get; private set; }

        public ReferenceLink(string shortDescription, string url)
        {
            ShortDescription = shortDescription;
            Uri = new Uri(url);
        }

        public ReferenceLink(string shortDescription)
        {
            ShortDescription = shortDescription;
        }
    } // class
} // namespace
