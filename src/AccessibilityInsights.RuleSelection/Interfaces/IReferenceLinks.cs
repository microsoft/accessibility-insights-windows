// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.Extensions.Interfaces.ReferenceLinks
{
    /// <summary>
    /// Provides links to reference documentation
    /// which the Axe.Windows rules are meant to validate.
    /// </summary>
    public interface IReferenceLinks
    {
        IReferenceLink GetReferenceLink(string lookupToken);
    } // class
} // namespace
