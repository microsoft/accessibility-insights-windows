// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Axe.Windows.Rules
{
    /// <summary>
    /// Values indicate standards from which rules are derived.
    /// They map to sections of WCAG and Section 508 compliance documents.
    /// </summary>
    public enum A11yCriteriaId
    {
        /// <summary>
        /// See <a href="https://www.w3.org/TR/UNDERSTANDING-WCAG20/content-structure-separation-programmatic.html">the WCAG documentation</a>
        /// </summary>
        InfoAndRelationships,

        /// <summary>
        /// See <a href="https://www.w3.org/TR/UNDERSTANDING-WCAG20/keyboard-operation-keyboard-operable.html">the WCAG documentation</a>
        /// </summary>
        Keyboard,

        /// <summary>
        /// See <a href="https://www.w3.org/TR/UNDERSTANDING-WCAG20/ensure-compat-rsv.html">the WCAG documentation</a>
        /// </summary>
        NameRoleValue,

        /// <summary>
        /// See <a href="https://www.access-board.gov/guidelines-and-standards/communications-and-it/about-the-ict-refresh/final-rule/single-file-version#502-interoperability-assistive-technology">the Access Board documentation</a>
        /// </summary>
        ObjectInformation,

        /// <summary>
        /// See <a href="https://www.access-board.gov/guidelines-and-standards/communications-and-it/about-the-ict-refresh/final-rule/single-file-version#502-interoperability-assistive-technology">the Access Board documentation</a>
        /// </summary>
        AvailableActions,
    } // class
} // namespace
