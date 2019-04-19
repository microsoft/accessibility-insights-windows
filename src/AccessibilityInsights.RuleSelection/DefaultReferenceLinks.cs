// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using AccessibilityInsights.Extensions.Interfaces.ReferenceLinks;
using Axe.Windows.RuleSelection.Resources;
using Axe.Windows.Telemetry;

namespace Axe.Windows.RuleSelection
{
    /// <summary>
    /// Provides a guaranteed valid IReferenceLink object.
    /// </summary>
    class DefaultReferenceLinks : IReferenceLinks
    {
        public IReferenceLink GetReferenceLink(string lookupToken)
        {
            try
            {
                var url = DefaultGuidelineUrls.ResourceManager.GetString(lookupToken, CultureInfo.CurrentCulture);
                var shortDescription = DefaultGuidelineShortDescriptions.ResourceManager.GetString(lookupToken, CultureInfo.CurrentCulture);
                return new ReferenceLink(shortDescription, url);
            }
            catch (Exception e)
            {
                e.ReportException();
                return new ReferenceLink(DefaultGuidelineShortDescriptions.None);
            }
        }
    } // class
} // namespace
