// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.ReferenceLinks;
using Axe.Windows.Rules;

namespace Axe.Windows.RuleSelection
{
    /// <summary>
    /// Provides simplified access to links to Reference source material.
    /// Abstracts access to the ReferenceLinks extension.
    /// Ensures that predictable values are returned, even when the extension is not present.
    /// </summary>
    static class ReferenceLinks
    {
        private static readonly DefaultReferenceLinks DefaultLinks = new DefaultReferenceLinks();

        /// <summary>
        /// Gets information referring to the guideline from which a rule was derived.
        /// </summary>
        /// <param name="a11yCriteria"></param>
        /// <returns>Always returns empty strings, not null strings.</returns>
        public static (string ShortDescription, string Url) GetGuidelineInfo(A11yCriteriaId a11yCriteria)
        {
            var link = GetReferenceLink(a11yCriteria.ToString());
            if (link == null) return (string.Empty, string.Empty);

            return (link.ShortDescription ?? string.Empty,
                    TryGetValidUrl(link.Uri, out string url) ? url: string.Empty);
        }

        private static IReferenceLink GetReferenceLink(string lookupToken) => DefaultLinks.GetReferenceLink(lookupToken);

        private static bool TryGetValidUrl(System.Uri uri, out string url)
        {
            url = null;

            if (uri == null) return false;
            if (!uri.IsWellFormedOriginalString()) return false;

            url = uri.OriginalString;

            return true;
        }
    } // class
} // namespace
