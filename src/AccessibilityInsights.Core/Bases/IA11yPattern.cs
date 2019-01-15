// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Core.Bases
{
    public interface IA11yPattern
    {
        int Id { get; }
        T GetValue<T>(string propertyName);
    } // class
} // namespace
