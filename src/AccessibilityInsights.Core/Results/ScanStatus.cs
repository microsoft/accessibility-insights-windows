// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Core.Results
{
    /// <summary>
    /// Test status enum
    /// </summary>
#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    public enum ScanStatus
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
    {
        NoResult = 0,
        Pass = 1,
        Uncertain = 2,
        Fail = 3,
        ScanNotSupported = 4, // for the cases like HTML framework which we don't support. 
    }
}
