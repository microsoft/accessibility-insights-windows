// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.Interfaces.ReferenceLinks
{
    public interface IReferenceLink
    {
        string ShortDescription { get; }
        Uri Uri { get; }
    } // class
} // namespace
